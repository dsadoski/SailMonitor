﻿
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
            _cts = new CancellationTokenSource();
            _udpClient = new UdpClient(_port);
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
        }
    }
}