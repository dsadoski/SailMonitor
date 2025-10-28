using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SailMonitor.Services
{
    public class CompassDrawable : IDrawable
    {
        public float RotationDegrees { get; set; } = 0f; // For rotating compass if needed
        public List<float> PieAngles { get; set; } = new List<float> { 0f, 45f, 90f }; // start angles of wedges
        public float PieWidthDegrees { get; set; } = 5f;

        public float TrueWind=100;
        public float ApparentWind=85;
        public float Heading=0;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float centerX = width / 2;
            float centerY = height / 2;
            float radius = Math.Min(width, height) / 2 * 0.9f;

            // Clear background (transparent)
            canvas.SaveState();
            canvas.FillColor = Colors.Transparent;
            //SetFillColor(Colors.Transparent);
            canvas.FillRectangle(dirtyRect);
            canvas.RestoreState();

            // Draw compass rose
            canvas.SaveState();
            canvas.Translate(centerX, centerY);
            //canvas.Rotate(RotationDegrees);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;

            // Draw main circle
            canvas.DrawCircle(0, 0, radius);

            // Draw degree markers every 30°  draw it with coarse up
            for (int deg = 0 ; deg < 360; deg += 30)
            {
                float rad  = (deg -Heading) * (float)Math.PI / 180f;
                float inner = radius * 0.9f; 
                float outer = radius;
                float x1 = inner * (float)Math.Sin(rad);
                float y1 = -inner * (float)Math.Cos(rad);
                float x2 = outer * (float)Math.Sin(rad);
                float y2 = -outer * (float)Math.Cos(rad);
                canvas.DrawLine(x1, y1, x2, y2);

                // Draw degree numbers
                canvas.FontColor = Colors.Black;
                canvas.FontSize = radius * 0.08f;
                canvas.DrawString($"{deg}°", x1 * 1.3f, y1 * 1.3f, HorizontalAlignment.Center);
                //(, x1 * 1.1f, y1 * 1.1f, HorizontalAlignment.Center, VerticalAlignment.Center);
            }

            //Draw deg relative to boat

            int displayDeg;
            for (int deg = 0; deg < 360; deg += 30)
            {
                float rad = (deg) * (float)Math.PI / 180f;
                float inner = radius * 0.9f;
                float outer = radius;
                float x1 = inner * (float)Math.Sin(rad);
                float y1 = -inner * (float)Math.Cos(rad);
                float x2 = outer * (float)Math.Sin(rad);
                float y2 = -outer * (float)Math.Cos(rad);
                
                if (deg> 180)
                {
                    displayDeg = 360 -deg ;
                }
                else
                {
                    displayDeg = deg;
                }

                // Draw degree numbers
                canvas.FontColor = Colors.Black;
                canvas.FontSize = radius * 0.08f;
                canvas.DrawString($"{displayDeg}°", x1 * .8f, y1 * .8f, HorizontalAlignment.Center);
                //(, x1 * 1.1f, y1 * 1.1f, HorizontalAlignment.Center, VerticalAlignment.Center);
            }


            DrawHeading(canvas, Colors.Blue, 10, radius, 0);
            DrawHeading(canvas, Colors.Red, 10,  radius, ApparentWind);
            DrawHeading(canvas, Colors.Green, 10, radius, TrueWind);

            
            canvas.RestoreState();
        }

        public void DrawHeading(ICanvas canvas, Color color, float size,  float radius, float degree)
        {
            canvas.StrokeColor = color;
            canvas.StrokeSize = size;
            float rad = degree * (float)Math.PI / 180f;
            
            
            float x2 = radius * (float)Math.Sin(rad);
            float y2 = -radius * (float)Math.Cos(rad);
            canvas.DrawLine(0, 0, x2, y2);
        }
    }
}
