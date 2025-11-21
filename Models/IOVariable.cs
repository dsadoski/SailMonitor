namespace SailMonitor.Models
{
    public class IOVariable
    {

        public double displayValue;
        public double internalValue;
        public string uom;
        public string internaluom;

        public IOVariable()
        {
            displayValue = 0.0;
            internalValue = 0.0;
            uom = string.Empty;
            internaluom = string.Empty;
        }


    }
}
