
using SailMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Services
{
    internal class DataPointDisplay : IDrawable
    {
        FieldData fieldData;
        public string name = "";
        string precision = "F2";


        public DataPointDisplay(string Name,string Precision)
        {
            
            name = Name;
            precision = Precision;
            fieldData = new FieldData(Name);
            
        }

        public void AddDataPoint(double value)
        {
            fieldData.AddDataPoint(value);
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Draw the line of history

            // draw the title

            // draw the latest value

            throw new NotImplementedException();
        }
    }
}
