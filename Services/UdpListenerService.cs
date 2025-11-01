

using SailMonitor.Models;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SailMonitor.Services
{
    public class UdpListenerService
    {
        private readonly int _port;
        private UdpClient? _udpClient;
        private CancellationTokenSource? _cts;

        public event Action<Record>? OnMessageReceived;
        Setup  setup = new Setup();
        public Record record;
        
        public bool hasLocation = false;

        bool isInitialized = false;
        private NmeaService nmeaService;
        public UdpListenerService(int port)
        {
            _port = port;
            record = new Record();
            setup = new Setup();
            nmeaService = new NmeaService(setup);
        }

        public void Start()
        {
            if(isInitialized ==true)
            {
                return;
            }
            isInitialized = true;

            if (OperatingSystem.IsAndroid())
            {
                try
                {

                    // Clean up if called twice or after a crash/reload
                    _udpClient?.Close();
                    _udpClient?.Dispose();
                    _udpClient = null;
                }
                catch { }
            }


            try
            {
                _cts = new CancellationTokenSource();
                if (OperatingSystem.IsAndroid())
                {
                    var endpoint = new IPEndPoint(IPAddress.Any, _port);
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    // Allow immediate rebinding even if the OS still thinks it’s in use
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    
                    _udpClient = new UdpClient();
                    _udpClient.Client = socket;
                    _udpClient.Client.Bind(endpoint);
                }
                else
                {
                    
                    _udpClient = new UdpClient(_port);
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
            


            Task.Run(async () =>
            {
                try
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        var result = await _udpClient.ReceiveAsync();
                        var message = Encoding.UTF8.GetString(result.Buffer);
                        /*NMEA2000 nMEA2000Message = new NMEA2000(message);
                        var record = n2KService.N2KParse(nMEA2000Message.PGN, nMEA2000Message.byteArray);*/
                        record = nmeaService.ParseSentence(message, record);
                        if (hasLocation == true)
                        {
                            ParseLocation();
                        }
                        OnMessageReceived?.Invoke(record);
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

            record.latitude = record.location.Latitude;
            record.longitude = record.location.Longitude;
            record.SOG = (record.location.Speed ?? 0.0) * 1.94384; // m/s → knots
            record.COG = record.location.Course ?? 0.0;
            record = nmeaService.CalculateWind(record);
            if (record.location != null)
            {
                // can we calc COG/SOG from  2 points?
                TimeSpan timeSpan = new TimeSpan(record.location.Timestamp.Ticks - record.location.Timestamp.Ticks);
                // can we calc COG/SOG from  2 points?


                if (Math.Abs(timeSpan.TotalSeconds) > 5)
                {
                    double distance = nmeaService.CalcDistanceNM(record.location, record.location); // in nautical miles
                    record.SOG = distance / (Math.Abs(timeSpan.TotalSeconds) / 3600.0); // knots
                    double bearing = nmeaService.CalcBearing(record.location, record.location);
                    record.headingTrue = bearing;
                    record.COG = bearing;
                }
            }
            else
            {
                record.location = new Location(record.location);
            }
            

            hasLocation = false;
        }

        public void Stop()
        {
            _cts?.Cancel();
            _udpClient?.Close();
            _udpClient?.Dispose();
        }
    }
}