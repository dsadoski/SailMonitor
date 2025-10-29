

using SailMonitor.Models;
using SailMonitor.Services;


namespace SailMonitor;

public partial class Page2 : ContentView, IContentViewHost
{

    private Record record = new Record();
    
    List<DataPointDisplay> dataPointDisplays =new List<DataPointDisplay>();
    

    public Page2()
    {
		InitializeComponent();
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
            dataPointDisplays.Add(new DataPointDisplay("AWS", "F1"));
            dataPointDisplays.Add(new DataPointDisplay("AWD", "F1"));
            int rowcount = 0;
            int colcount = 0;

            foreach (var display in dataPointDisplays)
            {
                var cellGrid = new Grid
                {
                    WidthRequest = 200,
                    HeightRequest = 200,
                    Padding = 5
                };

                // Add a background GraphicsView (fills the whole cell)
                display.graphicsView= new GraphicsView
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

                display.center = new Label
                {
                    Text = "Center",
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 72,
                    VerticalOptions = LayoutOptions.Center
                };

                // Add elements — GraphicsView first (so it's behind)
                cellGrid.Children.Add(display.graphicsView);
                cellGrid.Children.Add(display.topLeft);
                cellGrid.Children.Add(display.bottomLeft);
                cellGrid.Children.Add(display.bottomRight);
                cellGrid.Children.Add(display.center);


                /* var view = new GraphicsView
                {
                    Drawable = display,
                    HeightRequest = 200,
                    WidthRequest = 200
                };

                var border = new Border
                {
                    Stroke = Colors.Black,
                    StrokeThickness = 1,
                    Content = view,
                    Margin = 0,
                    Padding = 2
                };*/

                MainGrid.Add(cellGrid,colcount,rowcount);
                colcount++;
                if(colcount >= 3)
                {
                    colcount = 0;
                    rowcount++;
                }
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
            //view.Invalidate();
            
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

        foreach (var display in dataPointDisplays)
        {
            display.UpdateUI();
        }

        if (eventName == "RefreshData")
        {
        }

        UpdateUI();
    }
}