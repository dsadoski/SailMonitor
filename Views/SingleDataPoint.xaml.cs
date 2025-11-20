

using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
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

        

        try
        {


            // Add a background GraphicsView (fills the whole cell)
            dataPoint.graphicsView = new GraphicsView
            {
                Drawable = dataPoint,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                AnchorX = 0,
                AnchorY = 0,
                WidthRequest = screenWidth,
                HeightRequest = screenHeight,
                
            };
            MainLayout.Children.Add(dataPoint.graphicsView);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Page2 constructor: {ex.Message}");
        }

    }


    public void OnAppEvent(string eventName, Record data, List<FieldData> DataPoints)
    {
        record = data.Copy();
        var point = DataPoints.Where(d  => d.name == dataPoint.name).FirstOrDefault();

        MainLayout.MaximumWidthRequest = DeviceDisplay.MainDisplayInfo.Width;
        MainLayout.MinimumHeightRequest = DeviceDisplay.MainDisplayInfo.Height;
        dataPoint.width = MainLayout.Width; 
        dataPoint.height = MainLayout.Height;

        dataPoint.fieldData = point;
        dataPoint.graphicsView.Invalidate();
    }
    public void OnReSize()
    {
    }

    public void OnSetupChanged(Setup settings)
    {

    }
}