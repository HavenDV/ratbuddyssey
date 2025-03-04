using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Ratbuddyssey.MultEQAvr;

#nullable disable
#pragma warning disable

namespace Ratbuddyssey.MultEQTcp;

[StructLayout(LayoutKind.Explicit, Size = 4)]
internal struct FloatInt32
{
    [FieldOffset(0)] private float Float;
    [FieldOffset(0)] private int Int32;

    private static FloatInt32 inst = new();
    public static int FloatToInt32(float value)
    {
        inst.Float = value;
        return inst.Int32;
    }
    public static float Int32ToFloat(int value)
    {
        inst.Int32 = value;
        return inst.Float;
    }
}

public enum ReceiveAll
{
    RCVALL_OFF = 0,
    RCVALL_ON = 1,
    RCVALL_SOCKETLEVELONLY = 2,
    RCVALL_IPLEVEL = 3,
}

public enum Protocol
{
    TCP = 6,
    UDP = 17,
    Unknown = -1
}

internal class AudysseyMultEQTcpIp
{
    #region Properties
    public char TransmitReceive { get; set; }
    public byte CurrentPacket { get; set; }
    public byte TotalPackets { get; set; }
    public ushort TotalLength { get; set; }
    public ushort CommandLength { get; } = 10;
    public string Command { get; set; }
    public byte Reserved { get; set; }
    public ushort DataLength { get; set; }
    [JsonIgnore]
    public byte[] ByteData { get; set; }
    public string CharData { get; set; }
    public int[] Int32Data { get; set; }
    public byte CheckSum { get; set; } = 0;
    #endregion
}

public class AudysseyMultEQTcpSniffer
{
    private static readonly object _locker = new();

    private readonly string AudysseySnifferFileName = "AudysseySniffer.json";

    private readonly TcpIP TcpHost = null;
    private readonly TcpIP TcpClient = null;

    private readonly Socket mainSocket = null;

    private byte[] packetData = new byte[65536];

    private byte[] ByteData = null;

    private AudysseyMultEQTcpIp audysseyMultEQTcpIp = new();
    private AudysseyMultEQAvr _audysseyMultEQAvr = null;

    private AudysseyMultEQAvr AudysseyMultEQAvr
    {
        get
        {
            return _audysseyMultEQAvr;
        }
        set
        {
            _audysseyMultEQAvr = value;
        }
    }

    public AudysseyMultEQTcpSniffer(AudysseyMultEQAvr audysseyMultEQAvr, string HostAddress, string ClientAddress)
    {
        AudysseyMultEQAvr = audysseyMultEQAvr;

        TcpHost = new TcpIP(HostAddress, 0, 0);
        TcpClient = new TcpIP(ClientAddress, 1256, 0);

        //For sniffing the socket to capture the packets has to be a raw socket, with the
        //address family being of type internetwork, and protocol being IP
        try
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP)
            {
                ReceiveBufferSize = 32768
            };

            //Bind the socket to the selected IP address IPAddress.Parse("192.168.50.66")
            mainSocket.Bind(new IPEndPoint(IPAddress.Parse(TcpHost.Address), TcpHost.Port));

            //Set the socket  options
            mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            mainSocket.IOControl(IOControlCode.ReceiveAll, new byte[] { (byte)ReceiveAll.RCVALL_ON, 0, 0, 0 }, new byte[] { (byte)ReceiveAll.RCVALL_ON, 0, 0, 0 });

