using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Models
{
    class UnitOfMeasure
    {
        public string Name;
        public string Internal;
        public string Selected;
        public List<Unit> UnitList;

        public UnitOfMeasure() 
        { 
        }
    }
}
