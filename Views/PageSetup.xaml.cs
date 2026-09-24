using SailMonitor.Models;
using SailMonitor.Services;

namespace SailMonitor;

public partial class PageSetup : ContentView, IContentViewHost
{
    private readonly Setup setup;
    private bool loading;
    private bool landscapeLayout;

    public PageSetup(Setup setup)
    {
        InitializeComponent();
        this.setup = setup;
        loading = true;

        Port.Text = setup.Port.ToString();
        Night.IsToggled = setup.Night;
        KeepActive.IsToggled = setup.KeepActive;
        UseGPSPOS.IsToggled = setup.UseGPSPOS;
        UseGPSHEADING.IsToggled = setup.UseGPSHEADING;
        UseGPSSOG.IsToggled = setup.UseGPSSOG;
        SaveFrequency.Text = setup.saveFrequency.ToString();

        LoadUnitPicker(DepthPicker, setup.Depth);
        LoadUnitPicker(SpeedPicker, setup.Speed);
        LoadUnitPicker(WindSpeedPicker, setup.WindSpeed);

        SizeChanged += (_, _) => UpdateResponsiveLayout();
        loading = false;
        ApplyTheme();
    }

    private static void LoadUnitPicker(Picker picker, UnitOfMeasure unit)
    {
        foreach (var item in unit.UnitList)
        {
            picker.Items.Add(item.Name);
        }

        picker.SelectedItem = unit.SelectedUnit;
    }

