namespace SailMonitor;
using SailMonitor.Models;
using SailMonitor.Services;


public partial class Page1 : ContentView
{

    private Record record = new Record();
    private readonly UdpListenerService _udpService;
    private readonly GPSService _gpsService;
    private readonly NmeaService _nmeaService;
    public CompassDrawable CompassDrawable { get; set; }

    public Page1(UdpListenerService udpService, GPSService gpsService, NmeaService nmeaService)
    {

        InitializeComponent();
        CompassDrawable = new CompassDrawable
        {
            PieAngles = new List<float> { 0f, 60f, 120f }, // example wedges
            PieWidthDegrees = 5f
        };
        GraphicsOverlay.Drawable = CompassDrawable;

        // Redraw when needed
        GraphicsOverlay.Invalidate();

        _udpService = udpService;
        _gpsService = gpsService;
        _nmeaService = nmeaService;

        //WindRoseView.Drawable = _drawable;

        // Subscribe to data events
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

            record = _nmeaService.CalculateWind(record);
            UpdateUI();
        });
    }

    private void UpdateUI()
    {
        
        HeadingLabel.Text = $"{record.headingMag:F2}";
        SpeedWaterLabel.Text = $"{record.SOW:F2}";
        SpeedGroundLabel.Text = $"{record.SOG:F2}";
        DepthLabel.Text = $"{record.depth:F2}";
        AWSLabel.Text = $"{record.windAppSpeed:F2}";
        AWDLabel.Text = $"{record.windAppDir:F2}";
        TWSLabel.Text = $"{record.windTrueSpeed:F2}";
        TWDLabel.Text = $"{record.windTrueDir:F2}";
        CompassDrawable.Heading = (float)record.headingMag;
        CompassDrawable.ApparentWind = (float)record.windAppDir;
        CompassDrawable.TrueWind = (float)record.windTrueDir;
        GraphicsOverlay.Invalidate();
        //_drawable.Direction = (float)record.windTrueDir;
        //WindRoseView.Invalidate();
    }

    public void SetRotation(float degrees)
    {
        CompassDrawable.RotationDegrees = degrees;
        GraphicsOverlay.Invalidate();
    }

    // Optional: update wedges dynamically
  



}