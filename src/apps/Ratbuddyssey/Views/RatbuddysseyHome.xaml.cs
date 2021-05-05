using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Audyssey;
using Audyssey.MultEQApp;
using Audyssey.MultEQAvr;

namespace Ratbuddyssey
{
    public partial class RatbuddysseyHome
    {
        #region Properties

        private AudysseyMultEQReferenceCurveFilter ReferenceCurveFilter { get; } = new();
        private AudysseyMultEQApp AudysseyApp { get; set; }

        private static string TcpClientFileName { get; } = "TcpClient.json";

        #endregion

        public RatbuddysseyHome()
        {
            InitializeComponent();
            channelsView.SelectionChanged += ChannelsView_SelectionChanged;
            plot.PreviewMouseWheel += Plot_PreviewMouseWheel;

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

        private void HandleDroppedFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                if (files != null && files.Length > 0)
                    OpenFile(files[0]);
            }
        }

        private void LoadApp(string fileName)
        {
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                AudysseyApp = JsonConvert.DeserializeObject<AudysseyMultEQApp>(json, new JsonSerializerSettings
                {
                    FloatParseHandling = FloatParseHandling.Decimal
                });
            }
        }

        private void SaveApp(string fileName)
        {
            if(AudysseyApp != null)
            {
                var json = JsonConvert.SerializeObject(AudysseyApp, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                if ((!string.IsNullOrEmpty(fileName)))
                {
                    File.WriteAllText(fileName, json);
                }
            }
        }

        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".ady", 
                Filter = "Audyssey files (*.ady)|*.ady",
            };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                OpenFile(dlg.FileName);
            }
        }

        private void ReloadFile_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("This will reload the .ady file and discard all changes since last save", "Are you sure?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (File.Exists(currentFile.Content.ToString()))
                {
                    LoadApp(currentFile.Content.ToString());
                    if ((AudysseyApp != null) && (tabControl.SelectedIndex == 0))
                    {
                        DataContext = AudysseyApp;
                    }
                }
            }
        }

        private void SaveFile_OnClick(object sender, RoutedEventArgs e)
        {
#if DEBUG
            currentFile.Content = Path.ChangeExtension(currentFile.Content.ToString(), ".json");
#endif
            SaveApp(currentFile.Content.ToString());
        }

        private void SaveFileAs_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = currentFile.Content.ToString(),
                DefaultExt = ".ady",
                Filter = "Audyssey calibration (.ady)|*.ady"
            };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                currentFile.Content = dlg.FileName;
                SaveApp(currentFile.Content.ToString());
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
                    if (AudysseyApp == null)
                    {
                        if (AvrAdapter != null)
                        {
                            DataContext = AvrAdapter;
                        }
                    }
                    else
                    {
                        DataContext = AudysseyApp;
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

        private void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            currentFile.Content = filePath;
            LoadApp(currentFile.Content.ToString());
            if ((AudysseyApp != null) && (tabControl.SelectedIndex == 0))
            {
                DataContext = AudysseyApp;
            }
        }
    }
}