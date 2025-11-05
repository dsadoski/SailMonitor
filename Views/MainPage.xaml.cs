


using SailMonitor;
using SailMonitor.Models;
using SailMonitor.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SailMonitor
{
    public partial class MainPage : ContentPage
    {

        private readonly UdpListenerService _udpService;
        private readonly GPSService _gpsService;
        private readonly NmeaService _nmeaService;
        public Record record = new Record();


        //public ObservableCollection<ContentView> DisplayedPage { get; set; }
        public List<ContentView> PageViews { get; set; }
        int currentIndex = 0;

        public MainPage(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService)
        {
            try
            {
                InitializeComponent();

                _udpService = udpService;
                _gpsService = gpsService;
                _nmeaService = nmeaService;
                DeviceDisplay.KeepScreenOn = true;



                PageViews = new List<ContentView>
            {
                new Page1(),
                new Page2(),
                new Page3()
            };

                content.Content = PageViews[currentIndex];

                _udpService.OnMessageReceived += HandleUdpMessage;
                _gpsService.OnLocationReceived += HandleGpsLocation;

                _udpService.Start();
                _ = InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainPage constructor: {ex.Message}");

            }
        }

        private async Task InitializeAsync()
        {
            await _gpsService.Start();
        }

        private void HandleUdpMessage(Record n2krecord)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                record = n2krecord.Copy();
                RaiseEventToCurrentView("UDPUpdate", n2krecord);

            });
        }

        private void HandleGpsLocation(Location location)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //record = _udpService.record.Copy();
                _udpService.record.location = new Location(location);
                _udpService.hasLocation = true;


            });
        }
        private void Next_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (currentIndex < PageViews.Count - 1)
                {
                    currentIndex++;
                    content.Content = PageViews[currentIndex];

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Next_Clicked: {ex.Message}");
            }
        }

        private void Prev_Clicked(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                content.Content = PageViews[currentIndex];
            }
        }

        private void RaiseEventToCurrentView(string eventName, Record data)
        {
            if (content.Content is IContentViewHost activeView)
                activeView.OnAppEvent(eventName, data);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Restore normal sleep behavior when leaving
            DeviceDisplay.KeepScreenOn = false;
        }


    }
}
