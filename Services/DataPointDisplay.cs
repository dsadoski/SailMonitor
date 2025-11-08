

using Microsoft.Maui;
using Microsoft.Maui.Layouts;
using SailMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Services
{
    public class DataPointDisplay : IDrawable
    {
        public FieldData fieldData;
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
        public string description;
        Setup setup;

        public DataPointDisplay(string Name,string Precision, string Description)
        {
            
            name = Name;
            precision = Precision;
            description = Description;
            fieldData = new FieldData(Name);
            fonts = new List<Microsoft.Maui.Graphics.Font>();
            setup = new Setup();
            fonts.Add(new Microsoft.Maui.Graphics.Font("OpenSansRegular"));
            fonts.Add(new Microsoft.Maui.Graphics.Font("OpenSansBold"));

        }

        public void Draw(ICanvas Canvas, RectF DirtyRect)
        {
            canvas = Canvas;
            dirtyRect = DirtyRect;


            try
            {
                if(fieldData.DataPoints.Count < 2)
                {
                    // Not enough data to draw
                    return;
                }
                canvas.SaveState();
                //canvas.FillColor = Colors.White;
                canvas.FillColor = new Color(0, 0, 0, 0.3f);
                canvas.StrokeColor = Colors.Blue;
                canvas.StrokeSize = 2;
                //canvas.FillRectangle(dirtyRect);

                float MaxY = (float)fieldData.Max * 1.1f;
                float MinY = 0;// (float)fieldData.Min * 0.9f;
                float rangeY = MaxY - MinY;
                float yMult = dirtyRect.Height*.8f / rangeY;
                int xStep = 1;

                if (dirtyRect.Width > fieldData.DataPoints.Count)
                {
                    while (fieldData.DataPoints.Count > dirtyRect.Width * xStep)
                    {
                        xStep++;
                    }
                }

                float lastY = dirtyRect.Height - ((float)(fieldData.DataPoints[0].value - MinY) * yMult);
                float lastX = 0;

                for (int i = xStep; i < fieldData.DataPoints.Count - 1; i += xStep)
                {
                    float curX = i;
                    float curY = dirtyRect.Height - ((float)(fieldData.DataPoints[i].value - MinY) * yMult);
                    canvas.DrawLine(lastX, (dirtyRect.Height * .1f) + lastY, curX, (dirtyRect.Height * .1f)+curY);
                    lastX = curX;
                    lastY = curY;

                }
                canvas.FontColor = setup.foreColor;
                canvas.Font = fonts[0];
                canvas.FontSize = 18;
                string txt = fieldData.Min.ToString($"F{precision}");
                canvas.DrawString(txt, 1, (float)(dirtyRect.Height * 0.9f),HorizontalAlignment.Left);

                
                canvas.DrawString(description, 1,50, HorizontalAlignment.Left);


                canvas.RestoreState();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DataPointDisplay.Draw: {ex.Message}");
            }
            
        }

        public void UpdateUI()
        {
           
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            // width & height are in raw pixels
            double width = displayInfo.Width /displayInfo.Density;
            double height = displayInfo.Height / displayInfo.Density;

            AbsoluteLayout.SetLayoutBounds(center, new Rect(width * .4, height *.4, -1, -1));
            
            AbsoluteLayout.SetLayoutBounds(topLeft, new Rect(1, 1, -1, -1));


            topLeft.Text = name;
           

            bottomLeft.Text = string.Format($"{{0:{precision}}}", fieldData.Min);
            bottomRight.Text = string.Format($"{{0:{precision}}}", fieldData.Max);
            center.Text = string.Format($"{{0:{precision}}}", fieldData.current);
            
        }
    }
}
