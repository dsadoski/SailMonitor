namespace SailMonitor.Models
{
    public class SingleDataPoint
    {
        public double value { get; set; }

        public DateTime dateTime { get; set; }

        public SingleDataPoint(double Value)
        {
            value = Value;
            dateTime = DateTime.Now;
        }
    }
}
