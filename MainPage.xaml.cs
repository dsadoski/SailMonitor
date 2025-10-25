


using SailMonitor.Models;
using SailMonitor.Services;
using SailMonitor.Views;

namespace SailMonitor
{
    public partial class MainPage : ContentPage
    {

        private readonly UdpListenerService _udpService;
        private readonly GPSService _gpsService;
        private readonly NmeaService _nmeaService;

        public MainPage(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService)
        {
            InitializeComponent();

            _udpService = udpService;
            _gpsService = gpsService;
            _nmeaService = nmeaService;
            SetupCarousel();
        }


        private void SetupCarousel()
        {
            // Programmatically add pages


            // Connect IndicatorView
            PagerView.IndicatorView = PagerIndicator;

            // Ensure first page shows after layout
            PagerView.Loaded += (s, e) =>
            {
                PagerView.ItemsSource = new ContentView[]
                {
                new Page1(_udpService, _gpsService, _nmeaService),
                new Page2()

                };
                PagerView.Position = 0;
            };
        }

        private void PreviousPage_Clicked(object sender, EventArgs e)
        {
            if (PagerView.Position > 0)
                PagerView.Position -= 1;
        }

        private void NextPage_Clicked(object sender, EventArgs e)
        {
            if (PagerView.Position < PagerView.ItemsSource.Cast<object>().Count() - 1)
                PagerView.Position += 1;
        }
    }
}
