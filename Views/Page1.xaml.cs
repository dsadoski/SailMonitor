namespace SailMonitor;

using SailMonitor.Models;
using SailMonitor.Services;

public partial class Page1 : ContentView, IContentViewHost
{
    private Record record = new Record();

    public CompassDrawable CompassDrawable { get; set; }

    public List<FieldDisplay> fieldDisplays { get; set; }

    public List<WindPointDisplay> windPointDisplays { get; set; }

    public GraphicsView compassGraphic { get; set; }

    public Page1()
    {
        InitializeComponent();
        fieldDisplays = new List<FieldDisplay>();
        windPointDisplays = new List<WindPointDisplay>();
        var mainPage = GetParentPage();
        var fieldData = mainPage?.fieldData;
        var setup = new Setup();
        compassGraphic = new GraphicsView();

        // this.BackgroundColor = Colors.White;
        SizeChanged += Page1_SizeChanged;
        CompassDrawable = new CompassDrawable();
        compassGraphic.Drawable = CompassDrawable;

        bool isLandscape = true;

        if (Width > 0 && Height > 0)
        {
            isLandscape = Width > Height;
        }

        AdjustLayout(isLandscape);
        ResizeCompass(isLandscape);
        ResizeFonts();

        OnReSize();

        // Redraw when needed
        compassGraphic.Invalidate();
    }

    private void Page1_SizeChanged(object sender, EventArgs e)
    {
        if (Width <= 0 || Height <= 0)
        {
            return;
        }

        // Delay one tick so that InfoPanel has real width/height
        MainThread.BeginInvokeOnMainThread(() =>
        {
            bool isLandscape = Width > Height;

            AdjustLayout(isLandscape);
            ResizeCompass(isLandscape);
            ResizeFonts();

            compassGraphic.Invalidate();
        });
    }

    private void ResizeCompass(bool isLandscape)
    {
        double availableWidth = Width;
        double availableHeight = Height;

        // Using ActualWidth and ActualHeight ensures REAL measured size
        double panelWidth = Width * .5;
        double panelHeight = Height * .5;

        // avoid null/zero bad values
        if (panelWidth < 0)
        {
            panelWidth = 0;
        }

        if (panelHeight < 0)
        {
            panelHeight = 0;
        }

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
        if (maxSide < 50) // minimum 50px so compass is always visible
        {
            maxSide = Math.Min(availableWidth, availableHeight);
        }

        compassGraphic.WidthRequest = maxSide;
        compassGraphic.HeightRequest = maxSide;
    }

    private void UpdateUI()
    {
        CompassDrawable.Heading = (float)record.headingMag;
        CompassDrawable.ApparentWind = (float)record.windAppDir;
        CompassDrawable.TrueWind = (float)record.windTrueDir;
        compassGraphic.Invalidate();
    }

    // Optional: update wedges dynamically
    public void OnAppEvent(string eventName, Record data, List<FieldData> DataPoints)
    {
        record = data.Copy();

        // OnReSize();
        UpdateUI();
        foreach (var field in fieldDisplays)
        {
            field.Update(DataPoints);
        }

        foreach (var wind in windPointDisplays)
        {
            wind.Update(DataPoints);
        }
    }

    public void OnReSize()
    {
        if (Width <= 0 || Height <= 0)
        {
            return;
        }

        bool isLandscape = Width > Height;
        AdjustLayout(isLandscape);
        ResizeFonts();
    }

    private MainPage? GetParentPage()
    {
        Element? parent = this;
        while (parent != null && parent is not MainPage)
        {
            parent = parent.Parent;
        }

        return parent as MainPage;
    }

    private void AdjustLayout(bool isLandscape)
    {
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.Children.Clear();
        fieldDisplays = new List<FieldDisplay>();
        windPointDisplays = new List<WindPointDisplay>();
        Setup setup = new Setup();

        if (isLandscape)
        {
            if (Width > 0)
            {
                compassGraphic.WidthRequest = Width * .4;
            }

            /*                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }); // Left half
                            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Right col 1
                            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Right col 2*/
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }); // Left ~40% for compass
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) }); // Right col 1 (~30%)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) }); // Right col 2 (~30%)

            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            MainGrid.Children.Add(compassGraphic);
            MainGrid.SetRow(compassGraphic, 0);
            MainGrid.SetColumn(compassGraphic, 0);
            MainGrid.SetRowSpan(compassGraphic, 4);

            fieldDisplays.Add(new FieldDisplay("HDG", MainGrid, setup, 0, 1, "F0", "Heading"));
            fieldDisplays.Add(new FieldDisplay("DPT", MainGrid, setup, 0, 2, "F1", "Depth"));
            fieldDisplays.Add(new FieldDisplay("SOW", MainGrid, setup, 1, 1, "F1", "Speed Ground"));
            fieldDisplays.Add(new FieldDisplay("SOG", MainGrid, setup, 1, 2, "F1", "Speed Water"));
            windPointDisplays.Add(new WindPointDisplay("AWD", "AWS", MainGrid, setup, 2, 1, "F0", "Apparent"));
            windPointDisplays.Add(new WindPointDisplay("WTC", "TWS", MainGrid, setup, 2, 2, "F0", "True"));
        }
        else
        {
            compassGraphic.HeightRequest = Height * .4;
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }); // Left half
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }); // Right col 1

            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.25, GridUnitType.Star) });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.25, GridUnitType.Star) });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.25, GridUnitType.Star) });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });

            fieldDisplays.Add(new FieldDisplay("HDG", MainGrid, setup, 0, 0, "F0", "Heading"));
            fieldDisplays.Add(new FieldDisplay("DPT", MainGrid, setup, 0, 1, "F1", "Depth"));
            fieldDisplays.Add(new FieldDisplay("SOW", MainGrid, setup, 1, 0, "F1", "Speed Water"));
            fieldDisplays.Add(new FieldDisplay("SOG", MainGrid, setup, 1, 1, "F1", "Speed Ground"));
            windPointDisplays.Add(new WindPointDisplay("AWD", "AWS", MainGrid, setup, 2, 0, "F0", "Apparent"));
            windPointDisplays.Add(new WindPointDisplay("WTC", "TWS", MainGrid, setup, 2, 1, "F0", "True"));
            MainGrid.Children.Add(compassGraphic);
            MainGrid.SetRow(compassGraphic, 3);
            MainGrid.SetColumn(compassGraphic, 0);
            MainGrid.SetColumnSpan(compassGraphic, 2);
        }
    }

    private void ResizeFonts()
    {
        double baseSize = Math.Min(Width, Height);

        double headerSize = baseSize * 0.018; // e.g., "Heading"
        double valueSize = baseSize * 0.036; // e.g., "123.45"

        foreach (var windPointDisplay in windPointDisplays)
        {
            windPointDisplay.Resize(Width, Height);
        }

        foreach (var fieldDisplay in fieldDisplays)
        {
            fieldDisplay.Resize(Width, Height);
        }
    }

    public void OnSetupChanged(Setup settings)
    {
    }
}
