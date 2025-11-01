
using SailMonitor.Services;
using SailMonitor.Models;
namespace SailMonitor;

public partial class Page3 : ContentView, IContentViewHost
{
    private Record record = new Record();
    public Page3()
	{
		InitializeComponent();
	}

    public void OnAppEvent(string eventName, Record data)
    {
        record = data.Copy();
        UpdateUI();
        if (eventName == "RefreshData")
        {
            // handle event
            //RefreshData((MyDataModel)data!);
        }
    }

    public void UpdateUI()
    {
        latitudeLabel.Text = $"{record.latitude:F6} °";
        longitudeLabel.Text = $"{record.longitude:F6} °";
        DepthLabel.Text = $"{record.depth:F2} ft";
        HeadingMagLabel.Text = $"{record.headingMag:F0} °";
        headingTrueLabel.Text = $"{record.headingTrue:F0} °";
        SOGLabel.Text = $"{record.SOG:F2}";
        COGLabel.Text = $"{record.COG:F2}";
        WaterTempLabel.Text = $"{record.waterTemp:F1}";

        /*VoltageLabel.Text = $"{record.voltage:F2} V";
        WaterTempLabel.Text = $"{record.waterTemp:F2} °C";*/
    }
}