
using SailMonitor.Models;

namespace SailMonitor;

public partial class PageSetup : ContentView
{
    private Record record = new Record();
    private Setup setup;

    public PageSetup(Setup _setup)
    {
        InitializeComponent();
        BindingContext = this;
        setup = _setup;

        Port.Text = setup.Port.ToString();
        Night.IsChecked = setup.Night;
        KeepActive.IsChecked = setup.KeepActive;
        UseGPSPOS.IsChecked = setup.UseGPSPOS;
        UseGPSHEADING.IsChecked = setup.UseGPSHEADING;
        UseGPSSOG.IsChecked = setup.UseGPSSOG;
        SaveFrequency.Text = setup.saveFrequency.ToString();
        WindSpeedbutton.Text = setup.WindSpeed.SelectedUnit;
        BindCollectionView(WindSpeedList,setup.WindSpeed);




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
        collectionView.IsVisible = false;


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

    public void WindSpeedbutton_Clicked(object sender, EventArgs e)
    {
        if(WindSpeedList.IsVisible)
        {
            WindSpeedList.IsVisible = false;
        }
        else
        {
            WindSpeedList.IsVisible = true;
            WindSpeedbutton.IsVisible = false;
        }
    }

    private void WindSpeedChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as string;
        if (selectedItem != null)
        {
            
            WindSpeedbutton.Text = selectedItem;
            WindSpeedList.IsVisible = false;
            WindSpeedbutton.IsVisible = true;
        }
    }

}