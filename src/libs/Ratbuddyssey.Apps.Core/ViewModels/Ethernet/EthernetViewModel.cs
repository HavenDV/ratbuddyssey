using System.ComponentModel;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Ratbuddyssey.MultEQAvr;
using Ratbuddyssey.MultEQAvrAdapter;
using Ratbuddyssey.MultEQTcp;

namespace Ratbuddyssey.ViewModels;

public class EthernetViewModel : RoutableViewModel
{
    #region Properties

    [Reactive]
    public AudysseyMultEQAvr Avr { get; set; } = new();

    [Reactive]
    public AudysseyMultEQAvrAdapter AvrAdapter { get; set; }

    private AudysseyMultEQAvrTcp? AvrTcp { get; set; }
    private AudysseyMultEQTcpSniffer? TcpSniffer { get; set; }

    private static string TcpClientFileName { get; } = "TcpClient.json";

    public IReadOnlyCollection<string> Ips { get; }

    [Reactive]
    public string? SelectedIp { get; set; }

    public IReadOnlyCollection<string> InterfaceIps { get; } = Array.Empty<string>();

    [Reactive]
    public string? SelectedInterfaceIp { get; set; }

    [Reactive]
    public bool ConnectReceiverIsChecked { get; set; }

    [Reactive]
    public bool ConnectSnifferIsChecked { get; set; }

    [Reactive]
    public bool ConnectAudysseyIsChecked { get; set; }

    [Reactive]
    public Dictionary<string, string>? SelectedChannelSetupView { get; set; }

    public IReadOnlyCollection<string> ChannelSetupList { get; } = new[] { "N", "S", "E" };

    public IReadOnlyCollection<string> AudyDynSetList { get; } = new[] { "H", "M", "L" };

    public IReadOnlyCollection<string> AudyEqSetList { get; } = new[] { "Audy", "Flat" };

    public IReadOnlyCollection<int> AudyEqRefList { get; } = new[] { 0, 5, 10, 15 };

    public IReadOnlyCollection<int> AudyLfcLevList { get; } = new[] { 1, 2, 3, 4, 5, 6, 7 };

    public IReadOnlyCollection<decimal> SelectedChLevelList { get; } = new[] {
            -12m, -11.5m, -11m, -10.5m, -10m, -9.5m, -9m, -8.5m, -8m, -7.5m, -7m, -6.5m,
            -6m, -5.5m, -5m, -4.5m, -4m, -3.5m, -3m, -2.5m, -2m, -1.5m, -1m, -0.5m,
            0m, 0.5m, 1.0m, 1.5m, 2.0m, 2.5m, 3m, 3.5m, 4m, 4.5m, 5m, 5.5m,
            6m, 6.5m, 7m, 7.5m, 9m, 8.5m, 9m, 9.5m, 10m, 10.5m, 11m, 11.5m, 12m,
        };

    public IReadOnlyCollection<string> AudyFinFlgList { get; } = new[] { "Fin", "NotFin" };

    #region Commands

    public ReactiveCommand<Unit, Unit> OpenFile { get; }
    public ReactiveCommand<Unit, Unit> SaveFile { get; }
    public ReactiveCommand<Unit, Unit> GetReceiverInfo { get; }
    public ReactiveCommand<Unit, Unit> GetReceiverStatus { get; }
    public ReactiveCommand<Unit, Unit> SetAvrSetAmp { get; }
    public ReactiveCommand<Unit, Unit> SetAvrSetAudy { get; }
    public ReactiveCommand<Unit, Unit> SetAvrSetDisFil { get; }
    public ReactiveCommand<Unit, Unit> SetAvrInitCoefs { get; }
    public ReactiveCommand<Unit, Unit> SetAvrSetCoefDt { get; }
    public ReactiveCommand<Unit, Unit> SetAudysseyFinishedFlag { get; }

    #endregion

    #endregion

    #region Constructors

    public EthernetViewModel(IScreen hostScreen) : base(hostScreen, "Ethernet")
    {
        AvrAdapter = new AudysseyMultEQAvrAdapter(Avr);

        OpenFile = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            var data = await FileInteractions.OpenFile.Handle(
                new OpenFileArguments(
                    "AudysseySniffer.aud",
                    new[] { ".aud" },
                    "Audyssey sniffer"));
            if (data == null)
            {
                return;
            }

            LoadAvr(data.Value.GetString());
        });
        SaveFile = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            _ = await FileInteractions.SaveFile.Handle(
                new SaveFileArguments(
                    "AudysseySniffer.aud",
                    ".aud",
                    "Audyssey sniffer",
                    () => Task.FromResult(Encoding.UTF8.GetBytes(SaveAvr()))));
        });
        GetReceiverInfo = ReactiveCommand.Create(() =>
        {
            if (AvrTcp == null)
            {
                return;
            }
            if (!AvrTcp.GetAvrInfo())
            {
                return;
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IInfo))
            });
            File.WriteAllText(Environment.CurrentDirectory + "\\AvrInfo.json", json);
#endif
        });
        GetReceiverStatus = ReactiveCommand.Create(() =>
        {
            if (AvrTcp == null)
            {
                return;
            }
            if (!AvrTcp.GetAvrStatus())
            {
                return;
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IStatus))
            });
            File.WriteAllText(Environment.CurrentDirectory + "\\AvrStatus.json", json);
