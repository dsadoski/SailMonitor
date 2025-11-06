using Microsoft.Extensions.Logging;
using SailMonitor.Models;
using SailMonitor.Services;

namespace SailMonitor
{
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
