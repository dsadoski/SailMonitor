


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
                record.latitude = location.Latitude;
                record.longitude = location.Longitude;
                record.SOG = (location.Speed ?? 0.0) * 1.94384; // m/s → knots
                record.COG = location.Course ?? 0.0;
                record = _nmeaService.CalculateWind(record);
                if (record.location != null)
                {
                    // can we calc COG/SOG from  2 points?
                    TimeSpan timeSpan = new TimeSpan(location.Timestamp.Ticks - record.location.Timestamp.Ticks);
                    // can we calc COG/SOG from  2 points?


                    if (Math.Abs(timeSpan.TotalSeconds) > 5)
                    {
                        double distance = _nmeaService.CalcDistanceNM(record.location, location); // in nautical miles
                        record.SOG = distance / (Math.Abs(timeSpan.TotalSeconds) / 3600.0); // knots
                        double bearing = _nmeaService.CalcBearing(record.location, location);
                        record.headingTrue = bearing;
                        record.COG = bearing;
                        record.location = location;

                    }
                }
                else
                {
                    record.location = new Location(location);
                }
                _udpService.record = record.Copy();

                RaiseEventToCurrentView("GPSUpdate", record);
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
