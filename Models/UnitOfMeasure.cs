

namespace SailMonitor.Models
{
    public class UnitOfMeasure
    {
        public string Name;
        public string Internal;
        public List<Unit> UnitList;
        public string SelectedUnit;

        public double ConvertToInternal(double value)
        {
            return ConvertToInternal(SelectedUnit, value);
        }

        public double ConvertToInternal(string fromUnit, double value)
        {
            var from = UnitList.Find(u => u.Name == fromUnit);
            var to = UnitList.Find(u => u.Name == Internal);
            if (from != null && to != null)
            {
                return value * from.Conversion;
            }
            return value;
        }

        public double ConvertToDisplay(double value)
        {
            
            var to = UnitList.Find(u => u.Name == SelectedUnit);
            if (to != null )
            {
                return value / to.Conversion;
            }
            return value;
        }

        public UnitOfMeasure()
        {
        }
    }
}