#endif
        });
        SetAvrSetAmp = ReactiveCommand.Create(() =>
        {
            if (AvrTcp == null)
            {
                return;
            }
            if (!AvrTcp.SetAvrSetAmp())
            {
                return;
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IAmp))
            });
            File.WriteAllText(Environment.CurrentDirectory + "\\AvrSetDataAmp.json", json);
#endif
        });
        SetAvrSetAudy = ReactiveCommand.Create(() =>
        {
            if (AvrTcp == null)
            {
                return;
            }
            if (!AvrTcp.SetAvrSetAudy())
            {
                return;
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
            {
                ContractResolver = new InterfaceContractResolver(typeof(IAudy))
            });
            File.WriteAllText(Environment.CurrentDirectory + "\\AvrSetDataAud.json", json);
#endif
        });
        SetAvrSetDisFil = ReactiveCommand.Create(() =>
        {
            if (AvrTcp == null)
            {
                return;
            }
            if (!AvrTcp.SetAvrSetDisFil())
            {
                return;
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(Avr.DisFil);
            File.WriteAllText(Environment.CurrentDirectory + "\\AvrDisFil.json", json);
#endif
        });
        SetAvrInitCoefs = ReactiveCommand.Create(() =>
        {
            if (AvrTcp != null)
            {
                AvrTcp.SetAvrInitCoefs();
            }
        });
        SetAvrSetCoefDt = ReactiveCommand.Create(() =>
        {
            if (AvrTcp == null)
            {
                return;
            }
            if (!AvrTcp.SetAvrSetCoefDt())
            {
                return;
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(Avr.CoefData);
            File.WriteAllText(Environment.CurrentDirectory + "\\AvrCoefDafa.json", json);
#endif
        });
        SetAudysseyFinishedFlag = ReactiveCommand.Create(() =>
        {
            AvrTcp?.SetAudysseyFinishedFlag();
        });

        this.WhenAnyValue(static x => x.ConnectReceiverIsChecked)
            .Subscribe(value =>
            {
                if (value)
                {
                    if (string.IsNullOrEmpty(SelectedInterfaceIp))
                    {
                        MessageInteractions.Warning.Handle("Please enter receiver IP address.").Subscribe();
                    }
                    else
                    {
                        // if there is no Tcp client
                        if (AvrTcp == null)
                        {
                            // create receiver tcp instance
                            AvrTcp = new AudysseyMultEQAvrTcp(Avr, SelectedInterfaceIp);
                        }
                        AvrTcp.Connect();
                        // attach sniffer
                        if (ConnectSnifferIsChecked)
                        {
                            // sniffer must be elevated to capture raw packets
                            if (!IsElevated())
                            {
                                // we cannot create the sniffer...
                                ConnectSnifferIsChecked = false;
                                // but we can ask the user to elevate the program!
                                RunAsAdmin();
                            }
                            else
                            {
                                TcpSniffer ??= new AudysseyMultEQTcpSniffer(
                                    Avr,
                                    SelectedIp,
                                    SelectedInterfaceIp);
                            }
                        }
                    }
                }
                else
                {
                    Avr = new AudysseyMultEQAvr();
                    AvrAdapter = new AudysseyMultEQAvrAdapter(Avr);
                    AvrTcp = null;
                    // immediately clean up the object
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            });

        this.WhenAnyValue(static x => x.ConnectSnifferIsChecked)
            .Subscribe(value =>
            {
                if (value)
                {
                    // sniffer must be elevated to capture raw packets
                    if (!IsElevated())
                    {
                        // we cannot create the sniffer...
                        ConnectSnifferIsChecked = false;
                        // but we can ask the user to elevate the program!
                        RunAsAdmin();
                    }
                    else
                    {
                        // onyl create sniffer if it not already exists
                        TcpSniffer ??= new AudysseyMultEQTcpSniffer(
                        Avr,
                        SelectedIp,
                        SelectedInterfaceIp);
                    }
                }
                else
                {
                    if (TcpSniffer != null)
                    {
                        TcpSniffer = null;
                        // if not interested in receiver then close connection and delete objects
                        if (ConnectReceiverIsChecked == false)
                        {
                            Avr = new AudysseyMultEQAvr();
                            AvrAdapter = new AudysseyMultEQAvrAdapter(Avr);
                        }
                        // immediately clean up the object
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            });

        this.WhenAnyValue(static x => x.ConnectAudysseyIsChecked)
            .Subscribe(value =>
            {
                if (value)
                {
                    if (AvrTcp != null)
                    {
                        AvrTcp.EnterAudysseyMode();
                    }
                    else
                    {
                        ConnectAudysseyIsChecked = false;
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
            });

        this.WhenAnyValue(static x => x.SelectedChannelSetupView)
            .WhereNotNull()
            .Subscribe(value =>
            {
                Avr.SelectedItem = value;
            });

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

    #region Methods

    public void LoadAvr(string json)
    {
        Avr = JsonConvert.DeserializeObject<AudysseyMultEQAvr>(json) ??
              throw new InvalidOperationException("Invalid json.");
        AvrAdapter = new AudysseyMultEQAvrAdapter(Avr);
    }

    public string SaveAvr()
    {
        return JsonConvert.SerializeObject(Avr, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
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
                MessageInteractions.Warning.Handle("Sniffer needs elevated rights for raw socket!").Subscribe();
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
}
