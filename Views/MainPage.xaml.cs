using SailMonitor.Models;
using SailMonitor.Services;
using System.Diagnostics;

namespace SailMonitor
{
    public partial class MainPage : ContentPage
    {
        public Record record = new Record();
        public Setup _setup;
        public List<DataPointDisplay> dataPointDisplays;
        public List<FieldData> fieldData;
        private readonly UdpListenerService _udpService;
        private readonly GPSService _gpsService;
        private readonly NmeaService _nmeaService;

        private double _panStartX;

        //public ObservableCollection<ContentView> DisplayedPage { get; set; }
        public List<ContentView> PageViews { get; set; }
        private int currentIndex = 1;

        public MainPage(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService, Setup setup)
        {
            try
            {
                InitializeComponent();
                /*if (OperatingSystem.IsAndroid())
                {
                    if (MainLayout.Children.Contains(ButtonHzStack))
                    {
                        MainLayout.Children.Remove(ButtonHzStack);
                    }
                }*/
                HeightRequest = DeviceDisplay.MainDisplayInfo.Height;
                WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
                SizeChanged += OnSizeChanged;

                _udpService = udpService;
                _gpsService = gpsService;
                _nmeaService = nmeaService;
                _setup = setup;
                DeviceDisplay.KeepScreenOn = true;
                fieldData = new List<FieldData>();

                dataPointDisplays = new List<DataPointDisplay>();
                dataPointDisplays.Add(new DataPointDisplay("AWS", "F1", "App Wind Speed"));
                dataPointDisplays.Add(new DataPointDisplay("AWD", "F1", "App Wind Dir"));
                dataPointDisplays.Add(new DataPointDisplay("TWS", "F1", "True Wind Speed"));
                dataPointDisplays.Add(new DataPointDisplay("TWD", "F1", "True Wind Dir"));
                dataPointDisplays.Add(new DataPointDisplay("DPT", "F1", "Depth"));
                dataPointDisplays.Add(new DataPointDisplay("WTC", "F1", "Wind True Compass"));
                dataPointDisplays.Add(new DataPointDisplay("SOG", "F1", "Speed Over Ground"));
                dataPointDisplays.Add(new DataPointDisplay("SOW", "F1", "Speed -> Water"));
                dataPointDisplays.Add(new DataPointDisplay("HDG", "F1", "Heading"));

                PageViews = new List<ContentView>
                {
                    new PageSetup(_setup),
                    new Page1(),
                    /*new Page2(dataPointDisplays),
                    new Page3(),
                    new Page4(),*/
                };

                foreach (var item in dataPointDisplays)
                {
                    PageViews.Add(new SingleDataPoint(item));
                    fieldData.Add(new FieldData(item.name));
                }

                PageViews.Add(new Page3());
                PageViews.Add(new Page4());

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
            _setup = setup;
            this.BackgroundColor = setup.backColor;
            PrevButton.BackgroundColor = Colors.DarkBlue;
            NextButton.BackgroundColor = Colors.DarkBlue;

            if (setup.Night)
            {
                PrevButton.TextColor = setup.foreColor;
                NextButton.TextColor = setup.foreColor;
            }
            else
            {
                PrevButton.TextColor = Colors.White;
                NextButton.TextColor = Colors.White;
            }

            foreach (ContentView view in PageViews)
            {
                SetColorsRecursively(view, setup);
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
                try
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
                    UpdateDataDisplayRecord("WTC", record.windTrueCompass);

                    RaiseEventToCurrentView("UDPUpdate", record);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in Handle UDP: {ex.Message}");
                }
            });
        }

        public void UpdateDataDisplayRecord(string name, double value)
        {
            var view = fieldData.FirstOrDefault(d => d.name == name);
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
                _udpService.Record.location = new Location(location);
                _udpService.HasLocation = true;
            });
        }

        private void NextPage()
        {
            if (currentIndex < PageViews.Count - 1)
            {
                currentIndex++;
                content.Content = PageViews[currentIndex];
                SetColorsRecursively(content.Content, _setup);
            }
        }

        private void PrevPage()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                content.Content = PageViews[currentIndex];
                SetColorsRecursively(content.Content, _setup);
            }
        }

        private void Next_Clicked(object sender, EventArgs e) => NextPage();
        private void Prev_Clicked(object sender, EventArgs e) => PrevPage();

        private void RaiseEventToCurrentView(string eventName, Record data)
        {
            if (content.Content is IContentViewHost activeView)
            {
                activeView.OnAppEvent(eventName, data, fieldData);
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
                    btn.BackgroundColor = Colors.DarkBlue;
                    if (setup.Night)
                    {
                        btn.TextColor = setup.foreColor;
                    }
                    else
                    {
                        btn.TextColor = Colors.White;
                    }

                    break;

                case Entry entry:
                    entry.BackgroundColor = setup.backColor;
                    entry.TextColor = setup.foreColor;
                    break;

                case Editor editor:
                    editor.BackgroundColor = setup.backColor;
                    editor.TextColor = setup.foreColor;
                    break;

                case CheckBox checkBox:
                    checkBox.BackgroundColor = setup.backColor;
                    checkBox.Color = setup.foreColor;
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

            if (view is ContentView contentView && contentView.Content != null)
            {
                SetColorsRecursively(contentView.Content, setup);
            }

            if (view is ScrollView scrollView)
            {
                var content = scrollView.Content;
                SetColorsRecursively(content, setup);
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (content.Content is IContentViewHost activeView)
            {
                content.Content.WidthRequest = Width;
                content.Content.HeightRequest = Height;

                activeView.OnReSize();
            }
        }

        private void Content_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _panStartX = e.TotalX;
                    break;

                case GestureStatus.Running:
                    // Optional: you could move the content visually here for a "dragging" effect
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    double deltaX = e.TotalX - _panStartX;

                    // Detect swipe thresholds
                    if (deltaX < -50) // swipe left → next
                    {
                        NextPage();
                    }
                    else if (deltaX > 50) // swipe right → previous
                    {
                        PrevPage();
                    }

                    break;
            }
        }
    }
}

