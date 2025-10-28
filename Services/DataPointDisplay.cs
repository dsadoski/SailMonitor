
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
            try
            {
                float width = dirtyRect.Width;
                float height = dirtyRect.Height;
                float centerX = width / 2;
                float centerY = height / 2;
                canvas.SaveState();
                canvas.FillColor = Colors.Transparent;
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 2;
                canvas.FillRectangle(dirtyRect);
                canvas.RestoreState();
                canvas.DrawString($"AWS", 0,100,dirtyRect.Width *.4f, dirtyRect.Height*.2f, HorizontalAlignment.Left,VerticalAlignment.Top);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DataPointDisplay.Draw: {ex.Message}");
            }
            //canvas.Translate(centerX, centerY);

            // Draw the line of history

            // draw the title

            // draw the latest value


        }
    }
}
