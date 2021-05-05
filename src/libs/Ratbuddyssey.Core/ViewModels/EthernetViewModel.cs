using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Audyssey.MultEQAvr;
using Audyssey.MultEQAvrAdapter;
using Audyssey.MultEQTcp;
using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;

#nullable enable

namespace Audyssey.ViewModels
{
    public class EthernetViewModel : ViewModelBase
    {
        #region Properties

        private AudysseyMultEQAvr? Avr { get; set; }
        private AudysseyMultEQAvrAdapter? AvrAdapter { get; set; }

        private AudysseyMultEQAvrTcp? AvrTcp { get; set; }
        private AudysseyMultEQTcpSniffer? TcpSniffer { get; set; }

        private static string TcpClientFileName { get; } = "TcpClient.json";

        public IReadOnlyCollection<string> Ips { get; } 

        [Reactive]
        public string? SelectedIp { get; set; }

        public IReadOnlyCollection<string> InterfaceIps { get; } = Array.Empty<string>();

        [Reactive]
        public string? SelectedInterfaceIp { get; set; }

        #endregion

        #region Constructors

        public EthernetViewModel()
        {
            Ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .Select(static ip => $"{ip}")
                .ToArray();
            SelectedIp = Ips.LastOrDefault();

            var path = Environment.CurrentDirectory + "\\" + TcpClientFileName;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var tcpClient = JsonConvert.DeserializeObject<TcpIP>(json);
                    if (tcpClient != null)
                    {
                        InterfaceIps = new[] { tcpClient.Address };
                        SelectedInterfaceIp = tcpClient.Address;
                    }
                }
            }

            for (var x = 0; x < 61; x++)
            {
                var fcentre = Math.Pow(10.0, 3.0) * Math.Pow(2.0, (x - 34.0) / 6.0);
                Console.Write(x); Console.Write(" ");
                Console.WriteLine("{0:N1}", fcentre);
            }
        }

        #endregion
    }
}
