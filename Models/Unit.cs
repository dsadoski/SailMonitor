namespace SailMonitor.Models
{
    public class Unit
    {
        public string Name;
        public double Conversion;
        public Unit(string name, double conversion)
        {
            Name = name;
            Conversion = conversion;
        }
    }
}
