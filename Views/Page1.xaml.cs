namespace SailMonitor;

using Microsoft.Maui.Animations;
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
        SizeChanged += Page1_SizeChanged;
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

    private void Page1_SizeChanged(object sender, EventArgs e)
    {
        if (Width <= 0 || Height <= 0)
            return;

        // Delay one tick so that InfoPanel has real width/height
        MainThread.BeginInvokeOnMainThread(() =>
        {
            bool isLandscape = Width > Height;

            AdjustLayout(isLandscape);
            ResizeCompass(isLandscape);
            ResizeFonts();

            GraphicsOverlay.Invalidate();
        });
    }

    private void ResizeCompass(bool isLandscape)
    {
        double availableWidth = Width;
        double availableHeight = Height;

        // Using ActualWidth and ActualHeight ensures REAL measured size
        double panelWidth = InfoPanel.Width;
        double panelHeight = InfoPanel.Height;

        // avoid null/zero bad values
        if (panelWidth < 0) panelWidth = 0;
        if (panelHeight < 0) panelHeight = 0;

        double maxSide;

        if (isLandscape)
        {
            double compassWidth = availableWidth - panelWidth;
            maxSide = Math.Min(compassWidth, availableHeight);
        }
        else
        {
            double compassHeight = availableHeight - panelHeight;
            maxSide = Math.Min(availableWidth, compassHeight);
        }

        // Ensure we never get 0 or negative
        if (maxSide < 50)   // minimum 50px so compass is always visible
            maxSide = Math.Min(availableWidth, availableHeight);

        GraphicsOverlay.WidthRequest = maxSide;
        GraphicsOverlay.HeightRequest = maxSide;
    }




    private void UpdateUI()
    {

        HeadingLabel.Text = $"{record.headingMag:F0}";
        SpeedWaterLabel.Text = $"{record.SOW:F1}";
        SpeedGroundLabel.Text = $"{record.SOG:F1}";
        DepthLabel.Text = $"{record.depth:F1}";
        AWSLabel.Text = $"{record.windAppSpeed:F0}";
        AWDLabel.Text = $"{record.windAppDir:F0}°";
        TWSLabel.Text = $"{record.windTrueSpeed:F0}";
        TWDLabel.Text = $"{record.windTrueCompass:F0}°";
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
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            // LEFT: Compass (Auto width)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // RIGHT: Info panel (fills remaining space)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            MainGrid.SetRow(GraphicsOverlay, 0);
            MainGrid.SetColumn(GraphicsOverlay, 0);

            MainGrid.SetRow(InfoPanel, 0);
            MainGrid.SetColumn(InfoPanel, 1);

           

            
            // side-by-side
            /*MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); // compass
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // text

            Grid.SetRow(GraphicsOverlay, 0);
            Grid.SetColumn(GraphicsOverlay, 0);

            Grid.SetRow(InfoPanel, 0);
            Grid.SetColumn(InfoPanel, 1);*/
        }
        else
        {
            // stacked
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star }); // text
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star }); // compass
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            MainGrid.SetRow(InfoPanel, 0);
            MainGrid.SetColumn(InfoPanel, 0);

            MainGrid.SetRow(GraphicsOverlay, 1);
            MainGrid.SetColumn(GraphicsOverlay, 0);
        }
    }


    private void ResizeFonts()
    {
        double baseSize = Math.Min(Width, Height);

        double headerSize = baseSize * 0.018; // e.g., "Heading"
        double valueSize = baseSize * 0.036; // e.g., "123.45"

        foreach (var lbl in InfoPanel.Children.OfType<Label>())
        {
            if (lbl.FontSize <= 20)   // header label
                lbl.FontSize = headerSize;
            else                      // value label
                lbl.FontSize = valueSize;
        }
    }
    public void OnSetupChanged(Setup settings)
    {

    }
}
