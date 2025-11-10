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
        WidthRequest = 1000;
        HeightRequest = 1000;
        if(Width>Height)
        { 
            GraphicsOverlay.WidthRequest = Width*.4;
        }
        else
        {
            GraphicsOverlay.HeightRequest = Height * .4;
        }
        OnReSize();
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
        AWDLabel.Text = $"{record.windAppDir:F2}°";
        TWSLabel.Text = $"{record.windTrueSpeed:F2}";
        TWDLabel.Text = $"{record.windTrueDir:F2}°";
        CompassDrawable.Heading = (float)record.headingMag;
        CompassDrawable.ApparentWind = (float)record.windAppDir;
        CompassDrawable.TrueWind = (float)record.windTrueDir;
       
        GraphicsOverlay.Invalidate();
    }

    public void SetRotation(float degrees)
    {
        CompassDrawable.RotationDegrees = degrees;
        GraphicsOverlay.Invalidate();
    }

    // Optional: update wedges dynamically

    public void OnAppEvent(string eventName, Record data, List< FieldData> DataPoints)
    {
        record = data.Copy();
        OnReSize();
        UpdateUI();
        if (eventName == "RefreshData")
        {
            // handle event
            //RefreshData((MyDataModel)data!);
        }
    }

    public void OnReSize()
    {
        if (Width <= 0 || Height <= 0)
            return;

        bool isLandscape = Width > Height;
        AdjustLayout(isLandscape);
    }

    private void AdjustLayout(bool isLandscape)
    {
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();

        if (isLandscape)
        {
            // Landscape: side-by-side
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Place graphics on left, numbers on right
            Grid.SetRow(GraphicsOverlay, 0);
            Grid.SetColumn(GraphicsOverlay, 0);
            Grid.SetRow(InfoPanel, 0);
            Grid.SetColumn(InfoPanel, 1);

            // Make the graphics view square
            GraphicsOverlay.WidthRequest = Width * .4;
            GraphicsOverlay.HeightRequest = Height * .9;
            
        }
        else
        {
            // Portrait: stacked vertically
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            // Place numbers on top, graphics below
            Grid.SetRow(InfoPanel, 0);
            Grid.SetColumn(InfoPanel, 0);
            Grid.SetRow(GraphicsOverlay, 1);
            Grid.SetColumn(GraphicsOverlay, 0);
            GraphicsOverlay.WidthRequest = Width * .9;
            GraphicsOverlay.HeightRequest = Height * .4;
        }
        
        
    }
}
