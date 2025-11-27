using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace SailMonitor
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen,
                            Android.Views.WindowManagerFlags.Fullscreen);

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)
             (
                 SystemUiFlags.LayoutFullscreen |
                 SystemUiFlags.LayoutHideNavigation |
                 SystemUiFlags.HideNavigation |
                 SystemUiFlags.Fullscreen |
                 SystemUiFlags.ImmersiveSticky
             );

        }
    }
}
