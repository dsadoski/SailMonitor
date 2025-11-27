using SailMonitor.Models;
using SailMonitor.Services;

namespace SailMonitor;

public partial class PageSetup : ContentView, IContentViewHost
{
    private Record record = new Record();
    private Setup setup;
    public bool loading;
    public List<Button> depthButtons;
    public List<Button> speedButtons;
    public List<Button> windSpeedButtons;

    public PageSetup(Setup _setup)
    {
        InitializeComponent();
        BindingContext = this;
        setup = _setup;
        loading = true;

        Port.Text = setup.Port.ToString();
        Night.IsChecked = setup.Night;
        KeepActive.IsChecked = setup.KeepActive;
        UseGPSPOS.IsChecked = setup.UseGPSPOS;
        UseGPSHEADING.IsChecked = setup.UseGPSHEADING;
        UseGPSSOG.IsChecked = setup.UseGPSSOG;
        SaveFrequency.Text = setup.saveFrequency.ToString();

        depthButtons = SetUnits(DepthGrid, setup.Depth);
        speedButtons = SetUnits(SpeedGrid, setup.Speed);
        windSpeedButtons = SetUnits(WindSpeedGrid, setup.WindSpeed);
        loading = false;
    }

    public List<Button> SetUnits(VerticalStackLayout view, UnitOfMeasure unitOfMeasure)
    {
        List<Button> buttons = new List<Button>();
        foreach (var item in unitOfMeasure.UnitList)
        {
            var button = new Button();
            button.Text = item.Name;
            button.Padding = 1;
            button.CommandParameter = unitOfMeasure.Name + "~" + item.Name;
            button.Clicked += unit_Clicked;
            view.Children.Add(button);
            buttons.Add(button);
        }

        SetButtonColors(buttons, unitOfMeasure);
        return buttons;
    }

    public void SetButtonColors(List<Button> buttons, UnitOfMeasure unitOfMeasure)
    {
        var txt = unitOfMeasure.Name + "~" + unitOfMeasure.SelectedUnit;
        foreach (var button in buttons)
        {
            if (button.CommandParameter.ToString() == txt)
            {
                button.TextColor = setup.backColor;
                button.BackgroundColor = setup.foreColor;
            }
            else
            {
                button.TextColor = setup.foreColor;
                button.BackgroundColor = setup.backColor;
            }
        }
    }


    public void unit_Clicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        var names = new StringParser().TildaListToStrings(button.CommandParameter.ToString());
        if (names[0] == setup.Depth.Name)
        {
            setup.Depth.SelectedUnit = names[1];
            SetButtonColors(depthButtons, setup.Depth);

        }
        else if (names[0] == setup.Speed.Name)
        {
            setup.Speed.SelectedUnit = names[1];
            SetButtonColors(speedButtons, setup.Speed);
        }
        else if (names[0] == setup.WindSpeed.Name)
        {
            setup.WindSpeed.SelectedUnit = names[1];
            SetButtonColors(windSpeedButtons, setup.WindSpeed);
        }
    }

    public void SaveButtonClicked(object sender, EventArgs e)
    {
        Save();
    }

    public void Save()
    {
        if (loading) return;
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
        DeviceDisplay.KeepScreenOn = setup.KeepActive;
        OnSetupChanged(setup);
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

    public void ToggleCheckBox(object sender, EventArgs e)
    {
        Save();
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

    public void OnAppEvent(string eventName, Record record, List<FieldData> DataPoints) { }

    public void OnReSize()
    {

    }

    public void OnSetupChanged(Setup settings)
    {
        SetButtonColors(depthButtons, setup.Depth);

        SetButtonColors(speedButtons, setup.Speed);

        SetButtonColors(windSpeedButtons, setup.WindSpeed);
    }
}