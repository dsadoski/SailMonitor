namespace SailMonitor.Services;

using Microsoft.Maui.Graphics.Platform;
using SailMonitor.Models;
using System.ComponentModel.Design;

public class WindDisplay : IDrawable
{
    public FieldData SpeedData;
    public FieldData DirData;
    public string speedName = string.Empty;
    public string dirName = string.Empty;
    public GraphicsView graphicsView;
    public string Description;
    private Setup setup;
    public double Width;
    public double Height;

    private string precision = "F2";
    private List<Microsoft.Maui.Graphics.Font> fonts;
    private ICanvas canvas;
    private RectF DirtyRect;
    public string unitOfMeasureSpeed;
    public string unitOfMeasureDir;

    public bool drawRaw;
    public bool drawSmoothed;
    public bool drawAveraged;

    public WindDisplay(string SpeedName, string DirName, string Precision, string Description, string UofMSpeed, string UofMDir)
    {
        drawRaw = false;
        drawSmoothed = true;
        drawAveraged = true;
        this.speedName = SpeedName;
        this.dirName = DirName;
        precision = Precision;
        this.Description = Description;
        unitOfMeasureSpeed = UofMSpeed;
        unitOfMeasureDir = UofMDir;
        SpeedData = new FieldData(this.speedName, unitOfMeasureSpeed);
        DirData = new FieldData(this.dirName, unitOfMeasureDir);
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

            DrawdataSetup(canvas, DirtyRect, SpeedData, Colors.Blue, Colors.LightBlue, true);
            DrawdataSetup(canvas, DirtyRect, DirData, Colors.Red, Colors.Pink, false);

            canvas.ResetState();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DataPointDisplay.Draw: {ex.Message}");
        }
    }

    public void DrawdataSetup(ICanvas Canvas, RectF DirtyRect, FieldData data, Color darkline, Color lightline, bool drawXAxis)
    {
        // Not used canvas.FontSize = 18;
        var position = this.DirtyRect;
        string txt = string.Empty;

        canvas.FontSize = 24;
        var textSize = canvas.GetStringSize("M", fonts[0], 18);
        if (drawXAxis)
        {   
            canvas.DrawString(Description, 1, textSize.Height, HorizontalAlignment.Left);
        }

        canvas.FontSize = 64;
        canvas.FontColor = darkline;
        if (drawXAxis)
        {
            txt = this.SpeedData.Current.ToString($"{precision}") + " " + unitOfMeasureSpeed;
            canvas.DrawString(txt, (float)(Width / 2), (float)(Height * .05) , HorizontalAlignment.Center);
        }
        else
        {
            txt = DirData.Current.ToString($"{precision}") + unitOfMeasureDir;
            textSize = canvas.GetStringSize(txt, fonts[0], 64);
            canvas.DrawString(txt, (float)(Width / 2), (float)(Height * .05) + (float)(textSize.Height * 1.2f), HorizontalAlignment.Center);
        }
        textSize = canvas.GetStringSize("M", fonts[0], 72);

        canvas.FontSize = 18;
        txt = data.Min.ToString($"{precision}") + " - " + data.Max.ToString($"{precision}");
        if (drawXAxis)
        {
            canvas.DrawString(txt, (float)( Width / 2), (float)((Height * .2) + textSize.Height), HorizontalAlignment.Center);
        }
        else
        {
            canvas.DrawString(txt, (float)( Width / 2), (float)((Height * .2) + textSize.Height * 2 ), HorizontalAlignment.Center);
        }


        if (data.DataPoints.Count < 2 || data.Max == data.Min)
        {
            // Not enough data to draw
            canvas.ResetState();
            return;
        }

        // canvas.FillColor = Colors.White;
        canvas.FillColor = new Color(0, 0, 0, 0.3f);

        float MaxYSpd = (float)data.Max * 1.1f;
        int MYSpd = (int)MaxYSpd;
        int scaleMultSpd = 0;

        // scaling the Y axis
        if (MYSpd == 0)
        {
            MYSpd = 1;
        }
        else
        {
            while (MYSpd > 10)
            {
                MYSpd = MYSpd / 10;
                scaleMultSpd++;
            }

            if (MYSpd < 5)
            {
                MYSpd++;
            }
            else
            {
                MYSpd = 10;
                if (scaleMultSpd > 1)
                {
                    scaleMultSpd--;
                }
            }

            if (scaleMultSpd > 0)
            {
                MYSpd = (int)Math.Pow(10, scaleMultSpd) * MYSpd;
                MaxYSpd = MYSpd;
            }
            else
            {
                MaxYSpd = MYSpd;
            }
        }

        bool invertY = false;
        if (speedName == "DPT")
        {
            invertY = true;
        }

        float MinY = (float)data.Min * 0.9f;

        float yMult = (float)(position.Bottom - position.Top) / MaxYSpd;

        canvas.StrokeColor = Colors.DarkGray;
        int yStep = MYSpd / (int)Math.Pow(10, scaleMultSpd);

        if (scaleMultSpd > 1)
        {
            yStep = yStep * (int)Math.Pow(10, scaleMultSpd - 1);
        }

        if (yStep == MYSpd / (int)Math.Pow(10, scaleMultSpd) && yStep > 10)
        {
            yStep = yStep / 10;
        }

        canvas.StrokeSize = 2;
        int i;

        List<PointF> points = new List<PointF>();
        canvas.FontColor = darkline;
        for (i = 0; i <= MaxYSpd; i = i + yStep)
        {
            canvas.DrawLine(0, position.Bottom - ((float)i * yMult), (float)position.Right, position.Bottom - ((float)i * yMult));
            if (invertY)
            {
                if (drawXAxis)
                {
                    canvas.DrawString(i.ToString(), 0, (float)i * yMult, HorizontalAlignment.Left);
                }
                else
                {
                    canvas.DrawString(i.ToString(), position.Right - 20, (float)i * yMult, HorizontalAlignment.Right);
                }
            }
            else
            {
                if(drawXAxis)
                {
                    canvas.DrawString(i.ToString(), 0, position.Bottom - ((float)i * yMult), HorizontalAlignment.Left);
                    
                }
                else
                {
                    canvas.DrawString(i.ToString(), position.Right -20, position.Bottom - ((float)i * yMult), HorizontalAlignment.Right);
                }
                    
            }
        }

        canvas.StrokeColor = darkline;
        canvas.StrokeSize = 6;

        int xStep = 1;

        if (position.Right - position.Left > data.DataPoints.Count)
        {
            while (data.DataPoints.Count > (position.Right - position.Left) * xStep)
            {
                xStep++;
            }
        }

        float xMult = (float)(position.Right - position.Left) / (float)(data.DataPoints.Count() - 1);

        //////////////// X axis grid lines and labels
        ///// 
        TimeSpan timeSpan = TimeSpan.FromTicks(data.DataPoints[data.DataPoints.Count - 1].dateTime.Ticks - data.DataPoints[0].dateTime.Ticks);
        int Minutes = 1;

        if (drawXAxis)
        {
            if (timeSpan.TotalHours > 1)
            {
                Minutes = 10;
            }
            else if (timeSpan.TotalMinutes > 15)
            {
                Minutes = 5;
            }
        }
        /////////////// 
        

        float lastY = 0;

        if (!invertY)
        {
            lastY = (float)(position.Bottom - ((float)data.DataPoints[0].value * yMult) + position.Top);

        }
        else
        {
            lastY = ((float)(data.DataPoints[0].value - MinY) * yMult) + (float)position.Top;
        }



        float lastX = 0;
        points.Add(new PointF(lastX, lastY));
        float curY;



        long lastTicks = data.DataPoints[xStep].dateTime.Ticks;
        canvas.FontColor = setup.foreColor;
        for (i = xStep; i < data.DataPoints.Count; i += xStep)
        {
            float curX = i * xMult;
            if (!invertY)
            {
                curY = (float)(position.Bottom - ((float)data.DataPoints[i].value * yMult) + position.Top);
            }
            else
            {
                curY = (float)(data.DataPoints[i].value - MinY) * yMult;
            }
            if (drawRaw)
            {
                canvas.DrawLine(lastX, lastY, curX, curY);
            }
            points.Add(new PointF(curX, curY));
            if (drawXAxis)
            {
                timeSpan = TimeSpan.FromTicks(data.DataPoints[i].dateTime.Ticks - lastTicks);
                if (timeSpan.TotalMinutes >= Minutes)
                {
                    canvas.StrokeColor = Colors.DarkGray;
                    canvas.StrokeSize = 1;
                    canvas.DrawLine(curX, (float)position.Top, curX, (float)position.Bottom);
                    canvas.DrawString(data.DataPoints[i].dateTime.ToShortTimeString(), curX, position.Bottom - 20, HorizontalAlignment.Center);
                    lastTicks = data.DataPoints[i].dateTime.Ticks;
                    canvas.StrokeColor = darkline;
                    canvas.StrokeSize = 6;
                }
            }
            lastX = curX;
            lastY = curY;
        }

       
        if (drawSmoothed)
        {
            var smooth = CreateSmoothQuadSpline(points);
            canvas.StrokeColor = lightline;
            canvas.StrokeSize = 1;
            canvas.DrawPath(smooth);
        }
        if (drawAveraged)
        {
            var spline = GaussianSmoothedPath(points);
            canvas.StrokeColor = darkline;
            canvas.StrokeSize = 5;

            canvas.DrawPath(spline);
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
