namespace SailMonitor;
using SailMonitor.Models;
using SailMonitor.Services;


public partial class Page1 : ContentView, IContentViewHost
{

    private Record record = new Record();
  
    public CompassDrawable CompassDrawable { get; set; }

    public Page1()
    {

        InitializeComponent();
        //this.BackgroundColor = Colors.White;
        CompassDrawable = new CompassDrawable();
        GraphicsOverlay.Drawable = CompassDrawable;

        // Redraw when needed
        GraphicsOverlay.Invalidate();


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

    public void OnAppEvent(string eventName, Record data, List<DataPointDisplay> dataPointDisplays)
    {
        record = data.Copy();
        UpdateUI();
        if (eventName == "RefreshData")
        {
            // handle event
            //RefreshData((MyDataModel)data!);
        }
    }




}