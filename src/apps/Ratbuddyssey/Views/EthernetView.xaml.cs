using Audyssey;
using Audyssey.MultEQAvr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Audyssey.MultEQAvrAdapter;
using Audyssey.MultEQTcp;

namespace Ratbuddyssey.Views
{
    public partial class EthernetView
    {
        #region Properties

        public AudysseyMultEQAvr Avr { get; set; }
        public AudysseyMultEQAvrAdapter AvrAdapter { get; set; }

        private AudysseyMultEQAvrTcp AvrTcp { get; set; }
        private AudysseyMultEQTcpSniffer TcpSniffer { get; set; }

        private static string TcpClientFileName { get; } = "TcpClient.json";

        #endregion

        #region Constructors

        public EthernetView()
        {
            InitializeComponent();

            var hostEntry = System.Net.Dns.GetHostEntry((System.Net.Dns.GetHostName()));
            if (hostEntry.AddressList.Length > 0)
            {
                foreach (var ip in hostEntry.AddressList)
                {
                    cmbInterfaceHost.Items.Add(ip.ToString());
                }
                cmbInterfaceHost.SelectedIndex = cmbInterfaceHost.Items.Count - 1;
            }

            if (File.Exists(Environment.CurrentDirectory + "\\" + TcpClientFileName))
            {
                var text = File.ReadAllText(Environment.CurrentDirectory + "\\" + TcpClientFileName);
                if (text.Length > 0)
                {
                    var tcpClient = JsonConvert.DeserializeObject<TcpIP>(text);
                    if (tcpClient != null)
                    {
                        cmbInterfaceClient.Items.Add(tcpClient.Address);
                        cmbInterfaceClient.SelectedIndex = cmbInterfaceClient.Items.Count - 1;
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


        #region Methods

        private void LoadAvr(string fileName)
        {
            var json = File.ReadAllText(fileName);

            Avr = JsonConvert.DeserializeObject<AudysseyMultEQAvr>(json);
            AvrAdapter ??= new AudysseyMultEQAvrAdapter(Avr); // TODO: Bug after second load different file?
        }

        private void SaveAvr(string fileName)
        {
            if (Avr == null)
            {
                return;
            }

            var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            File.WriteAllText(fileName, json);
        }

        private static void RunAsAdmin()
        {
            try
            {
                var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path, "/run_elevated_action")
                {
                    Verb = "runas"
                });
                process?.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223)
                {
                    System.Windows.Forms.MessageBox.Show("Sniffer needs elevated rights for raw socket!", "Warning");
                }
                else
                {
                    throw;
                }
            }
        }

        private static bool IsElevated()
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);

            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        #endregion

        private void openProjectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "AudysseySniffer.aud",
                DefaultExt = ".aud",
                Filter = "Audyssey sniffer (*.aud)|*.aud",
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            LoadAvr(dialog.FileName);

            if (Avr != null)
            {
                DataContext = Avr;
            }
        }

