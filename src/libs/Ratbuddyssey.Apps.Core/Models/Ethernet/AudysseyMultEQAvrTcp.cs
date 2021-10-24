﻿using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable disable

namespace Ratbuddyssey.MultEQAvr;

public class AudysseyMultEQAvrTcp : INotifyPropertyChanged
{
    private AudysseyMultEQAvr _audysseyMultEQAvr = null;

    private readonly TcpIP TcpClient = null;
    private AudysseyMultEQAvrTcpClientWithTimeout audysseyMultEQAvrTcpClientWithTimeout = null;

    #region Properties
    public AudysseyMultEQAvr audysseyMultEQAvr
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
    #endregion

    private const string NACK = "{\"Comm\":\"NACK\"}";
    private const string ACK = "{\"Comm\":\"ACK\"}";
    private const string INPROGRESS = "{\"Comm\":\"INPROGRESS\"}";
    private const string AUDYFINFLG = "{\"AudyFinFlg\":\"Fin\"}";

    public string GetTcpClientAsString()
    {
        return TcpClient.Address;
    }

    public TcpIP GetTcpClient()
    {
        return TcpClient;
    }

    public AudysseyMultEQAvrTcp(AudysseyMultEQAvr audysseyMultEQAvr, string ClientAddress)
    {

        _audysseyMultEQAvr = audysseyMultEQAvr;
        TcpClient = new TcpIP(ClientAddress, 1256, 5000);
    }

    public void Connect()
    {
        audysseyMultEQAvrTcpClientWithTimeout = new AudysseyMultEQAvrTcpClientWithTimeout(TcpClient.Address, TcpClient.Port, TcpClient.Timeout);
    }

    private string MakeQuery(string Serialized)
    {
        var SerializedJObject = JObject.Parse(Serialized);
        foreach (var prop in SerializedJObject.Properties()) { prop.Value = "?"; }
        return SerializedJObject.ToString(Formatting.None);
    }

