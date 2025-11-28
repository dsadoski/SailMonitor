namespace SailMonitor.Services;

using SailMonitor.Models;

public class DataPointDisplay : IDrawable
{
    public FieldData FieldData;
    public string Name = string.Empty;
    public GraphicsView graphicsView;
    public string Description;
    private Setup setup;
    public double Width;
    public double Height;

    private string precision = "F2";
    private List<Microsoft.Maui.Graphics.Font> fonts;
    private ICanvas canvas;
    private RectF DirtyRect;
    public string unitOfMeasure;

    public DataPointDisplay(string Name, string Precision, string Description, string UnitofMeasure)
    {
        this.Name = Name;
        precision = Precision;
        this.Description = Description;
        unitOfMeasure = UnitofMeasure;
        FieldData = new FieldData(Name, unitOfMeasure);
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
        Rect rect = new Rect(window.X, window.Y, window.Width, window.Height);
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

        this.DirtyRect = DirtyRect;
        var position = this.DirtyRect;

        try
        {
            canvas.SaveState();
            canvas.Translate(position.X, position.Y);
            canvas.Scale(.9f, .8f);
            canvas.FontColor = setup.foreColor;
            canvas.Font = fonts[0];
            canvas.FontSize = 18;

            string txt = string.Empty;

            var textSize = canvas.GetStringSize("M", fonts[0], 18);

            canvas.DrawString(Description, 1, textSize.Height, HorizontalAlignment.Left);

            canvas.FontSize = 72;

            txt = FieldData.Current.ToString($"{precision}");
            canvas.DrawString(txt, (float)(Width / 2), (float)(Height * .05), HorizontalAlignment.Center);
            textSize = canvas.GetStringSize("M", fonts[0], 72);

            canvas.FontSize = 18;

            txt = FieldData.Min.ToString($"{precision}") + " - " + FieldData.Max.ToString($"{precision}");
            canvas.DrawString(txt, (float)(Width / 2), (float)((Height * .2) + textSize.Height), HorizontalAlignment.Center);

            if (FieldData.DataPoints.Count < 2 || FieldData.Max == FieldData.Min)
            {
                // Not enough data to draw
                canvas.ResetState();
                return;
            }

            // canvas.FillColor = Colors.White;
            canvas.FillColor = new Color(0, 0, 0, 0.3f);

            float MaxY = (float)FieldData.Max * 1.1f;
            int MY = (int)MaxY;
            int scaleMult = 0;

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
            if (Name == "DPT")
            {
                invertY = true;
            }

            float MinY = (float)FieldData.Min * 0.9f;

            float yMult = (float)(position.Bottom - position.Top) / MaxY;

            canvas.StrokeColor = Colors.DarkGray;
            int yStep = MY / (int)Math.Pow(10, scaleMult);

            if (scaleMult > 1)
            {
                yStep = yStep * (int)Math.Pow(10, scaleMult - 1);
            }

            if (yStep == MY / (int)Math.Pow(10, scaleMult) && yStep > 10)
            {
                yStep = yStep / 10;
            }
           
            canvas.StrokeSize = 2;
            int i;

            List<PointF> points = new List<PointF>();

            for (i = 0; i <= MaxY; i = i + yStep)
            {
                canvas.DrawLine(0, position.Bottom - ((float)i * yMult), (float)position.Right, position.Bottom - ((float)i * yMult));
                if (invertY)
                {
                    canvas.DrawString(i.ToString(), 0, (float)i * yMult, HorizontalAlignment.Left);
                }
                else
                {
                    canvas.DrawString(i.ToString(), 0, position.Bottom - ((float)i * yMult), HorizontalAlignment.Left);
                }
            }

            canvas.StrokeColor = Colors.Blue;
            canvas.StrokeSize = 6;

            int xStep = 1;

            if (position.Right - position.Left > FieldData.DataPoints.Count)
            {
                while (FieldData.DataPoints.Count > (position.Right - position.Left) * xStep)
                {
                    xStep++;
                }
            }

            float xMult = (float)(position.Right - position.Left) / (float)(FieldData.DataPoints.Count() - 1);

            //////////////// X axis grid lines and labels
            ///// 
            TimeSpan timeSpan = TimeSpan.FromTicks(FieldData.DataPoints[FieldData.DataPoints.Count - 1].dateTime.Ticks - FieldData.DataPoints[0].dateTime.Ticks);
            int Minutes = 1;
            if (timeSpan.TotalHours > 1)
            {
                Minutes = 10;
            }
            else if (timeSpan.TotalMinutes > 15)
            {
                Minutes = 5;
            }
            /////////////// 

            float lastY = 0;

            if (!invertY)
            {
                lastY = (float)(position.Bottom - ((float)FieldData.DataPoints[0].value * yMult) + position.Top);
                
            }
            else
            {
                lastY = ((float)(FieldData.DataPoints[0].value - MinY) * yMult) + (float)position.Top;
            }

            

            float lastX = 0;
            points.Add(new PointF(lastX, lastY));
            float curY;

            
            
            long lastTicks = FieldData.DataPoints[xStep].dateTime.Ticks;

            for (i = xStep ; i < FieldData.DataPoints.Count; i += xStep)
            {
                float curX = i * xMult;
                if (!invertY)
                {
                    curY = (float)(position.Bottom - ((float)FieldData.DataPoints[i].value * yMult) + position.Top);
                }
                else
                {
                    curY = (float)(FieldData.DataPoints[i].value - MinY) * yMult;
                }
                points.Add(new PointF(curX, curY));
                timeSpan = TimeSpan.FromTicks(FieldData.DataPoints[i].dateTime.Ticks - lastTicks);
                if (timeSpan.TotalMinutes >= Minutes)
                {
                    canvas.StrokeColor = Colors.DarkGray;
                    canvas.StrokeSize = 1;
                    canvas.DrawLine(curX, (float)position.Top, curX, (float)position.Bottom);
                    canvas.DrawString(FieldData.DataPoints[i].dateTime.ToShortTimeString(), curX, position.Bottom-20, HorizontalAlignment.Center);
                    lastTicks = FieldData.DataPoints[i].dateTime.Ticks;
                    canvas.StrokeColor = Colors.Blue;
                    canvas.StrokeSize = 6;
                }
                lastX = curX;
                lastY = curY;
            }

            var smooth = CreateSmoothQuadSpline(points);
            canvas.StrokeColor = Colors.LightBlue;
            canvas.StrokeSize = 3;
            canvas.DrawPath(smooth);
            var spline = GaussianSmoothedPath(points);
            canvas.StrokeColor = Colors.Blue;
            canvas.DrawPath(spline);
            

            canvas.ResetState();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DataPointDisplay.Draw: {ex.Message}");
        }
    }

