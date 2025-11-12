

using Microsoft.Maui.Controls;
using SailMonitor.Models;
using SailMonitor.Services;


namespace SailMonitor;

public partial class Page2 : ContentView, IContentViewHost
{

    private Record record = new Record();


    public CompassDrawable CompassDrawable { get; set; }

    GraphicsView graphicsView;
    List<DataPointDisplay> datapoints;


    public Page2(List<DataPointDisplay> dataPointdisplay)
    {
        InitializeComponent();
        datapoints = dataPointdisplay;
        //this.BackgroundColor = Colors.White;
        var displayInfo = DeviceDisplay.MainDisplayInfo;

        // width & height are in raw pixels
        double width = displayInfo.Width;
        double height = displayInfo.Height;

        // convert to device-independent units (DIPs)
        double screenWidth = width / displayInfo.Density;
        double screenHeight = height / displayInfo.Density;

        // example: set a view’s size
        MainGrid.WidthRequest = screenWidth;
        MainGrid.HeightRequest = screenHeight;
        try
        {



            for (int i = 0; i < 4; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int i = 0; i < 3; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Redraw when needed

            int rowcount = 0;
            int colcount = 0;


            foreach (var display in datapoints)
            {
                var cellGrid = new Grid
                {
                    WidthRequest = screenWidth / 4,
                    HeightRequest = screenHeight / 5,
                    Padding = 5
                };

                // Add a background GraphicsView (fills the whole cell)
                display.graphicsView = new GraphicsView
                {
                    Drawable = display,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                // Add labels (foreground content)
                display.topLeft = new Label
                {
                    Text = "Top Left",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    FontSize = 18,
                    Margin = new Thickness(6)
                };

                display.bottomLeft = new Label
                {
                    Text = "Bottom Left",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 18,
                    Margin = new Thickness(6)
                };

                display.bottomRight = new Label
                {
                    Text = "Bottom Right",
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 18,
                    Margin = new Thickness(6)
                };

                display.center = new Label { Text = "Center", HorizontalOptions = LayoutOptions.Center, FontSize = 36, VerticalOptions = LayoutOptions.Center };

                // Add elements — GraphicsView first (so it's behind)
                cellGrid.Children.Add(display.graphicsView);
                cellGrid.Children.Add(display.topLeft);
                cellGrid.Children.Add(display.bottomLeft);
                cellGrid.Children.Add(display.bottomRight);
                cellGrid.Children.Add(display.center);



                MainGrid.Add(cellGrid, colcount, rowcount);
                colcount++;
                if (colcount >= 3)
                {
                    colcount = 0;
                    rowcount++;
                }
            }

            var comptGrid = new Grid
            {
                WidthRequest = screenWidth,
                HeightRequest = screenHeight * 3 / 5,
                Padding = 5
            };
            CompassDrawable = new CompassDrawable();
            graphicsView = new GraphicsView
            {
                Drawable = CompassDrawable,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            comptGrid.Children.Add(graphicsView);
            MainGrid.Add(comptGrid, 0, rowcount++);



        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Page2 constructor: {ex.Message}");
        }

    }



    private void UpdateUI()
    {

        var graphicsViews = MainGrid.Children.OfType<GraphicsView>().ToList();
        CompassDrawable.Heading = (float)record.headingMag;
        CompassDrawable.ApparentWind = (float)record.windAppDir;
        CompassDrawable.TrueWind = (float)record.windTrueDir;
        graphicsView.Invalidate();

    }

    public void Dispose()
    {
        /*_udpService.OnMessageReceived -= HandleUdpMessage;
        _gpsService.OnLocationReceived -= HandleGpsLocation;*/
    }

    public void OnAppEvent(string eventName, Record data, List<FieldData> DataPoints)
    {
        record = data.Copy();



        foreach (var point in DataPoints)
        {

            /*display.UpdateUI();
            display.graphicsView.Invalidate();*/

        }



        UpdateUI();
    }

    public void OnReSize()
    {
    }

    public void OnSetupChanged(Setup settings)
    {

    }
}