    public bool GetAvrInfo()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "GET_AVRINF";
            Console.Write(CmdString);
            // build JSON and replace values with "?"
            var AvrString = MakeQuery(JsonConvert.SerializeObject(_audysseyMultEQAvr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IInfo))
            }));
            Console.WriteLine(AvrString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, AvrString);
            // receive response
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            // parse JSON to class member variables
            if (!string.IsNullOrEmpty(AvrString))
            {
                JsonConvert.PopulateObject(AvrString, _audysseyMultEQAvr, new JsonSerializerSettings
                {
                    ContractResolver = new InterfaceContractResolver(typeof(IInfo)),
                    FloatParseHandling = FloatParseHandling.Decimal
                });
            }
            return (CmdString.Equals("GET_AVRINF") && !AvrString.Equals(NACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool GetAvrStatus()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "GET_AVRSTS";
            Console.Write(CmdString);
            // build JSON and replace values with "?"
            var AvrString = MakeQuery(JsonConvert.SerializeObject(_audysseyMultEQAvr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IStatus))
            }));
            Console.WriteLine(AvrString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, AvrString);
            // receive rseponse
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            // parse JSON to class member variables
            if (!string.IsNullOrEmpty(AvrString))
            {
                JsonConvert.PopulateObject(AvrString, _audysseyMultEQAvr, new JsonSerializerSettings
                {
                    ContractResolver = new InterfaceContractResolver(typeof(IStatus)),
                    FloatParseHandling = FloatParseHandling.Decimal,
                });
            }
            return (CmdString.Equals("GET_AVRSTS") && !AvrString.Equals(NACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool EnterAudysseyMode()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "ENTER_AUDY";
            Console.WriteLine(CmdString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, "");
            // receive rseponse
            string AvrString;
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            return (CmdString.Equals("ENTER_AUDY") && AvrString.Equals(ACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool ExitAudysseyMode()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "EXIT_AUDMD";
            Console.WriteLine(CmdString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, "");
            // receive rseponse
            string AvrString;
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            if (!(CmdString.Equals("EXIT_AUDMD") && AvrString.Equals(ACK) && CheckSumChecked))
            {
                return false;
            }
            // transmit request
            var Data = new byte[] { 0x1b };
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(Data);
            // receive rseponse
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            if (!(CmdString.Equals("EXIT_AUDMD") && AvrString.Equals(NACK) && CheckSumChecked))
            {
                return false;
            }

            Data[0] = 0x04;
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(Data);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetAudysseyFinishedFlag()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "SET_SETDAT";
            Console.WriteLine(CmdString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, AUDYFINFLG);
            // receive rseponse
            string AvrString;
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            return (CmdString.Equals("SET_SETDAT") && AvrString.Equals(ACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool SetAvrSetAmp()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "SET_SETDAT";
            Console.Write(CmdString);
            // clear finflag
            //_audysseyMultEQAvr.AudyFinFlg = "NotFin";  //TODO what does this flag do?
            // build JSON for class Dat on interface Iamp
            var AvrString = JsonConvert.SerializeObject(_audysseyMultEQAvr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IAmp))
            });
            Console.WriteLine(AvrString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, AvrString);
            // receive rseponse
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            return (CmdString.Equals("SET_SETDAT") && AvrString.Equals(ACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool SetAvrSetAudy()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "SET_SETDAT";
            Console.Write(CmdString);
            // build JSON for class Dat on interface IAudy
            var AvrString = JsonConvert.SerializeObject(_audysseyMultEQAvr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IAudy))
            });
            Console.WriteLine(AvrString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, AvrString);
            // receive rseponse
            audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
            Console.Write(CmdString);
            Console.WriteLine(AvrString);
            return (CmdString.Equals("SET_SETDAT") && AvrString.Equals(ACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool SetAvrSetDisFil()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            // there are multiple speaker?
            foreach (var AvrDisFil in _audysseyMultEQAvr.DisFil)
            {
                var CheckSumChecked = false;
                var CmdString = "SET_DISFIL";
                Console.Write(CmdString);
                // build JSON
                var AvrString = JsonConvert.SerializeObject(AvrDisFil, new JsonSerializerSettings { });
                Console.WriteLine(AvrString);
                // transmit request
                audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, AvrString);
                // receive rseponse
                audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
                Console.Write(CmdString);
                Console.WriteLine(AvrString);
                // check every transmission
                if (!(CmdString.Equals("SET_DISFIL") && AvrString.Equals(ACK) && CheckSumChecked))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetAvrInitCoefs()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            var CheckSumChecked = false;
            var CmdString = "INIT_COEFS";
            Console.WriteLine(CmdString);
            // transmit request
            audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, "");
            // response may take some processing time for the receiver
            var TimeElapsed = Stopwatch.StartNew();
            string AvrString;
            do
            {   // receive reseponse
                audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
                Console.Write(CmdString);
                Console.WriteLine(AvrString);
            } while ((TimeElapsed.ElapsedMilliseconds < 10000) && AvrString.Equals(INPROGRESS));
            return (CmdString.Equals("INIT_COEFS") && AvrString.Equals(ACK) && CheckSumChecked);
        }
        else
        {
            return false;
        }
    }

    public bool SetAvrSetCoefDt()
    {
        if (audysseyMultEQAvrTcpClientWithTimeout != null)
        {
            // data for each speaker... this is a very dumb binary data pump... payload must be SECRET!
            foreach (var Coef in _audysseyMultEQAvr.CoefData)
            {
                // transmit packets in chunks of 512 bytes
                var total_byte_packets = (Coef.Length * 4) / 512;
                // the last packet may have less than 512 bytes
                var last_packet_length = Coef.Length - (total_byte_packets * 128);
                // count for all packets
                if (last_packet_length > 0)
                {
                    total_byte_packets++;
                }
                // transmit all the packets
                for (var current_packet = 0; current_packet < total_byte_packets; current_packet++)
                {
                    var CopyData = current_packet < total_byte_packets - 1 ? new int[128] : new int[last_packet_length];
                    Array.Copy(Coef, current_packet * 128, CopyData, 0, current_packet < total_byte_packets - 1 ? 128 : last_packet_length);
                    var CheckSumChecked = false;
                    var CmdString = "SET_COEFDT";
                    Console.Write(CmdString);
                    Console.WriteLine(Coef.ToString());
                    // transmit request
                    audysseyMultEQAvrTcpClientWithTimeout.TransmitTcpAvrStream(CmdString, audysseyMultEQAvrTcpClientWithTimeout.Int32ToByte(CopyData), current_packet, total_byte_packets - 1); ;
                    string AvrString;
                    // receive rseponse
                    audysseyMultEQAvrTcpClientWithTimeout.ReceiveTcpAvrStream(ref CmdString, out AvrString, out CheckSumChecked);
                    Console.Write(CmdString);
                    Console.WriteLine(AvrString);
                    // success if all succeed
                    if (false == (CmdString.Equals("SET_COEFDT") && AvrString.Equals(ACK) && CheckSumChecked))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    #region INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    private void RaisePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}
