namespace SailMonitor.Services
{
    using SailMonitor.Models;

    public interface IContentViewHost
    {
        void OnAppEvent(string eventName, Record record, List<FieldData> DataPoints);

        void OnReSize();

        void OnSetupChanged(Setup settings);
    }
}
