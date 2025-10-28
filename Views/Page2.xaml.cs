
using SailMonitor.Models;
using SailMonitor.Services;


namespace SailMonitor;

public partial class Page2 : ContentView, IContentViewHost
{

    private Record record = new Record();
    
    List<DataPointDisplay> dataPointDisplays =new List<DataPointDisplay>();
    List<GraphicsView> graphicsViews = new List<GraphicsView>();
    public Page2()
    {
		InitializeComponent();
        try
        {
           


            for (int i = 0; i < 3; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            for (int i = 0; i < 2; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Redraw when needed
            dataPointDisplays.Add(new DataPointDisplay("AWS", "F1"));
            dataPointDisplays.Add(new DataPointDisplay("AWD", "F1"));

            foreach (var display in dataPointDisplays)
            {
                var view = new GraphicsView
                {
                    Drawable = display,
                    HeightRequest = 400,
                    WidthRequest = 400
                };

                var border = new Border
                {
                    Stroke = Colors.Black,
                    StrokeThickness = 1,
                    Content = view,
                    Margin = 0,
                    Padding = 2
                };


                MainGrid.Add(border);
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Page2 constructor: {ex.Message}");
        }

    }


    private void UpdateUI()
    {

        var graphicsViews = MainGrid.Children.OfType<GraphicsView>().ToList();

        foreach (var view in graphicsViews)
        {
            view.Invalidate();
        }   

    }

    public void UpdateRecord(string name, double value)
    {
        var view = dataPointDisplays.FirstOrDefault(d => d.name == name);
        if (view != null)
        {
            view.AddDataPoint(value);
        }
    }
    public void Dispose()
    {
        /*_udpService.OnMessageReceived -= HandleUdpMessage;
        _gpsService.OnLocationReceived -= HandleGpsLocation;*/
    }

    public void OnAppEvent(string eventName, Record data)
    {
        record = data.Copy();
        UpdateRecord("AWS", record.windAppSpeed);
        UpdateRecord("AWD", record.windAppDir);

        if (eventName == "RefreshData")
        {
        }
    }
}