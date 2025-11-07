


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
        private Setup _setup;
        public List<DataPointDisplay> dataPointDisplays;
        


        //public ObservableCollection<ContentView> DisplayedPage { get; set; }
        public List<ContentView> PageViews { get; set; }
        int currentIndex = 1;

        public MainPage(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService, Setup setup)
        {
            try
            {
                InitializeComponent();

                _udpService = udpService;
                _gpsService = gpsService;
                _nmeaService = nmeaService;
                _setup = setup;
                DeviceDisplay.KeepScreenOn = true;

                dataPointDisplays = new List<DataPointDisplay>();
                dataPointDisplays.Add(new DataPointDisplay("AWS", "F1", "App Wind Speed"));
                dataPointDisplays.Add(new DataPointDisplay("AWD", "F1", "App Wind Dir"));
                dataPointDisplays.Add(new DataPointDisplay("TWS", "F1", "True Wind Speed"));
                dataPointDisplays.Add(new DataPointDisplay("TWD", "F1", "True Wind Dir"));
                dataPointDisplays.Add(new DataPointDisplay("DPT", "F1", "Depth"));
                dataPointDisplays.Add(new DataPointDisplay("SOG", "F1", "Speed Over Ground"));
                dataPointDisplays.Add(new DataPointDisplay("SOW", "F1", "Speed -> Water"));
                dataPointDisplays.Add(new DataPointDisplay("HDG", "F1", "Heading"));

                PageViews = new List<ContentView>
                {
                    new PageSetup(),
                    new Page1(),
                    new Page2(dataPointDisplays),
                    new Page3(),
                    new Page4(),
                };

                foreach(var item in dataPointDisplays)
                {
                    PageViews.Add(new SingleDataPoint(item));
                }

                SetColorScheme(_setup);
               

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

        public void SetColorScheme(Setup setup)
        {
            this.BackgroundColor = setup.backColor;
            foreach (ContentView view in PageViews)
            {
                SetColorsRecursively(view,setup);
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
                UpdateDataDisplayRecord("AWS", record.windAppSpeed);
                UpdateDataDisplayRecord("AWD", record.windAppDir);
                UpdateDataDisplayRecord("TWS", record.windTrueSpeed);
                UpdateDataDisplayRecord("TWD", record.windTrueDir);
                UpdateDataDisplayRecord("DPT", record.depth);
                UpdateDataDisplayRecord("SOG", record.SOG);
                UpdateDataDisplayRecord("SOW", record.SOW);
                UpdateDataDisplayRecord("HDG", record.headingMag);

                RaiseEventToCurrentView("UDPUpdate", record);

            });
        }

        public void UpdateDataDisplayRecord(string name, double value)
        {
            var view = dataPointDisplays.FirstOrDefault(d => d.name == name);
            if (view != null)
            {
                view.AddDataPoint(value);
            }
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
            {
                activeView.OnAppEvent(eventName, data, dataPointDisplays);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Restore normal sleep behavior when leaving
            DeviceDisplay.KeepScreenOn = false;
        }

        public void SetColorsRecursively(IView view, Setup setup)
        {
            // Set background/foreground based on control type
            switch (view)
            {
                case Label lbl:
                    lbl.BackgroundColor = setup.backColor;
                    lbl.TextColor = setup.foreColor;
                    break;

                case Button btn:
                    btn.BackgroundColor = setup.backColor;
                    btn.TextColor = setup.foreColor;
                    break;

                case Entry entry:
                    entry.BackgroundColor = setup.backColor;
                    entry.TextColor = setup.foreColor;
                    break;

                case Editor editor:
                    editor.BackgroundColor = setup.backColor;
                    editor.TextColor = setup.foreColor;
                    break;

                case Grid grid:
                    grid.BackgroundColor = setup.backColor;
                    break;

                    case Microsoft.Maui.Controls.Switch swtch:
                        swtch.BackgroundColor = setup.backColor;
                        break;
            }

            // Now recurse if it’s a layout or content view
            if (view is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    SetColorsRecursively(child, setup);
                }
            }
            else if (view is ContentView contentView && contentView.Content != null)
            {
                SetColorsRecursively(contentView.Content, setup);
            }
        }

    }
}

