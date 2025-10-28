
using SailMonitor.Models;
using SailMonitor.Services;


namespace SailMonitor;

public partial class Page2 : ContentView
{

    private Record record = new Record();
    private readonly UdpListenerService _udpService;
    private readonly GPSService _gpsService;
    private readonly NmeaService _nmeaService;
    List<DataPointDisplay> dataPointDisplays =new List<DataPointDisplay>();
    List<GraphicsView> graphicsViews = new List<GraphicsView>();
    public Page2(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService)
    {
		InitializeComponent();
        _udpService = udpService;
        _gpsService = gpsService;
        _nmeaService = nmeaService;


        for (int i = 0; i < 3; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        for (int i = 0; i < 2; i++)
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Redraw when needed
        dataPointDisplays.Add(new DataPointDisplay("AWS", "F1"));
        dataPointDisplays.Add(new DataPointDisplay("AWD", "F1"));

        foreach(var display in dataPointDisplays)
        {
            var view = new GraphicsView
            {
                Drawable = display,
                HeightRequest = 200,
                WidthRequest = 200
            };
            graphicsViews.Add(view);
            MainGrid.Add(view);
        }

         




        _udpService.OnMessageReceived += HandleUdpMessage;
        _gpsService.OnLocationReceived += HandleGpsLocation;

        _udpService.Start();
        _ = InitializeAsync();

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
            UpdateUI();
        });
    }

    private void HandleGpsLocation(Location location)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {


            record.latitude = location.Latitude;
            record.longitude = location.Longitude;
            record.SOG = (location.Speed ?? 0.0) * 1.94384; // m/s → knots
            record.COG = location.Course ?? 0.0;


            // can we calc COG/SOG from  2 points?
            TimeSpan timeSpan = new TimeSpan(location.Timestamp.Ticks - record.location.Timestamp.Ticks);
            // can we calc COG/SOG from  2 points?


            if (timeSpan.TotalSeconds > 5)
            {
                double distance = _nmeaService.CalcDistanceNM(record.location, location); // in nautical miles
                record.SOG = distance / (timeSpan.TotalSeconds / 3600.0); // knots
                double bearing = _nmeaService.CalcBearing(record.location, location);
                record.headingTrue = bearing;
                record.COG = bearing;
                record.location = location;
            }

            record = _nmeaService.CalculateWind(record);
            UpdateUI();
        });
    }

    private void UpdateUI()
    {
        UpdateRecord("AWS", record.windAppSpeed);
        UpdateRecord("AWD", record.windAppDir);

        foreach (var view in graphicsViews)
        {
            view.Invalidate();

        }
    }

    public void UpdateRecord(string name, double value)
    {
        var view = dataPointDisplays.FirstOrDefault(d => d.name == name);
        if (view != null)
        {
            view.AddDataPoint(value);
        }
    }
    public void Dispose()
    {
        _udpService.OnMessageReceived -= HandleUdpMessage;
        _gpsService.OnLocationReceived -= HandleGpsLocation;
    }
}