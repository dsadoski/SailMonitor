
using SailMonitor.Models;

namespace SailMonitor;

public partial class PageSetup : ContentView
{
    private Record record = new Record();
    private Setup setup;

    public PageSetup(Setup _setup)
    {
        InitializeComponent();
        setup = _setup;

        Port.Text = setup.Port.ToString();
        Night.IsChecked = setup.Night;
        KeepActive.IsChecked = setup.KeepActive;
        UseGPSPOS.IsChecked = setup.UseGPSPOS;
        UseGPSHEADING.IsChecked = setup.UseGPSHEADING;
        UseGPSSOG.IsChecked = setup.UseGPSSOG;
        SaveFrequency.Text = setup.saveFrequency.ToString();
    }

    public void Save(object sender, EventArgs e)
    {
        int.TryParse(Port.Text, out setup.Port);
        int.TryParse(SaveFrequency.Text, out setup.saveFrequency);
        setup.Night = Night.IsChecked;
        setup.KeepActive = KeepActive.IsChecked;
        setup.UseGPSPOS = UseGPSPOS.IsChecked;
        setup.UseGPSHEADING = UseGPSHEADING.IsChecked;
        setup.UseGPSSOG = UseGPSSOG.IsChecked;

        setup.Save();
        var parentPage = GetParentPage();
        parentPage?.SetColorScheme(setup);
        parentPage?.SetColorsRecursively(this, setup);
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

    public void ToggleNight(object sender, EventArgs e)
    {
        setup.Night = Night.IsChecked;
        setup.SetColor();
        var parentPage = GetParentPage();
        parentPage?.SetColorScheme(setup);
        parentPage?.SetColorsRecursively(this, setup);
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