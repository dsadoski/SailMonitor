

using Android.Security.Identity;
using Microsoft.Maui;
using SailMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Services
{
    internal class DataPointDisplay : IDrawable
    {
        FieldData fieldData;
        public string name = "";
        string precision = "F2";
        private List<Microsoft.Maui.Graphics.Font> fonts;
        private ICanvas canvas;
        RectF dirtyRect;
        public Label topLeft;
        public Label bottomLeft;
        public Label topRight;
        public Label bottomRight;
        public Label center;
        public GraphicsView graphicsView;

        public DataPointDisplay(string Name,string Precision)
        {
            
            name = Name;
            precision = Precision;
            fieldData = new FieldData(Name);
            fonts = new List<Microsoft.Maui.Graphics.Font>();
            fonts.Add(new Microsoft.Maui.Graphics.Font("OpenSansRegular"));
            fonts.Add(new Microsoft.Maui.Graphics.Font("OpenSansBold"));

        }

        public void AddDataPoint(double value)
        {
            if (fieldData.DataPoints.Count > 0)
            {
                if(Math.Abs(new TimeSpan(DateTime.Now.Ticks - fieldData.DataPoints[fieldData.DataPoints.Count-1].dateTime.Ticks).TotalSeconds) >= 5)
                {
                    fieldData.AddDataPoint(value);
                    graphicsView.Invalidate();
                }
            }
            else
            {
                fieldData.AddDataPoint(value);
                graphicsView.Invalidate();
            }
        }
        public void Draw(ICanvas Canvas, RectF DirtyRect)
        {
            canvas = Canvas;
            dirtyRect = DirtyRect;

            try
            {
               
                canvas.SaveState();
                canvas.FillColor = Colors.Transparent;
                canvas.StrokeColor = Colors.Blue;
                canvas.StrokeSize = 2;
                canvas.FillRectangle(dirtyRect);
                canvas.RestoreState();
                canvas.FontSize = 12;
                


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

        public void UpdateUI()
        {
            topLeft.Text = name;
            bottomLeft.Text = string.Format($"{{0:{precision}}}", fieldData.Min);
            bottomRight.Text = string.Format($"{{0:{precision}}}", fieldData.Max);
            center.Text = string.Format($"{{0:{precision}}}", fieldData.current);
        }

        public void DrawString(float x, float y, string font, float fontSize, string text)
        {
            DrawString(x, y, font, fontSize, text, HorizontalAlignment.Left);
        }

        public void DrawString(float x, float y,  string font, float fontSize, string text, HorizontalAlignment hAlign)
        {
            var fontToUse = fonts.Where(f => f.Name == font).FirstOrDefault();

            if (fontToUse == null) return;

            canvas.Font = fontToUse;
            canvas.FontSize = fontSize;
            
            var size = canvas.GetStringSize("AWS", fontToUse, fontSize);

            if(x==-1)
            {
                x= dirtyRect.Width - size.Width -5;
            }
            else if (x == -2)
            {
                x = (dirtyRect.Width - size.Width) / 2;
            }

            if (y == -1)
            {
                y = dirtyRect.Height - size.Height;
            }
            else if (y == -2)
            {
                y = (dirtyRect.Height - size.Height) / 2;
            }



            canvas.DrawString(text, x, y, hAlign);
            


        }
    }
}
