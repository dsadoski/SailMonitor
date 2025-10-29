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

        public double Max { get; set; }
        public double Min { get; set; }

        public double current;

        public FieldData(string Name)
        {   
            name = Name;
            DataPoints = new List<SingleDataPoint>();

        }

        public void AddDataPoint(double value)
        {
            current = value;
            if (DataPoints.Count ==0)
            {
                Max = value;
                Min = value;
            }
            else
            {
                if(value > Max)
                {
                    Max = value;
                }
                if(value < Min)
                {
                    Min = value;
                }
            }
            DataPoints.Add(new SingleDataPoint(value));
        }
    }
}
