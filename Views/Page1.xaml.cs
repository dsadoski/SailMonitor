namespace SailMonitor;

using Microsoft.Maui.Controls.Shapes;
using SailMonitor.Models;
using SailMonitor.Services;

public partial class Page1 : ContentView, IContentViewHost
{
    private Record record = new();
    private Setup setup;
    private bool? lastLandscape;

    public CompassDrawable CompassDrawable { get; private set; }
    public List<FieldDisplay> fieldDisplays { get; private set; } = new();
    public List<WindPointDisplay> windPointDisplays { get; private set; } = new();
    public GraphicsView compassGraphic { get; private set; }

    public Page1(Setup setup)
    {
        InitializeComponent();
        this.setup = setup;

        compassGraphic = new GraphicsView();
        CompassDrawable = new CompassDrawable(setup);
        compassGraphic.Drawable = CompassDrawable;

        SizeChanged += Page1_SizeChanged;
        ApplyBackground();
    }

    private void Page1_SizeChanged(object? sender, EventArgs e)
    {
        if (Width <= 0 || Height <= 0) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            bool landscape = Width > Height;
            if (lastLandscape != landscape || MainGrid.Children.Count == 0)
            {
                AdjustLayout(landscape);
                lastLandscape = landscape;
            }

            ResizeFonts();
            compassGraphic.Invalidate();
        });
    }

    private void AdjustLayout(bool landscape)
    {
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.Children.Clear();
        MainGrid.Padding = new Thickness(4, 2, 4, 4);
        MainGrid.RowSpacing = 2;
        MainGrid.ColumnSpacing = 2;

        fieldDisplays = new();
        windPointDisplays = new();

        if (landscape)
        {
            // Three instrument cards, large central compass, two wind cards.
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2.6, GridUnitType.Star)));
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(4.8, GridUnitType.Star)));
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2.6, GridUnitType.Star)));

            for (int i = 0; i < 3; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

            fieldDisplays.Add(new FieldDisplay("SOG", MainGrid, setup, 0, 0, "F1", "SOG", setup.Speed.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("SOW", MainGrid, setup, 1, 0, "F1", "SOW", setup.Speed.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("DPT", MainGrid, setup, 2, 0, "F1", "Depth", setup.Depth.SelectedUnit));

            AddCompass(0, 1, 3, 1);

            windPointDisplays.Add(new WindPointDisplay("AWD", "AWS", MainGrid, setup, 0, 2, "F0", "Apparent", setup.WindSpeed.SelectedUnit));
            windPointDisplays.Add(new WindPointDisplay("WTC", "TWS", MainGrid, setup, 1, 2, "F0", "True", setup.WindSpeed.SelectedUnit));

            // Heading card fills the remaining lower-right position.
            fieldDisplays.Add(new FieldDisplay("HDG", MainGrid, setup, 2, 2, "F0", "Heading", "°"));
        }
        else
        {
            // Portrait keeps the big values above the compass for distance readability.
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));

            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.90, GridUnitType.Star)));
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.90, GridUnitType.Star)));
            // Give the two wind cards enough vertical room for both Min/Avg/Max rows.
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1.65, GridUnitType.Star)));
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(3.05, GridUnitType.Star)));

            fieldDisplays.Add(new FieldDisplay("SOG", MainGrid, setup, 0, 0, "F1", "SOG", setup.Speed.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("SOW", MainGrid, setup, 0, 1, "F1", "SOW", setup.Speed.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("DPT", MainGrid, setup, 1, 0, "F1", "Depth", setup.Depth.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("HDG", MainGrid, setup, 1, 1, "F0", "Heading", "°"));

            windPointDisplays.Add(new WindPointDisplay("AWD", "AWS", MainGrid, setup, 2, 0, "F0", "Apparent", setup.WindSpeed.SelectedUnit));
            windPointDisplays.Add(new WindPointDisplay("WTC", "TWS", MainGrid, setup, 2, 1, "F0", "True", setup.WindSpeed.SelectedUnit));

            AddCompass(3, 0, 1, 2);
        }

        ResizeFonts();
        ApplyTheme();
    }

    private void AddCompass(int row, int column, int rowSpan, int columnSpan)
    {
        var compassCard = new Border
        {
            Content = compassGraphic,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Margin = new Thickness(4),
            Padding = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StyleId = "CompassCard"
        };

        MainGrid.Children.Add(compassCard);
        Grid.SetRow(compassCard, row);
        Grid.SetColumn(compassCard, column);
        Grid.SetRowSpan(compassCard, rowSpan);
        Grid.SetColumnSpan(compassCard, columnSpan);
    }

    private void ResizeFonts()
    {
        if (Width <= 0 || Height <= 0) return;
        foreach (var wind in windPointDisplays) wind.Resize(Width, Height);
        foreach (var field in fieldDisplays) field.Resize(Width, Height);
    }

    private void UpdateUI()
    {
        CompassDrawable.Heading = (float)record.headingMag;
        CompassDrawable.ApparentWind = (float)record.windAppDir;
        CompassDrawable.TrueWind = (float)record.windTrueDir;
        compassGraphic.Invalidate();
    }

    public void OnAppEvent(string eventName, Record data, List<FieldData> dataPoints)
    {
        record = data.Copy();
        UpdateUI();
        foreach (var field in fieldDisplays) field.Update(dataPoints);
        foreach (var wind in windPointDisplays) wind.Update(dataPoints);
    }

    public void OnReSize()
    {
        if (Width <= 0 || Height <= 0) return;
        bool landscape = Width > Height;
        if (lastLandscape != landscape || MainGrid.Children.Count == 0)
        {
            AdjustLayout(landscape);
            lastLandscape = landscape;
        }
        ResizeFonts();
        compassGraphic.Invalidate();
    }

    public void OnSetupChanged(Setup settings)
    {
        setup = settings;
        CompassDrawable.UpdateSetup(settings);
        ApplyTheme();
        compassGraphic.Invalidate();
    }

    private void ApplyTheme()
    {
        ApplyBackground();
        foreach (var field in fieldDisplays) field.ApplyTheme(setup);
        foreach (var wind in windPointDisplays) wind.ApplyTheme(setup);

        var borderColor = setup.Night ? Color.FromArgb("#3A2020") : Color.FromArgb("#D9E2EE");
        var cardColor = setup.Night ? Color.FromArgb("#080808") : Color.FromArgb("#FBFCFE");
        foreach (var border in MainGrid.Children.OfType<Border>())
        {
            if (border.StyleId == "CompassCard")
            {
                border.BackgroundColor = cardColor;
                border.Stroke = borderColor;
            }
        }
    }

    private void ApplyBackground()
    {
        BackgroundColor = setup.backColor;
        MainGrid.BackgroundColor = setup.backColor;
    }

    private MainPage? GetParentPage()
    {
        Element? parent = this;
        while (parent != null && parent is not MainPage) parent = parent.Parent;
        return parent as MainPage;
    }

    private void OnSwipeLeft(object sender, SwipedEventArgs e) => GetParentPage()?.NextPage();
    private void OnSwipeRight(object sender, SwipedEventArgs e) => GetParentPage()?.PrevPage();
}
