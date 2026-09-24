namespace SailMonitor.Services
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using SailMonitor.Models;

    public class UdpListenerService
    {
        private readonly object syncRoot = new();
        private UdpClient? udpClient;
        private CancellationTokenSource? cts;
        private Task? receiveTask;
        private bool isInitialized;
        private readonly NmeaService nmeaService;

        public event Action<Record>? OnMessageReceived;

        public Setup setup;
        public Record Record;
        public bool HasLocation;

        public UdpListenerService(Setup setup, NmeaService nmeaService)
        {
            this.setup = setup;
            this.nmeaService = nmeaService;
            Record = new Record();
        }

        public void Start()
        {
            lock (syncRoot)
            {
                if (isInitialized)
                {
                    return;
                }

                CleanupResources();

                try
                {
                    cts = new CancellationTokenSource();
                    udpClient = CreateUdpClient(setup.Port);
                    Record = new Record();
                    HasLocation = false;
                    isInitialized = true;
                    receiveTask = ReceiveLoopAsync(udpClient, cts.Token);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Socket bind failed: {ex.Message}");
                    CleanupResources();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UDP Listener Initialization Error: {ex.Message}");
                    CleanupResources();
                }
            }
        }

        private static UdpClient CreateUdpClient(int port)
        {
            if (!OperatingSystem.IsAndroid())
            {
                return new UdpClient(port);
            }

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                return new UdpClient { Client = socket };
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        private async Task ReceiveLoopAsync(UdpClient client, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await client.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    Record snapshot;

                    lock (syncRoot)
                    {
                        Record = nmeaService.ParseSentence(message, Record);
                        if (HasLocation)
                        {
                            ParseLocationCore();
                        }

                        snapshot = Record.Copy();
                    }

                    OnMessageReceived?.Invoke(snapshot);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UDP Listener Error: {ex.Message}");
            }
            finally
            {
                lock (syncRoot)
                {
                    if (ReferenceEquals(udpClient, client))
                    {
                        isInitialized = false;
                    }
                }
            }
        }

        public void SetLocation(Location location)
        {
            lock (syncRoot)
            {
                Record.location = new Location(location);
                HasLocation = true;
            }
        }

        public void ParseLocation()
        {
            lock (syncRoot)
            {
                ParseLocationCore();
            }
        }

        private void ParseLocationCore()
        {
            if (Record.location == null)
            {
                Record.location = new Location();
                HasLocation = false;
                return;
            }

            var timeSpan = Record.gpsTicks == 0
                ? TimeSpan.Zero
                : TimeSpan.FromTicks(Record.location.Timestamp.Ticks - Record.gpsTicks);

            Record.SOG = (Record.location.Speed ?? 0.0) * 1.94384;
            Record.COG = Record.location.Course ?? 0.0;

            if (Record.gpsTicks != 0 && Math.Abs(timeSpan.TotalSeconds) > setup.saveFrequency)
            {
                double distance = nmeaService.CalcDistanceNM(Record);
                if (distance > 0)
                {
                    Record.SOG = distance / (Math.Abs(timeSpan.TotalSeconds) / 3600.0);
                    double bearing = nmeaService.CalcBearing(Record);
                    Record.headingTrue = bearing;
                    Record.COG = bearing;
                }
            }

            Record.latitude = Record.location.Latitude;
            Record.longitude = Record.location.Longitude;
            Record.gpsTicks = Record.location.Timestamp.Ticks;
            Record = nmeaService.CalculateWind(Record);
            HasLocation = false;
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                isInitialized = false;
                CleanupResources();
                receiveTask = null;
            }
        }

        private void CleanupResources()
        {
            cts?.Cancel();
            udpClient?.Dispose();
            cts?.Dispose();
            udpClient = null;
            cts = null;
        }
    }
}
