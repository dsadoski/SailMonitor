using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Models
{
    public class FieldData
    {   
        public List<SingleDataPoint> DataPoints { get; set; }

        public string name { get; set; } = "";

        public FieldData(string Name)
        {   
            name = Name;
            DataPoints = new List<SingleDataPoint>();
        }

        public void AddDataPoint(double value)
        {
            DataPoints.Add(new SingleDataPoint(value));
        }
    }
}