            //Start receiving the packets asynchronously
            mainSocket.BeginReceive(packetData, 0, packetData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception exception)
        {
            MessageInteractions.Exception.Handle(new InvalidOperationException(
                $"{nameof(AudysseyMultEQTcpSniffer)}::constructor",
                exception)).Subscribe();
        }
    }

    ~AudysseyMultEQTcpSniffer()
    {
        ParseAvrToFile();
    }

    public string GetTcpHostAsString()
    {
        return TcpHost.Address;
    }

    public string GetTcpClientAsString()
    {
        return TcpClient.Address;
    }

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            var nReceived = mainSocket.EndReceive(ar);

            //Analyze the bytes received...
            ParseTcpIPData(packetData, nReceived);

            packetData = new byte[65536];

            //Another call to BeginReceive so that we continue to receive the incoming packets
            mainSocket.BeginReceive(packetData, 0, packetData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception exception)
        {
            MessageInteractions.Exception.Handle(new InvalidOperationException(
                $"{nameof(AudysseyMultEQTcpSniffer)}::{nameof(OnReceive)}",
                exception)).Subscribe();
        }
    }

    private void ParseTcpIPData(byte[] byteData, int nReceived)
    {
        //Since all protocol packets are encapsulated in the IP datagram
        //so we start by parsing the IP header and see what protocol data
        //is being carried by it and filter source and destination address.
        var ipHeader = new IPHeader(byteData, nReceived);
        if (ipHeader.SourceAddress.ToString().Equals(TcpClient.Address) ||
            ipHeader.DestinationAddress.ToString().Equals(TcpClient.Address))
        {
            //Now according to the protocol being carried by the IP datagram we parse 
            //the data field of the datagram if it carries TCP protocol
            if ((ipHeader.ProtocolType == Protocol.TCP) && (ipHeader.MessageLength > 0))
            {
                var tcpHeader = new TCPHeader(ipHeader.Data, ipHeader.MessageLength);
                //Now filter only on our Denon (or Marantz) receiver
                if (tcpHeader.SourcePort == TcpClient.Port.ToString() ||
                    tcpHeader.DestinationPort == TcpClient.Port.ToString())
                {
                    if (tcpHeader.MessageLength > 1)
                    {
                        ParseAvrData(tcpHeader.Data, tcpHeader.MessageLength);
                    }
                }
            }
        }
    }

    private void ParseAvrData(byte[] packetData, ushort packetLength)
    {
        try
        {
            var memoryStream = new MemoryStream(packetData, 0, packetLength);

            // If we want to filter only packets which we can decode the minimum
            // length of a packet with no data is header + checksum = 19 bytes.
            if (memoryStream.Length >= 19)
            {
                var array = packetData;
                Array.Resize<byte>(ref array, array.Length - 1);
                var CheckSum = CalculateChecksum(array);
                audysseyMultEQTcpIp.CheckSum = (byte)~CheckSum;

                var binaryReader = new BinaryReader(memoryStream);
                audysseyMultEQTcpIp.TransmitReceive = binaryReader.ReadChar();
                audysseyMultEQTcpIp.TotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16()); //BinaryPrimitives.ReverseEndianness(binaryReader.ReadUInt16());
                audysseyMultEQTcpIp.CurrentPacket = binaryReader.ReadByte();
                audysseyMultEQTcpIp.TotalPackets = binaryReader.ReadByte();
                audysseyMultEQTcpIp.Command = Encoding.ASCII.GetString(binaryReader.ReadBytes(audysseyMultEQTcpIp.CommandLength));
                audysseyMultEQTcpIp.Reserved = binaryReader.ReadByte();
                audysseyMultEQTcpIp.DataLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16()); //BinaryPrimitives.ReverseEndianness(binaryReader.ReadUInt16());
                if (audysseyMultEQTcpIp.DataLength == 0)
                {
                    //This is a packet without data
                    //Dispose irrelevant data
                    audysseyMultEQTcpIp.ByteData = null;
                    audysseyMultEQTcpIp.CharData = null;
                    audysseyMultEQTcpIp.Int32Data = null;
                }
                else
                {
                    //Read the Data
                    int ByteToRead = audysseyMultEQTcpIp.DataLength;
                    audysseyMultEQTcpIp.ByteData = binaryReader.ReadBytes(ByteToRead);
                    //Data can be single packet transfer of multiple packet transfer
                    if (audysseyMultEQTcpIp.TotalPackets == 0)
                    {
                        //This is a single packet transfer
                        audysseyMultEQTcpIp.CharData = System.Text.Encoding.UTF8.GetString(audysseyMultEQTcpIp.ByteData).ToString();
                        //Keep the growing array for multi-packet transfers as we will have ACK packets in between
                        //Dispose irrelevant data
                        audysseyMultEQTcpIp.Int32Data = null;
                    }
                    else
                    {
                        //This is a multiple packet transfer
                        audysseyMultEQTcpIp.CharData = null;
                        //First packet: create array and fill with data
                        if (audysseyMultEQTcpIp.CurrentPacket == 0)
                        {
                            ByteData = audysseyMultEQTcpIp.ByteData;
                        }
                        //Other packets: resize array and append data
                        else
                        {
                            //Store length of current array
                            var PreviousBytaDataLength = ByteData.Length;
                            //Resize the current array to add the new array
                            Array.Resize(ref ByteData, ByteData.Length + audysseyMultEQTcpIp.ByteData.Length);
                            //Append the new array to the current array
                            Array.Copy(audysseyMultEQTcpIp.ByteData, 0, ByteData, PreviousBytaDataLength, audysseyMultEQTcpIp.ByteData.Length);
                        }
                        //Last packet: store to class
                        if (audysseyMultEQTcpIp.CurrentPacket == audysseyMultEQTcpIp.TotalPackets)
                        {
                            //TotalPackets Data as int?
                            //Reverse endianness of byte data and store
                            audysseyMultEQTcpIp.Int32Data = ByteToInt32Array(ByteData);
                        }
                    }
                }
                audysseyMultEQTcpIp.CheckSum = binaryReader.ReadByte();
                if (CheckSum == audysseyMultEQTcpIp.CheckSum)
                {
                    var Serialized = JsonConvert.SerializeObject(audysseyMultEQTcpIp, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    // we are running asynchronous
                    lock (_locker)
                    {
                        //Dump to file for learning
                        File.AppendAllText(Environment.CurrentDirectory + "\\" + AudysseySnifferFileName, Serialized + "\n");
                    }
                    AudysseyMultEQAvr.Serialized += Serialized + "\n";
                    //Parse to parent to display? modify? re-transmit?
                    ParseAvrObject(audysseyMultEQTcpIp.TransmitReceive, audysseyMultEQTcpIp.Command, audysseyMultEQTcpIp.CharData, audysseyMultEQTcpIp.Int32Data);
                }
            }
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception exception)
        {
            MessageInteractions.Exception.Handle(new InvalidOperationException(
                $"{nameof(AudysseyMultEQTcpSniffer)}::{nameof(ParseAvrData)}",
                exception)).Subscribe();
        }
    }

    private void Invoke(Action action)
    {
        throw new NotImplementedException();
    }

    private void ParseAvrToFile()
    {
        if (File.Exists(Environment.CurrentDirectory + "\\" + AudysseySnifferFileName))
        {
            var AvrStrings = File.ReadAllLines(Environment.CurrentDirectory + "\\" + AudysseySnifferFileName);
            foreach (var _AvrString in AvrStrings)
            {
                audysseyMultEQTcpIp = JsonConvert.DeserializeObject<AudysseyMultEQTcpIp>(_AvrString, new JsonSerializerSettings { });
                ParseAvrObject(audysseyMultEQTcpIp.TransmitReceive, audysseyMultEQTcpIp.Command, audysseyMultEQTcpIp.CharData, audysseyMultEQTcpIp.Int32Data);
            }
            var AvrString = JsonConvert.SerializeObject(_audysseyMultEQAvr, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(Environment.CurrentDirectory + "\\" + System.IO.Path.ChangeExtension(AudysseySnifferFileName, ".aud"), AvrString);
        }
    }

    private void ParseAvrObject(char TransmitReceive, string CmdString, string AvrString, int[] AvrData)
    {
        switch (CmdString)
        {
            case "GET_AVRINF":
                if (TransmitReceive == 'R')
                {
                    JsonConvert.PopulateObject(AvrString, _audysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IInfo)),
                        FloatParseHandling = FloatParseHandling.Decimal,
                    });
                }
                break;
            case "GET_AVRSTS":
                if (TransmitReceive == 'R')
                {
                    JsonConvert.PopulateObject(AvrString, _audysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IStatus)),
                        FloatParseHandling = FloatParseHandling.Decimal,
                    });

                }
                break;
            case "ENTER_AUDY":
                break;
            case "EXIT_AUDMD":
                break;
            case "SET_SETDAT":
                if (TransmitReceive == 'T')
                {
                    JsonConvert.PopulateObject(AvrString, _audysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAmp)),
                        FloatParseHandling = FloatParseHandling.Decimal,
                    });
                    JsonConvert.PopulateObject(AvrString, _audysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAudy)),
                        FloatParseHandling = FloatParseHandling.Decimal,
                    });
                }
                break;
            case "SET_DISFIL":
                if (TransmitReceive == 'T')
                {
                    _audysseyMultEQAvr.DisFil.Add(JsonConvert.DeserializeObject<AvrDisFil>(AvrString, new JsonSerializerSettings { }));
                }

                break;
            case "INIT_COEFS":
                break;
            case "SET_COEFDT":
                if (TransmitReceive == 'T')
                {
                    if (AvrData != null)
                    {
                        _audysseyMultEQAvr.CoefData.Add(AvrData);
                    }
                }

                break;
        }
    }

    private int[] ByteToInt32Array(byte[] Byte)
    {
        int[] Int32s = null;
        if (Byte.Length % 4 == 0)
        {
            Int32s = new int[Byte.Length / 4];
            for (var i = 0; i < Byte.Length / 4; i++)
            {
                Int32s[i] = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(Byte, i * 4));
            }
        }
        return Int32s;
    }

    private byte[] Int32ToByteArray(int[] Int32s)
    {
        var Byte = new byte[4 * Int32s.Length];
        for (var i = 0; i < Int32s.Length; i++)
        {
            Array.Copy(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(Int32s[i])), 0, Byte, i * 4, 4);
        }
        return Byte;
    }

    private byte CalculateChecksum(byte[] dataToCalculate)
    {
        return dataToCalculate.Aggregate((r, n) => r += n);
    }
}
