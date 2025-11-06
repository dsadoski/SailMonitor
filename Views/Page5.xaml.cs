
using SailMonitor.Services;
using SailMonitor.Models;
namespace SailMonitor;

public partial class Page4 : ContentView, IContentViewHost
{
    private Record record = new Record();
    

    public Page4()
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
        

        /*VoltageLabel.Text = $"{record.voltage:F2} V";
        WaterTempLabel.Text = $"{record.waterTemp:F2} °C";*/
    }

    public void onSubmit(object sender, EventArgs e)
    {
       


    }
}