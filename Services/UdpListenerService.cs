
namespace SailMonitor.Services
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using SailMonitor.Models;

    public class UdpListenerService
    {
        private readonly int port;
        private UdpClient? udpClient;
        private CancellationTokenSource? cts;

        public event Action<Record>? OnMessageReceived;

        public Setup setup;
        public Record Record;

        public bool HasLocation = false;

        private bool isInitialized = false;
        private NmeaService nmeaService;

        public UdpListenerService(Setup setup, NmeaService nmeaService)
        {
            this.setup = setup;
            port = this.setup.Port;
            Record = new Record();
            this.setup = setup;
            this.nmeaService = nmeaService;
        }

        public void Start()
        {
            if (isInitialized == true)
            {
                return;
            }

            isInitialized = true;

            if (OperatingSystem.IsAndroid())
            {
                try
                {
                    // Clean up if called twice or after a crash/reload
                    udpClient?.Close();
                    udpClient?.Dispose();
                    udpClient = null;
                }
                catch
                {
                }
            }

            try
            {
                cts = new CancellationTokenSource();
                if (OperatingSystem.IsAndroid())
                {
                    var endpoint = new IPEndPoint(IPAddress.Any, port);
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    // Allow immediate rebinding even if the OS still thinks it’s in use
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    udpClient = new UdpClient();
                    udpClient.Client = socket;
                    udpClient.Client.Bind(endpoint);
                }
                else
                {
                    udpClient = new UdpClient(port);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket bind failed: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UDP Listener Initialization Error: {ex.Message}");
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        var result = await udpClient.ReceiveAsync();
                        var message = Encoding.UTF8.GetString(result.Buffer);

                        Record = nmeaService.ParseSentence(message, Record);
                        if (HasLocation == true)
                        {
                            ParseLocation();
                        }

                        OnMessageReceived?.Invoke(Record);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Normal when stopping the listener
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UDP Listener Error: {ex.Message}");
                }
            });
        }

        public void ParseLocation()
        {
            Record.latitude = Record.location.Latitude;
            Record.longitude = Record.location.Longitude;
            Record.SOG = (Record.location.Speed ?? 0.0) * 1.94384; // m/s → knots
            Record.COG = Record.location.Course ?? 0.0;
            Record = nmeaService.CalculateWind(Record);
            if (Record.location != null)
            {
                // can we calc COG/SOG from  2 points?
                TimeSpan timeSpan = new TimeSpan(Record.location.Timestamp.Ticks - Record.gpsTicks);

                // can we calc COG/SOG from  2 points?
                if (Math.Abs(timeSpan.TotalSeconds) > setup.saveFrequency)
                {
                    double distance = nmeaService.CalcDistanceNM(Record); // in nautical miles
                    if (distance > 0)
                    {
                        Record.SOG = distance / (Math.Abs(timeSpan.TotalSeconds) / 3600.0); // knots
                        double bearing = nmeaService.CalcBearing(Record);
                        Record.headingTrue = bearing;
                        Record.COG = bearing;
                        Record.latitude = Record.location.Latitude;
                        Record.longitude = Record.location.Longitude;
                        Record.gpsTicks = Record.location.Timestamp.Ticks;
                    }
                }
            }
            else
            {
                Record.location = new Location(Record.location);
            }

            HasLocation = false;
        }

        public void Stop()
        {
            cts?.Cancel();
            udpClient?.Close();
            udpClient?.Dispose();
        }
    }
}