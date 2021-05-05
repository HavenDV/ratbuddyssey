using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using Newtonsoft.Json;
using Audyssey.MultEQAvr;

namespace Ratbuddyssey
{
    public partial class RatbuddysseyHome
    {
        #region Properties

        private static string TcpClientFileName { get; } = "TcpClient.json";

        #endregion

        #region Event Handlers

        public void HandleDroppedFile(object sender, DragEventArgs e)
        {
            FileView.HandleDroppedFile(sender, e);
        }

        public void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            FileView.OpenFile_OnClick(sender, e);
        }

        public void ReloadFile_OnClick(object sender, RoutedEventArgs e)
        {
            FileView.ReloadFile_OnClick(sender, e);
        }

        public void SaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            FileView.SaveFile_OnClick(sender, e);
        }

        public void SaveFileAs_OnClick(object sender, RoutedEventArgs e)
        {
            FileView.SaveFileAs_OnClick(sender, e);
        }

        #endregion

        public RatbuddysseyHome()
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

            for(var x=0; x<61; x++)
            {
                var fcentre = Math.Pow(10.0, 3.0) * Math.Pow(2.0, (x-34.0)/6.0);
                Console.Write(x); Console.Write(" ");
                Console.WriteLine("{0:N1}", fcentre);
            }
        }

        private void ExitProgram_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow?.Close();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shout out to AVS Forum, use at your own risk!");
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

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentTab = ((TabControl)sender).SelectedIndex;

            switch (currentTab)
            {
                case 0:
                    if (FileView.AudysseyApp == null)
                    {
                        if (AvrAdapter != null)
                        {
                            DataContext = AvrAdapter;
                        }
                    }
                    else
                    {
                        DataContext = FileView.AudysseyApp;
                    }
                    break;
                case 1:
                    if (Avr != null)
                    {
                        DataContext = Avr;
                    }
                    break;
            }
        }
    }
}