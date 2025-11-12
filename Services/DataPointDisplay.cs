



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
        public double width;
        public double height;

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

        public Rect GetAbsoluteLocation(VisualElement element)
        {
            double x = element.X;
            double y = element.Y;
            double width = element.Width;
            double height = element.Height;
            var window = element.GetVisualElementWindow();
            Rect rect = new Rect(window.X,window.Y,window.Width,window.Height);

            


            /*Element parent = element.Parent;
            while (parent is VisualElement parentView)
            {
                window = parentView.GetVisualElementWindow();
                rect = new Rect(window.X, window.Y, window.Width, window.Height);
                parent = parentView.Parent;
            }*/


            if (rect.X < 0)
            {

                rect.X = 0;
            }
            if (rect.Y < 0)
            {
                rect.Y = 0;
            }
            return rect;
        }

        public void Draw(ICanvas Canvas, RectF DirtyRect)
        {
            canvas = Canvas;
            
            dirtyRect = DirtyRect;
            var position = dirtyRect;// GetAbsoluteLocation(graphicsView);

            //canvas.Translate((float)position.X, (float)position.Bottom);

            


            try
            {
                canvas.SaveState();
                canvas.Translate(position.X,position.Y);
                canvas.Scale(.9f, .8f);
                canvas.FontColor = setup.foreColor;
                canvas.Font = fonts[0];
                canvas.FontSize = 18;

                string txt = string.Empty;

                var textSize = canvas.GetStringSize("M", fonts[0], 18);

                
                canvas.DrawString(description, 1,textSize.Height, HorizontalAlignment.Left);

                canvas.FontSize = 72;
                
                txt = fieldData.current.ToString($"{precision}");
                canvas.DrawString(txt, (float)(width/2), (float)(height*.2), HorizontalAlignment.Center);
                textSize = canvas.GetStringSize("M", fonts[0], 72);

                canvas.FontSize = 18;

                txt = fieldData.Min.ToString($"{precision}") + " - " + fieldData.Max.ToString($"{precision}");
                canvas.DrawString(txt, (float)(width / 2), (float)(height * .2 + textSize.Height), HorizontalAlignment.Center);


                if (fieldData.DataPoints.Count < 2 || fieldData.Max == fieldData.Min)
                {
                    // Not enough data to draw
                    canvas.ResetState();
                    return;
                }
                
                //canvas.FillColor = Colors.White;
                canvas.FillColor = new Color(0, 0, 0, 0.3f);
                
                
                

                float MaxY = (float)fieldData.Max * 1.1f;
                int MY = (int)MaxY;
                int scaleMult =0;
                // scaling the Y axis
                if (MY == 0)
                {
                    MY = 1;
                }
                else
                {
                    while (MY > 10)
                    {
                        MY = MY / 10;
                        scaleMult++;
                    }
                    if (MY < 5)
                    {
                        MY++;
                    }
                    else
                    {
                        MY = 10;
                        if (scaleMult > 1)
                        {
                            scaleMult--;
                        }
                    }
                    if (scaleMult > 0)
                    {
                        MY = (int)Math.Pow(10, scaleMult) * MY;
                        MaxY = MY;
                    }
                    else
                    {
                        MaxY = MY;
                    }
                }
                bool invertY = false;
                if (name == "DPT")
                {
                    invertY = true;
                }
                float MinY =  (float)fieldData.Min * 0.9f;
                //float rangeY = MaxY - MinY;
                float yMult = (float)(position.Bottom - position.Top)  / MaxY;
                
                canvas.StrokeColor = Colors.DarkGray;
                int yStep = MY / (int)Math.Pow(10, scaleMult);

                
                if (scaleMult > 1)
                {
                    yStep = yStep * (int)Math.Pow(10, scaleMult - 1);
                }
                if (yStep == MY / (int)Math.Pow(10, scaleMult) && yStep>=10)
                {
                    yStep = yStep / 10;
                }
                canvas.StrokeSize = 2;
                int i;

                
                for (i = 0; i < MaxY; i = i + yStep)
                {
                    canvas.DrawLine(0, position.Bottom - (float)i * yMult, (float)position.Right, position.Bottom - (float)i * yMult);
                    if (invertY)
                    {
                        canvas.DrawString((i).ToString(), 0, (float)i * yMult, HorizontalAlignment.Left);
                    }
                    else
                    {
                        canvas.DrawString((i).ToString(), 0, position.Bottom - (float)i * yMult, HorizontalAlignment.Left);
                    }
                }
                

                canvas.StrokeColor = Colors.Blue;
                canvas.StrokeSize = 6;

                int xStep = 1;

                if (position.Right -position.Left > fieldData.DataPoints.Count)
                { 
                    while (fieldData.DataPoints.Count > (position.Right - position.Left) * xStep)
                    {
                        xStep++;
                    }
                }

                float xMult = (float)(position.Right - position.Left) / (float)(fieldData.DataPoints.Count()-1);
                
                float lastY = 0;

                if (!invertY)
                {
                    lastY = (float)(position.Bottom - ((float)(fieldData.DataPoints[0].value) * yMult)+position.Top);
                }
                else
                {
                    lastY = ((float)(fieldData.DataPoints[0].value - MinY) * yMult)+(float)position.Top;
                }


                    float lastX = 0;
                float curY;
                for ( i = xStep; i < fieldData.DataPoints.Count - 1; i += xStep)
                {
                    float curX = i *  xMult;
                    if (!invertY)
                    {

                        // curY = (float)position.Bottom - ((float)(fieldData.DataPoints[i].value - MinY) * yMult);
                        curY = (float)(position.Bottom - ((float)(fieldData.DataPoints[i].value) * yMult) + position.Top);
                    }
                    else
                    {
                        //curY = dirtyRect.Height - ((float)(fieldData.DataPoints[i].value - MinY) * yMult);
                        curY =  ((float)(fieldData.DataPoints[i].value - MinY) * yMult);
                    }
                    
                    canvas.DrawLine(lastX,  lastY, curX, curY);
                    lastX = curX;
                    lastY = curY;

                }
                canvas.ResetState();
                
                


                

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
