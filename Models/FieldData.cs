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

        Setup setup { get; set; }

        public FieldData(string Name)
        {   
            name = Name;
            DataPoints = new List<SingleDataPoint>();
            setup = new Setup();    

        }

        public void AddDataPoint(double value)
        {
            current = value;
            if (DataPoints.Count == 0)
            {
                Max = value;
                Min = value;
            }
            else
            {
                if (value > Max)
                {
                    Max = value;
                }
                if (value < Min)
                {
                    Min = value;
                }
            }
            if (DataPoints.Count > 0)
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - DataPoints[DataPoints.Count - 1].dateTime.Ticks);
                if (Math.Abs(timeSpan.TotalSeconds) >= setup.saveFrequency)
                {
                    DataPoints.Add(new SingleDataPoint(value));
                }
            }
            else
            {
                DataPoints.Add(new SingleDataPoint(value));
            }

            DateTime dateTime = DateTime.Now.AddHours(-1);
            int datapointCount = DataPoints.Count;
            DataPoints = DataPoints.Where(d => d.dateTime.Ticks > dateTime.Ticks).ToList();
            
                Max = DataPoints.Max(x => x.value);
            Min = DataPoints.Min(x => x.value);
            

        }
    }
}