    private void UpdateResponsiveLayout()
    {
        // Use the actual view width rather than physical display pixels. This also
        // behaves correctly in a resized Windows window.
        var useLandscape = Width >= 760;
        if (useLandscape == landscapeLayout && SetupRoot.RowDefinitions.Count > 0)
        {
            return;
        }

        landscapeLayout = useLandscape;
        SetupRoot.RowDefinitions.Clear();
        SetupRoot.ColumnDefinitions.Clear();

        if (useLandscape)
        {
            SetupRoot.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            SetupRoot.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            SetupRoot.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            SetupRoot.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            SetupRoot.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            Place(NetworkCard, 0, 0);
            Place(UnitsCard, 0, 1);
            Place(DisplayCard, 1, 0);
            Place(GpsCard, 1, 1);
            Place(LoggingCard, 2, 0);
            Grid.SetColumnSpan(LoggingCard, 1);
            Place(SaveButton, 2, 1);
            Grid.SetColumnSpan(SaveButton, 1);
            SaveButton.VerticalOptions = LayoutOptions.Fill;
        }
        else
        {
            SetupRoot.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            for (var i = 0; i < 6; i++)
            {
                SetupRoot.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            Place(NetworkCard, 0, 0);
            Place(UnitsCard, 1, 0);
            Place(DisplayCard, 2, 0);
            Place(GpsCard, 3, 0);
            Place(LoggingCard, 4, 0);
            Place(SaveButton, 5, 0);
            SaveButton.VerticalOptions = LayoutOptions.Center;
        }
    }

    private static void Place(BindableObject view, int row, int column)
    {
        Grid.SetRow(view, row);
        Grid.SetColumn(view, column);
        Grid.SetColumnSpan(view, 1);
    }

    private void UnitPickerChanged(object? sender, EventArgs e)
    {
        if (loading)
        {
            return;
        }

        if (DepthPicker.SelectedItem is string depth)
        {
            setup.Depth.SelectedUnit = depth;
        }
        if (SpeedPicker.SelectedItem is string speed)
        {
            setup.Speed.SelectedUnit = speed;
        }
        if (WindSpeedPicker.SelectedItem is string windSpeed)
        {
            setup.WindSpeed.SelectedUnit = windSpeed;
        }

        Save();
    }

    private void ToggleSwitch(object? sender, ToggledEventArgs e)
    {
        if (!loading)
        {
            Save();
        }
    }

    public void SaveButtonClicked(object? sender, EventArgs e) => Save();

    public void Save()
    {
        if (loading)
        {
            return;
        }

        if (int.TryParse(Port.Text, out var port) && port is > 0 and <= 65535)
        {
            setup.Port = port;
        }
        else
        {
            Port.Text = setup.Port.ToString();
        }

        if (int.TryParse(SaveFrequency.Text, out var frequency) && frequency > 0)
        {
            setup.saveFrequency = frequency;
        }
        else
        {
            SaveFrequency.Text = setup.saveFrequency.ToString();
        }

        setup.Night = Night.IsToggled;
        setup.KeepActive = KeepActive.IsToggled;
        setup.UseGPSPOS = UseGPSPOS.IsToggled;
        setup.UseGPSHEADING = UseGPSHEADING.IsToggled;
        setup.UseGPSSOG = UseGPSSOG.IsToggled;

        setup.Save();
        DeviceDisplay.KeepScreenOn = setup.KeepActive;

        var parentPage = GetParentPage();
        parentPage?.SetColorScheme(setup);
        ApplyTheme();
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

    private void ApplyTheme()
    {
        var pageBackground = setup.Night ? Color.FromArgb("#050505") : Color.FromArgb("#F4F7FB");
        var cardBackground = setup.Night ? Color.FromArgb("#111111") : Colors.White;
        var cardStroke = setup.Night ? Color.FromArgb("#5C1A1A") : Color.FromArgb("#D7E0EA");
        var primary = setup.Night ? Color.FromArgb("#D94A4A") : Color.FromArgb("#0B5CAD");
        var text = setup.Night ? Color.FromArgb("#E7A0A0") : Color.FromArgb("#10233D");
        var inputBackground = setup.Night ? Color.FromArgb("#1B1111") : Color.FromArgb("#F7F9FC");

        BackgroundColor = pageBackground;
        SetupRoot.BackgroundColor = pageBackground;

        foreach (var card in new[] { NetworkCard, UnitsCard, DisplayCard, GpsCard, LoggingCard })
        {
            card.BackgroundColor = cardBackground;
            card.Stroke = cardStroke;
            ApplyTextColor(card.Content, text);
        }

        foreach (var entry in new[] { Port, SaveFrequency })
        {
            entry.BackgroundColor = inputBackground;
            entry.TextColor = text;
        }

        foreach (var picker in new[] { DepthPicker, SpeedPicker, WindSpeedPicker })
        {
            picker.BackgroundColor = inputBackground;
            picker.TextColor = text;
        }

        foreach (var toggle in new[] { Night, KeepActive, UseGPSPOS, UseGPSHEADING, UseGPSSOG })
        {
            toggle.OnColor = primary;
            toggle.ThumbColor = setup.Night ? Color.FromArgb("#FFD0D0") : Colors.White;
        }

        SaveButton.BackgroundColor = primary;
        SaveButton.TextColor = setup.Night ? Color.FromArgb("#FFF0F0") : Colors.White;
    }

    private static void ApplyTextColor(IView? view, Color color)
    {
        if (view == null) return;
        if (view is Label label) label.TextColor = color;

        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                ApplyTextColor(child, color);
            }
        }
    }

    private void OnSwipeLeft(object sender, SwipedEventArgs e) => GetParentPage()?.NextPage();
    private void OnSwipeRight(object sender, SwipedEventArgs e) => GetParentPage()?.PrevPage();

    public void OnAppEvent(string eventName, Record record, List<FieldData> dataPoints) { }

    public void OnReSize() => UpdateResponsiveLayout();

    public void OnSetupChanged(Setup settings)
    {
        loading = true;
        Night.IsToggled = settings.Night;
        KeepActive.IsToggled = settings.KeepActive;
        UseGPSPOS.IsToggled = settings.UseGPSPOS;
        UseGPSHEADING.IsToggled = settings.UseGPSHEADING;
        UseGPSSOG.IsToggled = settings.UseGPSSOG;
        DepthPicker.SelectedItem = settings.Depth.SelectedUnit;
        SpeedPicker.SelectedItem = settings.Speed.SelectedUnit;
        WindSpeedPicker.SelectedItem = settings.WindSpeed.SelectedUnit;
        loading = false;
        ApplyTheme();
    }
}
