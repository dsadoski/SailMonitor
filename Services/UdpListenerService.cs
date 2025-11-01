
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
        bool isInitialized = false;

        public UdpListenerService(int port)
        {
            _port = port;
            record = new Record();
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
            NmeaService nmeaService = new NmeaService(setup);


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
                        var record = nmeaService.ParseSentence(message);

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

        public void Stop()
        {
            _cts?.Cancel();
            _udpClient?.Close();
            _udpClient?.Dispose();
        }
    }
}