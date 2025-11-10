
using SailMonitor.Models;
using SailMonitor.Services;
using System.ComponentModel;
namespace SailMonitor;

public partial class PageSetup : ContentView
{
    private Record record = new Record();
    Setup setup;
    


    public PageSetup()
    {
        InitializeComponent();
        setup = new Setup();

        Port.Text = setup.Port.ToString();
        Night.IsToggled = setup.Night;
        KeepActive.IsToggled = setup.KeepActive;
        UseGPSPOS.IsToggled = setup.UseGPSPOS;
        UseGPSHEADING.IsToggled = setup.UseGPSHEADING;
        UseGPSSOG.IsToggled = setup.UseGPSSOG;
        SaveFrequency.Text = setup.saveFrequency.ToString();

        
    }

    public void Save(object sender, EventArgs e)
    {
        int.TryParse(Port.Text, out setup.Port);
        int.TryParse(SaveFrequency.Text, out setup.saveFrequency);
        setup.Night = Night.IsToggled;
        setup.KeepActive = KeepActive.IsToggled;
        setup.UseGPSPOS = UseGPSPOS.IsToggled;
        setup.UseGPSHEADING = UseGPSHEADING.IsToggled;
        setup.UseGPSSOG = UseGPSSOG.IsToggled;
        
        setup.Save();
        var parentPage = GetParentPage();
        parentPage?.SetColorScheme(setup);
        parentPage?.SetColorsRecursively(this, setup);

    }

    MainPage? GetParentPage()
    {
        Element? parent = this;
        while (parent != null && parent is not MainPage)
            parent = parent.Parent;
        return parent as MainPage;
    }

    public void ToggleNight(object sender, EventArgs e)
    {
        setup.Night = Night.IsToggled;
        setup.SetColor();
        var parentPage = GetParentPage();
        parentPage?.SetColorScheme(setup);
            //parentPage?.SetColorsRecursively(this, setup);

    }

}