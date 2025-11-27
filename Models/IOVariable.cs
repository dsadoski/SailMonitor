namespace SailMonitor.Models
{
    public class IOVariable
    {
        public double displayValue;
        public double internalValue;
        public string uom;
        public string internaluom;

        public IOVariable(string Internaluom, string Uom)
        {
            displayValue = 0.0;
            internalValue = 0.0;
            uom = Uom;
            internaluom = Internaluom;
        }

        public IOVariable Copy()
        {
            IOVariable copy = new IOVariable(this.internaluom, this.uom);
            copy.displayValue = this.displayValue;
            copy.internalValue = this.internalValue;
            return copy;
        }
    }
}