        private void saveProjectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "AudysseySniffer.aud",
                DefaultExt = ".aud",
                Filter = "Audyssey sniffer (.aud)|*.aud",
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            SaveAvr(dialog.FileName);
        }

        private void ConnectReceiver_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == connectReceiver)
            {
                if (connectReceiver.IsChecked)
                {
                    if (string.IsNullOrEmpty(cmbInterfaceClient.Text))
                    {
                        MessageBox.Show("Please enter receiver IP address.");
                    }
                    else
                    {
                        // if there is no Tcp client
                        if (AvrTcp == null)
                        {
                            // create receiver instance
                            Avr ??= new AudysseyMultEQAvr();
                            // create receiver tcp instance
                            AvrTcp = new AudysseyMultEQAvrTcp(Avr, cmbInterfaceClient.Text);
                            // create adapter to interface MultEQAvr properties as if they were MultEQApp properties 
                            AvrAdapter ??= new AudysseyMultEQAvrAdapter(Avr);

                            DataContext = Avr;
                        }
                        AvrTcp.Connect();
                        // attach sniffer
                        if (connectSniffer.IsChecked)
                        {
                            // sniffer must be elevated to capture raw packets
                            if (!IsElevated())
                            {
                                // we cannot create the sniffer...
                                connectSniffer.IsChecked = false;
                                // but we can ask the user to elevate the program!
                                RunAsAdmin();
                            }
                            else
                            {
                                TcpSniffer ??= new AudysseyMultEQTcpSniffer(
                                    Avr,
                                    cmbInterfaceHost.SelectedItem.ToString(),
                                    cmbInterfaceClient.SelectedItem.ToString());
                            }
                        }
                    }
                }
                else
                {
                    AvrAdapter = null;
                    AvrTcp = null;
                    Avr = null;
                    // immediately clean up the object
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    this.DataContext = null;
                }
            }
        }

        private void ConnectSniffer_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == connectSniffer)
            {
                if (connectSniffer.IsChecked)
                {
                    // can only attach sniffer to receiver if receiver object exists 
                    if (Avr == null)
                    {
                        // create receiver instance
                        Avr = new AudysseyMultEQAvr();
                        // create adapter to interface MultEQAvr properties as if they were MultEQApp properties 
                        AvrAdapter = new AudysseyMultEQAvrAdapter(Avr);
                        // data Binding to adapter

                        DataContext = Avr;
                    }
                    // sniffer must be elevated to capture raw packets
                    if (!IsElevated())
                    {
                        // we cannot create the sniffer...
                        connectSniffer.IsChecked = false;
                        // but we can ask the user to elevate the program!
                        RunAsAdmin();
                    }
                    else
                    {
                        // onyl create sniffer if it not already exists
                        TcpSniffer ??= new AudysseyMultEQTcpSniffer(
                            Avr,
                            cmbInterfaceHost.SelectedItem.ToString(),
                            cmbInterfaceClient.SelectedItem.ToString());
                    }
                }
                else
                {
                    if (TcpSniffer != null)
                    {
                        TcpSniffer = null;
                        // if not interested in receiver then close connection and delete objects
                        if (connectReceiver.IsChecked == false)
                        {
                            this.DataContext = null;
                            AvrAdapter = null;
                            Avr = null;
                        }
                        // immediately clean up the object
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
        }

        private void ChannelSetupView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Avr != null)
            {
                if (ChannelSetupView.SelectedItem != null)
                {
                    Avr.SelectedItem = (Dictionary<string, string>)ChannelSetupView.SelectedItem;
                }
            }
        }

        private void ConnectAudyssey_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == connectAudyssey)
            {
                if (connectAudyssey.IsChecked)
                {
                    if (AvrTcp != null)
                    {
                        AvrTcp.EnterAudysseyMode();
                    }
                    else
                    {
                        connectAudyssey.IsChecked = false;
                    }
                }
                else
                {
                    if (AvrTcp != null)
                    {
                        AvrTcp.ExitAudysseyMode();
                    }
                    else
                    {
                        // if we end up here we have a problem
                    }
                }
            }
        }

        private void getReceiverInfo_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.GetAvrInfo())
                {
#if DEBUG
                    var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IInfo))
                    });
                    File.WriteAllText(Environment.CurrentDirectory + "\\AvrInfo.json", json);
#endif
                }
            }
        }

        private void getReceiverStatus_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.GetAvrStatus())
                {
#if DEBUG
                    var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IStatus))
                    });
                    File.WriteAllText(Environment.CurrentDirectory + "\\AvrStatus.json", json);
#endif
                }
            }
        }

        private void setAvrSetAmp_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.SetAvrSetAmp())
                {
#if DEBUG
                    var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAmp))
                    });
                    File.WriteAllText(Environment.CurrentDirectory + "\\AvrSetDataAmp.json", json);
#endif
                }
            }
        }

        private void setAvrSetAudy_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.SetAvrSetAudy())
                {
#if DEBUG
                    var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAudy))
                    });
                    File.WriteAllText(Environment.CurrentDirectory + "\\AvrSetDataAud.json", json);
#endif
                }

            }
        }

        private void setAvrSetDisFil_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.SetAvrSetDisFil())
                {
#if DEBUG
                    var json = JsonConvert.SerializeObject(Avr.DisFil);
                    File.WriteAllText(Environment.CurrentDirectory + "\\AvrDisFil.json", json);
#endif
                }
            }
        }

        private void setAvrInitCoefs_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.SetAvrInitCoefs())
                {
                }
            }
        }

        private void setAvrSetCoefDt_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.SetAvrSetCoefDt())
                {
#if DEBUG
                    var json = JsonConvert.SerializeObject(Avr.CoefData);
                    File.WriteAllText(Environment.CurrentDirectory + "\\AvrCoefDafa.json", json);
#endif
                }
            }
        }

        private void setAudysseyFinishedFlag_Click(object sender, RoutedEventArgs e)
        {
            if ((AvrTcp != null) && (Avr != null))
            {
                if (AvrTcp.SetAudysseyFinishedFlag())
                {
                }
            }
        }
    }
}
