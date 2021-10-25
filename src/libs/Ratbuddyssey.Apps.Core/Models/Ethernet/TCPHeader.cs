using System.Net;

namespace Ratbuddyssey;

public class TCPHeader
{
    //TCP header fields
    private readonly ushort usSourcePort;              //Sixteen bits for the source port number
    private readonly ushort usDestinationPort;         //Sixteen bits for the destination port number
    private readonly uint uiSequenceNumber = 555;          //Thirty two bits for the sequence number
    private readonly uint uiAcknowledgementNumber = 555;   //Thirty two bits for the acknowledgement number
    private readonly ushort usDataOffsetAndFlags = 555;      //Sixteen bits for flags and data offset
    private readonly ushort usWindow = 555;                  //Sixteen bits for the window size
    private readonly short sChecksum = 555;                 //Sixteen bits for the checksum
                                                   //(checksum can be negative so taken as short)
    private readonly ushort usUrgentPointer;           //Sixteen bits for the urgent pointer
                                              //End TCP header fields

    private readonly byte byHeaderLength;            //Header length
    private readonly ushort usMessageLength;           //Length of the data being carried
    private readonly byte[] byTCPData = Array.Empty<byte>();//Data carried by the TCP packet

    public TCPHeader(byte[] byBuffer, int nReceived)
    {
        try
        {
            var memoryStream = new MemoryStream(byBuffer, 0, nReceived);
            var binaryReader = new BinaryReader(memoryStream);

            //The first sixteen bits contain the source port
            usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //The next sixteen contain the destiination port
            usDestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //Next thirty two have the sequence number
            uiSequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

            //Next thirty two have the acknowledgement number
            uiAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

            //The next sixteen bits hold the flags and the data offset
            usDataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //The next sixteen contain the window size
            usWindow = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //In the next sixteen we have the checksum
            sChecksum = (short)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //The following sixteen contain the urgent pointer
            usUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //The data offset indicates where the data begins, so using it we
            //calculate the header length
            byHeaderLength = (byte)(usDataOffsetAndFlags >> 12);
            byHeaderLength *= 4;

            if (byHeaderLength > 0)
            {
                //Message length = Total length of the TCP packet - Header length
                usMessageLength = (ushort)(nReceived - byHeaderLength);
                //Copy the TCP data into the data buffer
                byTCPData = new byte[nReceived - byHeaderLength];
                Array.Copy(byBuffer, byHeaderLength, byTCPData, 0, nReceived - byHeaderLength);
            }
            else
            {
                usMessageLength = 0;
                byTCPData = Array.Empty<byte>();
            }
        }
        catch (Exception exception)
        {
            MessageInteractions.Exception.Handle(new InvalidOperationException(
                $"TCP HeaderReceived: {nReceived} HeaderLength: {byHeaderLength}",
                exception)).Subscribe();
        }
    }

    public string SourcePort
    {
        get
        {
            return usSourcePort.ToString();
        }
    }

    public string DestinationPort
    {
        get
        {
            return usDestinationPort.ToString();
        }
    }

    public string SequenceNumber
    {
        get
        {
            return uiSequenceNumber.ToString();
        }
    }

    public string AcknowledgementNumber
    {
        get
        {
            //If the ACK flag is set then only we have a valid value in
            //the acknowlegement field, so check for it beore returning 
            //anything
            if ((usDataOffsetAndFlags & 0x10) != 0)
            {
                return uiAcknowledgementNumber.ToString();
            }
            else
            {
                return "";
            }
        }
    }

    public string HeaderLength
    {
        get
        {
            return byHeaderLength.ToString();
        }
    }

    public string WindowSize
    {
        get
        {
            return usWindow.ToString();
        }
    }

    public string UrgentPointer
    {
        get
        {
            //If the URG flag is set then only we have a valid value in
            //the urgent pointer field, so check for it beore returning 
            //anything
            if ((usDataOffsetAndFlags & 0x20) != 0)
            {
                return usUrgentPointer.ToString();
            }
            else
            {
                return "";
            }
        }
    }

    public string Flags
    {
        get
        {
            //The last six bits of the data offset and flags contain the
            //control bits

            //First we extract the flags
            var nFlags = usDataOffsetAndFlags & 0x3F;

            var strFlags = string.Format("0x{0:x2} (", nFlags);

            //Now we start looking whether individual bits are set or not
            if ((nFlags & 0x01) != 0)
            {
                strFlags += "FIN, ";
            }
            if ((nFlags & 0x02) != 0)
            {
                strFlags += "SYN, ";
            }
            if ((nFlags & 0x04) != 0)
            {
                strFlags += "RST, ";
            }
            if ((nFlags & 0x08) != 0)
            {
                strFlags += "PSH, ";
            }
            if ((nFlags & 0x10) != 0)
            {
                strFlags += "ACK, ";
            }
            if ((nFlags & 0x20) != 0)
            {
                strFlags += "URG";
            }
            strFlags += ")";

            if (strFlags.Contains("()"))
            {
                strFlags = strFlags.Remove(strFlags.Length - 3);
            }
            else if (strFlags.Contains(", )"))
            {
                strFlags = strFlags.Remove(strFlags.Length - 3, 2);
            }

            return strFlags;
        }
    }

    public string Checksum
    {
        get
        {
            //Return the checksum in hexadecimal format
            return string.Format("0x{0:x2}", sChecksum);
        }
    }

    public byte[] Data
    {
        get
        {
            return byTCPData;
        }
    }

    public ushort MessageLength
    {
        get
        {
            return usMessageLength;
        }
    }
}
