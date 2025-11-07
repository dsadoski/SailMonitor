

using Microsoft.Maui.Controls;
using SailMonitor.Models;
using SailMonitor.Services;


namespace SailMonitor;

public partial class SingleDataPoint : ContentView, IContentViewHost
{

    private Record record = new Record();
    
    
    

    GraphicsView graphicsView;
    DataPointDisplay dataPoint;


    public SingleDataPoint(DataPointDisplay dataPointdisplay)
    {
        InitializeComponent();
        //this.BackgroundColor = Colors.White;
        var displayInfo = DeviceDisplay.MainDisplayInfo;
        dataPoint = dataPointdisplay;

        // width & height are in raw pixels
        double width = displayInfo.Width;
        double height = displayInfo.Height;

        // convert to device-independent units (DIPs)
        double screenWidth = width / displayInfo.Density;
        double screenHeight = height / displayInfo.Density;

        // example: set a view’s size

        try
        {


            // Add a background GraphicsView (fills the whole cell)
            dataPoint.graphicsView = new GraphicsView
            {
                Drawable = dataPoint,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            MainLayout.Children.Add(dataPoint.graphicsView);


            // Add labels (foreground content)
            dataPoint.topLeft = new Label
            {
                Text = "Top Left",                
                FontSize = 18,
            };
            MainLayout.Children.Add(dataPoint.topLeft);

            dataPoint.bottomLeft = new Label
            {
                Text = "Bottom Left",
                FontSize = 18,
            };
            MainLayout.Children.Add(dataPoint.bottomLeft);

            dataPoint.bottomRight = new Label
            {
                Text = "Bottom Right",                
                FontSize = 18,
                
            };
            MainLayout.Children.Add(dataPoint.bottomRight);

            dataPoint.center = new Label
            {
                Text = "Center",

                FontSize = 36,
            };
            MainLayout.Children.Add(dataPoint.center);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Page2 constructor: {ex.Message}");
        }

    }

   

    

     public void Dispose()
    {
        /*_udpService.OnMessageReceived -= HandleUdpMessage;
        _gpsService.OnLocationReceived -= HandleGpsLocation;*/
    }

    public void OnAppEvent(string eventName, Record data, List<DataPointDisplay> dataPoints)
    {
        record = data.Copy();
        var point = dataPoints.Where(d  => d.name == dataPoint.name).FirstOrDefault();

        MainLayout.MaximumWidthRequest = DeviceDisplay.MainDisplayInfo.Width;
        MainLayout.MinimumHeightRequest = DeviceDisplay.MainDisplayInfo.Height;

        dataPoint.fieldData = point.fieldData;
        //dataPoint.graphicsView.Invalidate();
        dataPoint.UpdateUI();

        
    }
}