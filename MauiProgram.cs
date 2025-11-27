namespace SailMonitor
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Maui.LifecycleEvents;
    using SailMonitor.Models;
    using SailMonitor.Services;
    #if WINDOWS
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    #endif

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.ConfigureLifecycleEvents(events =>
            {
                #if WINDOWS
                    events.AddWindows(w =>
                    {
                        w.OnWindowCreated(window =>
                        {
                            window.ExtendsContentIntoTitleBar = true; //If you need to completely hide the minimized maximized close button, you need to set this value to false.
                            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                            var _appWindow = AppWindow.GetFromWindowId(myWndId);
                            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                        });
                    });
                #endif
            });
            try
            {
                builder.Services.AddSingleton(sp => new Setup());
                Setup setup = new Setup();
                builder.Services.AddSingleton(sp => new NmeaService(setup));
                NmeaService nmeaService = new NmeaService(setup);
                builder.Services.AddSingleton(sp => new UdpListenerService(setup, nmeaService));
                builder.Services.AddSingleton(sp => new GPSService());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MauiApp Creation Error: {ex.Message}");
            }

            return builder.Build();
        }
    }
}
