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
            canvas.DrawString(txt, (float)(Width / 2), (float)(Height * .05), HorizontalAlignment.Center);
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
            canvas.DrawString(txt, (float)(Width / 2), (float)((Height * .2) + textSize.Height), HorizontalAlignment.Center);
        else
            canvas.DrawString(txt, (float)(Width / 2), (float)((Height * .2) + textSize.Height * 2), HorizontalAlignment.Center);

        if (data.DataPoints.Count < 2)
            return;

        // Direction is circular. Convert the plotted series to an unwrapped angle so
        // crossing North (359 -> 0 or 0 -> 359) follows the shortest path instead of
        // drawing a false line across the entire graph. Numeric readouts remain 0..360.
        var plotValues = new double[data.DataPoints.Count];
        plotValues[0] = data.DataPoints[0].value;
        if (!drawXAxis)
        {
            double previousRaw = data.DataPoints[0].value;
            for (int n = 1; n < data.DataPoints.Count; n++)
            {
                double raw = data.DataPoints[n].value;
                double delta = raw - previousRaw;
                if (delta > 180.0) delta -= 360.0;
                else if (delta < -180.0) delta += 360.0;
                plotValues[n] = plotValues[n - 1] + delta;
                previousRaw = raw;
            }
        }
        else
        {
            for (int n = 1; n < data.DataPoints.Count; n++)
                plotValues[n] = data.DataPoints[n].value;
        }

        bool invertY = speedName == "DPT";
        double rawMin = plotValues.Min();
        double rawMax = plotValues.Max();
        double range = rawMax - rawMin;
        if (range < 0.001) range = drawXAxis ? Math.Max(1.0, Math.Abs(rawMax) * .1) : 20.0;

        double pad = range * .10;
        double yMin = drawXAxis ? Math.Min(0, rawMin - pad) : rawMin - pad;
        double yMax = rawMax + pad;
        if (yMax <= yMin) yMax = yMin + 1;

        float graphHeight = position.Bottom - position.Top;
        float yMult = graphHeight / (float)(yMax - yMin);

        // Horizontal grid: labels reflect the continuous/unwrapped scale for direction.
        canvas.StrokeColor = Colors.DarkGray;
        canvas.StrokeSize = 1;
        canvas.FontColor = darkline;
        const int gridLines = 5;
        for (int g = 0; g <= gridLines; g++)
        {
            double value = yMin + ((yMax - yMin) * g / gridLines);
            float y = invertY
                ? position.Top + (float)((value - yMin) * yMult)
                : position.Bottom - (float)((value - yMin) * yMult);
            canvas.DrawLine(0, y, position.Right, y);
            string label = drawXAxis ? Math.Round(value).ToString() : $"{Math.Round(value)}°";
            if (drawXAxis) canvas.DrawString(label, 0, y, HorizontalAlignment.Left);
            else canvas.DrawString(label, position.Right - 20, y, HorizontalAlignment.Right);
        }

        int count = data.DataPoints.Count;
        int xStep = 1;
        while (count > Math.Max(1, (position.Right - position.Left) * xStep)) xStep++;
        float xMult = (position.Right - position.Left) / Math.Max(1f, count - 1f);

        TimeSpan timeSpan = data.DataPoints[^1].dateTime - data.DataPoints[0].dateTime;
        int minutes = timeSpan.TotalHours > 1 ? 10 : timeSpan.TotalMinutes > 15 ? 5 : 1;

        float MapY(double value) => invertY
            ? position.Top + (float)((value - yMin) * yMult)
            : position.Bottom - (float)((value - yMin) * yMult);

        var points = new List<PointF>();
        float lastX = 0;
        float lastY = MapY(plotValues[0]);
        points.Add(new PointF(lastX, lastY));

        long lastTicks = data.DataPoints[Math.Min(xStep, count - 1)].dateTime.Ticks;
        canvas.StrokeColor = darkline;
        canvas.StrokeSize = 6;
        canvas.FontColor = setup.foreColor;

        for (int i = xStep; i < count; i += xStep)
        {
            float curX = i * xMult;
            float curY = MapY(plotValues[i]);
            if (drawRaw) canvas.DrawLine(lastX, lastY, curX, curY);
            points.Add(new PointF(curX, curY));

            if (drawXAxis)
            {
                timeSpan = TimeSpan.FromTicks(data.DataPoints[i].dateTime.Ticks - lastTicks);
                if (timeSpan.TotalMinutes >= minutes)
                {
                    canvas.StrokeColor = Colors.DarkGray;
                    canvas.StrokeSize = 1;
                    canvas.DrawLine(curX, position.Top, curX, position.Bottom);
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
