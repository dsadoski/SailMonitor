namespace SailMonitor;

using Microsoft.Maui.Controls.Shapes;
using SailMonitor.Models;
using SailMonitor.Services;

public partial class Page1 : ContentView, IContentViewHost
{
    private Record record = new();
    private Setup setup;
    private bool? lastLandscape;
    private List<FieldData> lastDataPoints = new();
    private Label apparentSpeed = InstrumentStatistics.CreateLabel("--");
    private Label trueSpeed = InstrumentStatistics.CreateLabel("--");
    private Label apparentCaption = InstrumentStatistics.CreateLabel("APPARENT");
    private Label trueCaption = InstrumentStatistics.CreateLabel("TRUE");

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
        if (Width <= 0 || Height <= 0)
        {
            return;
        }

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
        // Detach the shared GraphicsView before placing it into the new orientation.
        if (compassGraphic.Parent is Grid previousCompass)
        {
            previousCompass.Children.Remove(compassGraphic);
        }
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
            // Keep the same reading order in landscape, with the compass on the right.
            for (int i = 0; i < 4; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            }
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.95, GridUnitType.Star)));
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.95, GridUnitType.Star)));
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1.85, GridUnitType.Star)));

            fieldDisplays.Add(new FieldDisplay("SOG", MainGrid, setup, 0, 0, "F1", "SOG", setup.Speed.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("SOW", MainGrid, setup, 0, 1, "F1", "SOW", setup.Speed.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("DPT", MainGrid, setup, 1, 0, "F1", "Depth", setup.Depth.SelectedUnit));
            fieldDisplays.Add(new FieldDisplay("HDG", MainGrid, setup, 1, 1, "F0", "Heading", "°"));
            windPointDisplays.Add(new WindPointDisplay("AWD", "AWS", MainGrid, setup, 2, 0, "F0", "Apparent", setup.WindSpeed.SelectedUnit));
            windPointDisplays.Add(new WindPointDisplay("WTC", "TWS", MainGrid, setup, 2, 1, "F0", "True", setup.WindSpeed.SelectedUnit));
            AddCompass(0, 2, 3, 2);
        }
        else
        {
            // Portrait keeps the big values above the compass for distance readability.
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));

            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.95, GridUnitType.Star)));
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.95, GridUnitType.Star)));
            // Give the two wind cards enough vertical room for both Min/Avg/Max rows.
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1.85, GridUnitType.Star)));
            MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(2.55, GridUnitType.Star)));

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
        foreach (var field in fieldDisplays)
        {
            field.Update(lastDataPoints);
        }
        foreach (var wind in windPointDisplays)
        {
            wind.Update(lastDataPoints);
        }
        UpdateCornerSpeeds();
    }

    private void AddCompass(int row, int column, int rowSpan, int columnSpan)
    {
        // Fixed readouts have their own row, outside the rotating compass drawing.
        var compassLayout = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(new GridLength(0.17, GridUnitType.Star)),
                new RowDefinition(new GridLength(0.83, GridUnitType.Star))
            }
        };
        var readouts = new Grid
        {
            Padding = new Thickness(8, 2),
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(new GridLength(0.4, GridUnitType.Star)),
                new RowDefinition(new GridLength(0.6, GridUnitType.Star))
            }
        };
        apparentCaption = InstrumentStatistics.CreateLabel("APPARENT");
        trueCaption = InstrumentStatistics.CreateLabel("TRUE");
        apparentSpeed = InstrumentStatistics.CreateLabel("--");
        trueSpeed = InstrumentStatistics.CreateLabel("--");
        readouts.Add(apparentCaption, 0, 0);
        readouts.Add(trueCaption, 1, 0);
        readouts.Add(apparentSpeed, 0, 1);
        readouts.Add(trueSpeed, 1, 1);
        foreach (var label in new[] { apparentCaption, apparentSpeed, trueCaption, trueSpeed })
        {
            label.FontAttributes = FontAttributes.Bold;
        }
        void ResizeReadouts()
        {
            if (readouts.Width <= 0 || readouts.Height <= 0)
            {
                return;
            }
            foreach (var label in new[] { apparentCaption, trueCaption })
            {
                label.FontSize = InstrumentStatistics.Fit(label.Text, readouts.Width / 2 - 16, (readouts.Height - 4) * 0.4, 18);
            }
            foreach (var label in new[] { apparentSpeed, trueSpeed })
            {
                label.FontSize = InstrumentStatistics.Fit(label.Text, readouts.Width / 2 - 16, (readouts.Height - 4) * 0.6, 32);
            }
        }
        readouts.SizeChanged += (_, _) => ResizeReadouts();
        apparentSpeed.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Label.Text))
            {
                ResizeReadouts();
            }
        };
        trueSpeed.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Label.Text))
            {
                ResizeReadouts();
            }
        };
        compassLayout.Add(readouts, 0, 0);
        compassLayout.Add(compassGraphic, 0, 1);
        var compassCard = new Border
        {
            Content = compassLayout,
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
        if (Width <= 0 || Height <= 0)
        {
            return;
        }
        foreach (var wind in windPointDisplays)
        {
            wind.Resize(Width, Height);
        }
        foreach (var field in fieldDisplays)
        {
            field.Resize(Width, Height);
        }
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
        lastDataPoints = dataPoints;
        UpdateCornerSpeeds();
        UpdateUI();
        foreach (var field in fieldDisplays)
        {
            field.Update(dataPoints);
        }
        foreach (var wind in windPointDisplays)
        {
            wind.Update(dataPoints);
        }
    }

    private void UpdateCornerSpeeds()
    {
        var apparent = lastDataPoints.FirstOrDefault(d => d.name == "AWS");
        var trueWind = lastDataPoints.FirstOrDefault(d => d.name == "TWS");
        apparentSpeed.Text = (apparent == null ? "--" : apparent.Current.ToString("F0")) + " " + setup.WindSpeed.SelectedUnit;
        trueSpeed.Text = (trueWind == null ? "--" : trueWind.Current.ToString("F0")) + " " + setup.WindSpeed.SelectedUnit;
    }

    public void OnReSize()
    {
        if (Width <= 0 || Height <= 0)
        {
            return;
        }
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
        UpdateCornerSpeeds();
        compassGraphic.Invalidate();
    }

    private void ApplyTheme()
    {
        ApplyBackground();
        foreach (var field in fieldDisplays)
        {
            field.ApplyTheme(setup);
        }
        foreach (var wind in windPointDisplays)
        {
            wind.ApplyTheme(setup);
        }

        foreach (var label in new[] { apparentCaption, apparentSpeed, trueCaption, trueSpeed })
        {
            label.BackgroundColor = Colors.Transparent;
            label.TextColor = Color.FromArgb(label == apparentCaption || label == apparentSpeed
                ? (setup.Night ? "#1687E8" : "#1267C4")
                : (setup.Night ? "#18B957" : "#087A32"));
        }
        var borderColor = setup.Night ? Color.FromArgb("#3A2020") : Color.FromArgb("#D9E2EE");
        var cardColor = setup.Night ? Color.FromArgb("#080808") : Color.FromArgb("#FBFCFE");
        foreach (var border in MainGrid.Children.OfType<Border>())
        {
            if (border.StyleId == "CompassCard")
            {
                border.BackgroundColor = cardColor;
                border.Stroke = borderColor;
                if (border.Content is Grid compassLayout)
                {
                    compassLayout.BackgroundColor = Colors.Transparent;
                    foreach (var grid in compassLayout.Children.OfType<Grid>())
                    {
                        grid.BackgroundColor = Colors.Transparent;
                    }
                }
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
        while (parent != null && parent is not MainPage)
        {
            parent = parent.Parent;
        }
        return parent as MainPage;
    }

    private void OnSwipeLeft(object sender, SwipedEventArgs e) => GetParentPage()?.NextPage();
    private void OnSwipeRight(object sender, SwipedEventArgs e) => GetParentPage()?.PrevPage();
}