    public static PathF CreateSmoothQuadSpline(IList<PointF> pts)
    {
        var path = new PathF();

        if (pts == null || pts.Count < 2)
            return path;

        path.MoveTo(pts[0]);

        for (int i = 1; i < pts.Count - 1; i++)
        {
            PointF p0 = pts[i - 1];
            PointF p1 = pts[i];
            PointF p2 = pts[i + 1];

            // Midpoints to convert Catmull-Rom segment into QuadTo
            var mid1 = new PointF((p0.X + p1.X) / 2f, (p0.Y + p1.Y) / 2f);
            var mid2 = new PointF((p1.X + p2.X) / 2f, (p1.Y + p2.Y) / 2f);

            // First segment
            path.LineTo(mid1);
            path.QuadTo(p1, mid2);
        }

        // Last line segment to final point
        path.LineTo(pts.Last());

        return path;
    }

    public static PathF BSplinePath(IList<PointF> pts, int resolution = 20)
    {
        var path = new PathF();

        if (pts == null || pts.Count < 4)
            return path; // need at least 4 points for cubic B-spline

        bool firstPoint = true;
        int n = pts.Count - 1;

        for (int i = 1; i < n - 1; i++)
        {
            for (int step = 0; step <= resolution; step++)
            {
                float t = step / (float)resolution;

                float b0 = ((1 - t) * (1 - t) * (1 - t)) / 6f;
                float b1 = (3 * t * t * t - 6 * t * t + 4) / 6f;
                float b2 = (-3 * t * t * t + 3 * t * t + 3 * t + 1) / 6f;
                float b3 = (t * t * t) / 6f;

                float x = b0 * pts[i - 1].X + b1 * pts[i].X + b2 * pts[i + 1].X + b3 * pts[i + 2].X;
                float y = b0 * pts[i - 1].Y + b1 * pts[i].Y + b2 * pts[i + 1].Y + b3 * pts[i + 2].Y;

                var p = new PointF(x, y);

                if (firstPoint)
                {
                    path.MoveTo(p);
                    firstPoint = false;
                }
                else
                {
                    path.LineTo(p);
                }
            }
        }

        return path;
    }
  
    // Gaussian-weighted smoothing, returns a PathF
    public static PathF GaussianSmoothedPath(IList<PointF> pts, int windowRadius = 10, float sigma = -1f)
    {
        var path = new PathF();

        if (pts == null || pts.Count == 0)
            return path;

        int n = pts.Count;
        if (sigma <= 0f)
            sigma = Math.Max(1f, windowRadius / 2f); // reasonable default

        // Precompute weights for offsets -windowRadius..windowRadius
        var offsets = Enumerable.Range(-windowRadius, windowRadius * 2 + 1).ToArray();
        double[] weights = offsets.Select(j => Math.Exp(-(j * j) / (2.0 * sigma * sigma))).ToArray();
        double weightSum = weights.Sum();

        // normalize weights
        for (int i = 0; i < weights.Length; i++) weights[i] /= weightSum;

        bool first = false;
        path.MoveTo(pts[0]);

        for (int i = 0; i < n; i++)
        {
            double sx = 0, sy = 0;
            double wtot = 0;

            // apply weights with clamping at edges (you can also reflect or pad)
            for (int k = 0; k < offsets.Length; k++)
            {
                int j = i + offsets[k];
                if (j < 0) j = 0;
                if (j >= n) j = n - 1;

                sx += weights[k] * pts[j].X;
                sy += weights[k] * pts[j].Y;
                wtot += weights[k];
            }

            // re-normalize at edges (optional, but ensures correct weights)
            if (wtot <= 0) wtot = 1;
            float x = (float)(sx / wtot);
            float y = (float)(sy / wtot);

            var p = new PointF(x, y);

            if (first)
            {
                path.MoveTo(p);
                first = false;
            }
            else
            {
                path.LineTo(p);
            }
        }
        path.LineTo(pts.Last());

        return path;
    }





    public void UpdateSetup(Setup settings)
    {
        setup = settings;
    }
}
