


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
        

        public ObservableCollection<ContentView> DisplayedPage { get; set; }
        public List<ContentView> PageViews { get; set; }
        int currentIndex = 0;

        public MainPage(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService)
        {
            InitializeComponent();

            _udpService = udpService;
            _gpsService = gpsService;
            _nmeaService = nmeaService;

            DisplayedPage = new ObservableCollection<ContentView>();

            PageViews =new List<ContentView>
            {
                new Page1(_udpService, _gpsService, _nmeaService),
                new Page2(_udpService, _gpsService, _nmeaService),
                new Page3()
            };
            DisplayedPage.Add(PageViews[0]);

            BindingContext = this;



        }
        private void Next_Clicked(object sender, EventArgs e)
        {
            if (currentIndex < PageViews.Count - 1)
            {
                currentIndex++;
                DisplayedPage.Clear();
                DisplayedPage.Add(PageViews[currentIndex]);

            }
        }

        private void Prev_Clicked(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                DisplayedPage.Clear();
                DisplayedPage.Add(PageViews[currentIndex]);

            }
        }

    }
}
