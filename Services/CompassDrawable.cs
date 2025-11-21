using SailMonitor.Models;

namespace SailMonitor.Services
{
    public class CompassDrawable : IDrawable
    {
        public float RotationDegrees { get; set; } = 0f; // For rotating compass if needed

        public float TrueWind = 100;
        public float ApparentWind = 85;
        public float Heading = 0;
        public Setup setup;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (dirtyRect.Width < 1 || dirtyRect.Height < 1) return;
            try
            {
                setup = new Setup();

                float centerX = dirtyRect.Width / 2;
                float centerY = dirtyRect.Height / 2;
                float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 * 0.7f;

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

                canvas.StrokeColor = setup.foreColor;
                canvas.StrokeSize = 2;

                // Draw main circle
                canvas.DrawCircle(0, 0, radius);

                // Draw degree markers every 30°  draw it with coarse up
                int degCount = 0;
                for (int deg = 0; deg < 360; deg += 5)
                {
                    float rad = (deg - Heading) * (float)Math.PI / 180f;
                    float inner;
                    if (degCount == 45)
                    {
                        inner = radius * 0.85f;
                    }
                    else
                    {
                        inner = radius * 0.95f;
                    }

                    float outer = radius;
                    float x1 = inner * (float)Math.Sin(rad);
                    float y1 = -inner * (float)Math.Cos(rad);
                    float x2 = outer * (float)Math.Sin(rad);
                    float y2 = -outer * (float)Math.Cos(rad);
                    canvas.DrawLine(x1, y1, x2, y2);

                    // Draw degree numbers
                    canvas.FontColor = setup.foreColor;
                    canvas.FontSize = radius * 0.08f;

                    if (degCount == 45 || degCount == 0)
                    {
                        canvas.DrawString($"{deg}°", x1 * 1.4f, y1 * 1.4f, HorizontalAlignment.Center);
                        degCount = 0;
                    }

                    degCount += 5;
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

                    if (deg > 180)
                    {
                        displayDeg = 360 - deg;
                    }
                    else
                    {
                        displayDeg = deg;
                    }

                    // Draw degree numbers
                    canvas.FontColor = setup.foreColor;
                    canvas.FontSize = radius * 0.08f;
                    canvas.DrawString($"{displayDeg}°", x1 * .8f, y1 * .8f, HorizontalAlignment.Center);
                    //(, x1 * 1.1f, y1 * 1.1f, HorizontalAlignment.Center, VerticalAlignment.Center);
                }

                canvas.StrokeLineCap = LineCap.Round;

                DrawHeading(canvas, Colors.DarkGray, 2, radius, 0);
                /*DrawHeading(canvas, Colors.Red, 15, radius, ApparentWind);
                DrawHeading(canvas, Colors.Green, 15, radius, TrueWind);*/
                DrawHeadingWedge(canvas, Colors.Blue, radius, TrueWind, 10);
                if (ApparentWind > 180)
                {
                    DrawHeadingWedge(canvas, Colors.Red, radius, ApparentWind, 10);
                }
                else
                {
                    DrawHeadingWedge(canvas, Colors.Green, radius, ApparentWind, 10);
                }

                canvas.RestoreState();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CompassDrawable.Draw: {ex.Message}");
            }
        }

        public void DrawHeading(ICanvas canvas, Color color, float size, float radius, float degree)
        {
            canvas.StrokeColor = color;
            canvas.StrokeSize = size;
            float rad = degree * (float)Math.PI / 180f;

            float x2 = radius * (float)Math.Sin(rad);
            float y2 = -radius * (float)Math.Cos(rad);
            canvas.DrawLine(0, 0, x2, y2);
        }

        public void DrawHeadingWedge(ICanvas canvas, Color color, float radius, float centerDegree, float sweepDegrees = 5f)
        {
            try
            {
                // paint
                canvas.FillColor = color.WithAlpha(0.5f);
                canvas.StrokeColor = Colors.Transparent;
                canvas.StrokeSize = 0;

                float half = sweepDegrees / 2f;
                float startDeg = centerDegree - half;
                float endDeg = centerDegree + half;
                //radius = radius * 1.1f;

                float startRad = startDeg * (float)Math.PI / 180f;
                float endRad = endDeg * (float)Math.PI / 180f;
                float midRad = centerDegree * (float)Math.PI / 180f;

                // Points on the outer circle (note: y uses negative cos to have 0° = up)
                var pEnd = new PointF(radius * (float)Math.Sin(startRad), -radius * (float)Math.Cos(startRad));
                var pMid = new PointF(radius * 1.1f * (float)Math.Sin(midRad), -radius * 1.1f * (float)Math.Cos(midRad));
                var pStart = new PointF(radius * (float)Math.Sin(endRad), -radius * (float)Math.Cos(endRad));

                var path = new PathF();

                // center -> outer start -> arc to outer end -> back to center
                path.MoveTo(0, 0);
                path.LineTo(pStart.X, pStart.Y);
                path.LineTo(pMid.X, pMid.Y);
                path.LineTo(pEnd.X, pEnd.Y);
                path.MoveTo(0, 0);

                // AddArc draws an elliptical arc from pStart to pEnd with radii rx,ry.
                // The 'clockwise' boolean selects the short/long arc direction.
                //path.AddArc(pStart, pEnd, startDeg, endDeg, true);

                path.Close();

                canvas.FillPath(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
