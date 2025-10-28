using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Models
{
    public class SingleDataPoint
    {   
        public double Value { get; set; }

        public DateTime dateTime { get; set; }



        public SingleDataPoint(double value)
        {
            
            Value = value;
            dateTime = DateTime.Now;
        }
    }
}
