using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
