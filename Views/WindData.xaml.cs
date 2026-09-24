using SailMonitor.Models;
using SailMonitor.Services;

namespace SailMonitor;

public partial class WindData : ContentView, IContentViewHost
{
    private Record record = new Record();

    private GraphicsView graphicsView;
    private WindDisplay dataPoint;

    public WindData(WindDisplay dataPointdisplay)
    {
        InitializeComponent();
        
        // this.BackgroundColor = Colors.White;
        var displayInfo = DeviceDisplay.MainDisplayInfo;
        dataPoint = dataPointdisplay;
        Box1.IsChecked = dataPoint.drawRaw;
        Box1.CheckedChanged += (s, e) =>
        {
            dataPoint.drawRaw = Box1.IsChecked;
          
        };
        Box2.IsChecked = dataPoint.drawSmoothed;
        Box2.CheckedChanged += (s, e) =>
        {
            dataPoint.drawSmoothed = Box2.IsChecked;
          
        };
        Box3.IsChecked = dataPoint.drawAveraged;
        Box3.CheckedChanged += (s, e) =>
        {
            dataPoint.drawAveraged = Box3.IsChecked;
        };


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

            dataPoint.graphicsView.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Left,
                Command = new Command(() =>
                {
                    GetParentPage()?.NextPage();
                })
            });

            // Swipe right
            dataPoint.graphicsView.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Right,
                Command = new Command(() =>
                {
                    GetParentPage()?.PrevPage();
                })
            });




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
        dataPoint.SpeedData = DataPoints.Where(d => d.name == dataPoint.speedName).FirstOrDefault();
        dataPoint.DirData = DataPoints.Where(d => d.name == dataPoint.dirName).FirstOrDefault();

        MainLayout.MaximumWidthRequest = DeviceDisplay.MainDisplayInfo.Width;
        MainLayout.MinimumHeightRequest = DeviceDisplay.MainDisplayInfo.Height;
        dataPoint.Width = MainLayout.Width;
        dataPoint.Height = MainLayout.Height;

        
        dataPoint.graphicsView.Invalidate();
    }

    public void OnReSize()
    {
    }

    public void OnSetupChanged(Setup settings)
    {
        this.dataPoint.UpdateSetup(settings);
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

    private void OnSwipeLeft(object sender, SwipedEventArgs e)
    {
        var mainPage = GetParentPage();
        mainPage?.NextPage();
    }

    private void OnSwipeRight(object sender, SwipedEventArgs e)
    {
        var mainPage = GetParentPage();
        mainPage?.PrevPage();
    }
}