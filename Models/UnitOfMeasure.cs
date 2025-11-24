using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Media.Animation;

namespace SailMonitor.Models
{
    public class UnitOfMeasure
    {
        public string Name;
        public string Internal;
        public List<Unit> UnitList;
        public string SelectedUnit;
        public double ConvertToInternal(string fromUnit, double value)
        {
            var from = UnitList.Find(u => u.Name == fromUnit);
            var to = UnitList.Find(u => u.Name == Internal);
            if (from != null && to != null)
            {
                return value * (to.Conversion / from.Conversion);
            }
            return value;
        }

        public double ConvertToDisplay(string toUnit,  double value)
        {
            var from = UnitList.Find(u => u.Name == Internal);
            var to = UnitList.Find(u => u.Name == toUnit);
            if (from != null && to != null)
            {
                return value * (to.Conversion / from.Conversion);
            }
            return value;
        }
        public UnitOfMeasure()
        {
        }
    }
}
