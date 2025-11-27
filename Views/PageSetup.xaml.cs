

using SailMonitor.Models;

namespace SailMonitor;

public partial class PageSetup : ContentView
{
    private Record record = new Record();
    private Setup setup;
    public bool loading;

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
        
        
        BindCollectionView(WindSpeedList, setup.WindSpeed);
        BindCollectionView(DepthList, setup.Depth);
        BindCollectionView(SpeedList, setup.Speed);
        loading = false;
    }



    public void BindCollectionView(CollectionView collectionView, UnitOfMeasure unitOfMeasure)
    {
        List<string> items = new List<string>();
        foreach (var item in unitOfMeasure.UnitList)
        {
            items.Add(item.Name);
        }
        collectionView.ItemsSource = items;
        var selected = items.FirstOrDefault(i => i == unitOfMeasure.SelectedUnit);
        collectionView.SelectedItem = selected;
        //collectionView.IsVisible = false;
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


    private void WindSpeedChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as string;
        if (selectedItem != null)
        {
            setup.WindSpeed.SelectedUnit = selectedItem;
            
            
        }
    }


    private void DepthChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as string;
        var collectionView = sender as CollectionView;
        if (selectedItem != null)
        {
            setup.Depth.SelectedUnit = selectedItem;
            collectionView.SelectedItem = selectedItem;

        }

        if (sender is Label lbl)
        {
            int i = 0;
        }
    }

    private void SpeedChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as string;
        if (selectedItem != null)
        {
            setup.Speed.SelectedUnit = selectedItem;
            
           
        }
    }

    private void OnItemLabelLoadedDepth(object sender, EventArgs e)
    {
        if (sender is Label lbl)
        {
            if (lbl.Text == setup.Depth.SelectedUnit)
            {
                lbl.TextColor = setup.backColor;
                lbl.BackgroundColor = setup.foreColor;
            }
            else
            {
                lbl.TextColor = setup.foreColor;
                lbl.BackgroundColor = setup.backColor;
            }
        }
    }

    private void OnItemLabelLoadedSpeed(object sender, EventArgs e)
    {
        if (sender is Label lbl)
        {
            if (lbl.Text == setup.Speed.SelectedUnit)
            {
                lbl.TextColor = setup.backColor;
                lbl.BackgroundColor = setup.foreColor;
            }
            else
            {
                lbl.TextColor = setup.foreColor;
                lbl.BackgroundColor = setup.backColor;
            }
        }
    }

    private void OnItemLabelLoadedWindSpeed(object sender, EventArgs e)
    {
        if (sender is Label lbl)
        {
            if (lbl.Text == setup.Speed.SelectedUnit)
            {
                lbl.TextColor = setup.backColor;
                lbl.BackgroundColor = setup.foreColor;
            }
            else
            {
                lbl.TextColor = setup.foreColor;
                lbl.BackgroundColor = setup.backColor;
            }
        }
    }
}