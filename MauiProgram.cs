using Microsoft.Extensions.Logging;
using SailMonitor.Services;

namespace SailMonitor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {




            var setup = new Models.Setup();
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
                builder.Services.AddSingleton(sp => new UdpListenerService(port: 10110));
                builder.Services.AddSingleton(sp => new GPSService());
                builder.Services.AddSingleton(sp => new NmeaService(setup));
            }

            catch (Exception ex)
            {
                Console.WriteLine($"MauiApp Creation Error: {ex.Message}");
            }

            return builder.Build();

        }
    }
}
