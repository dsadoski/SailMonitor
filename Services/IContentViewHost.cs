using SailMonitor.Models;

namespace SailMonitor.Services
{
    public interface IContentViewHost
    {
        void OnAppEvent(string eventName, Record record, List<FieldData> DataPoints);

        void OnReSize();

        void OnSetupChanged(Setup settings);
    }
}
