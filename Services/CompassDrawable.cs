namespace SailMonitor.Services
{
    using SailMonitor.Models;

    public class CompassDrawable : IDrawable
    {
        public float TrueWind = 100;
        public float ApparentWind = 85;
        public float Heading = 0;
        private Setup setup;

        public CompassDrawable(Setup setup) => this.setup = setup;

        public void UpdateSetup(Setup settings) => setup = settings;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (dirtyRect.Width < 1 || dirtyRect.Height < 1) return;

            float cx = dirtyRect.Width / 2f;
            float cy = dirtyRect.Height / 2f;
            float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.40f;
            var primary = setup.foreColor;
            var muted = setup.Night ? Color.FromArgb("#7D3333") : Color.FromArgb("#6E7F96");
            var faint = setup.Night ? Color.FromArgb("#2A1717") : Color.FromArgb("#DCE5F0");
            var accent = setup.Night ? Color.FromArgb("#E54848") : Color.FromArgb("#1267C4");

            canvas.SaveState();
            canvas.Translate(cx, cy);

            // Clean concentric rose.
            canvas.StrokeColor = faint;
            canvas.StrokeSize = 1;
            canvas.DrawCircle(0, 0, radius * .68f);
            canvas.DrawCircle(0, 0, radius);

            // Fixed lubber line at the top.
            canvas.StrokeColor = accent;
            canvas.StrokeSize = Math.Max(2, radius * .012f);
            canvas.DrawLine(0, -radius * 1.08f, 0, -radius * .82f);

            // Rotating degree ring: 5° minor, 10° medium, 30° major.
            for (int deg = 0; deg < 360; deg += 5)
            {
                float angle = DegToRad(deg - Heading);
                bool major = deg % 30 == 0;
                bool medium = deg % 10 == 0;
                float inner = radius * (major ? .84f : medium ? .90f : .94f);

                canvas.StrokeColor = major ? primary : muted;
                canvas.StrokeSize = major ? 2.2f : medium ? 1.5f : 1f;
                canvas.DrawLine(
                    inner * MathF.Sin(angle), -inner * MathF.Cos(angle),
                    radius * MathF.Sin(angle), -radius * MathF.Cos(angle));

                if (major)
                {
                    string text = deg switch
                    {
                        0 => "N",
                        90 => "E",
                        180 => "S",
                        270 => "W",
                        _ => deg.ToString()
                    };
                    float tr = radius * .74f;
                    canvas.FontColor = primary;
                    canvas.FontSize = deg % 90 == 0 ? radius * .13f : radius * .075f;
                    canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
                    canvas.DrawString(text,
                        tr * MathF.Sin(angle) - radius * .12f,
                        -tr * MathF.Cos(angle) - radius * .06f,
                        radius * .24f, radius * .12f,
                        HorizontalAlignment.Center, VerticalAlignment.Center);
                }
            }

            // Wind pointers remain relative to the vessel.
            DrawWindPointer(canvas, TrueWind, setup.Night ? Color.FromArgb("#B96A6A") : Color.FromArgb("#246FB8"), radius, false);
            DrawWindPointer(canvas, ApparentWind, setup.Night ? Color.FromArgb("#E54848") : Color.FromArgb("#168A5B"), radius, true);

            // Simple vessel silhouette: much sharper than a center line.
            var boat = new PathF();
            boat.MoveTo(0, -radius * .48f);
            boat.LineTo(radius * .12f, radius * .28f);
            boat.LineTo(radius * .07f, radius * .45f);
            boat.LineTo(-radius * .07f, radius * .45f);
            boat.LineTo(-radius * .12f, radius * .28f);
            boat.Close();
            canvas.FillColor = setup.Night ? Color.FromArgb("#202020") : Color.FromArgb("#E8EEF5");
            canvas.FillPath(boat);
            canvas.StrokeColor = primary;
            canvas.StrokeSize = Math.Max(2, radius * .012f);
            canvas.DrawPath(boat);

            // Heading readout above the rose.
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.FontColor = primary;
            canvas.FontSize = radius * .17f;
            string heading = $"{Normalize(Heading):000}°";
            canvas.DrawString(heading, -radius * .45f, -radius * 1.24f,
                radius * .90f, radius * .20f,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.RestoreState();
        }

        private static void DrawWindPointer(ICanvas canvas, float degrees, Color color, float radius, bool apparent)
        {
            float a = DegToRad(degrees);
            float r1 = radius * .58f;
            float r2 = radius * .80f;
            float x1 = r1 * MathF.Sin(a);
            float y1 = -r1 * MathF.Cos(a);
            float x2 = r2 * MathF.Sin(a);
            float y2 = -r2 * MathF.Cos(a);

            canvas.StrokeColor = color;
            canvas.StrokeSize = apparent ? 5 : 3;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawLine(x1, y1, x2, y2);

            float side = apparent ? radius * .045f : radius * .035f;
            float back = DegToRad(degrees + 180);
            float left = DegToRad(degrees - 90);
            var arrow = new PathF();
            arrow.MoveTo(x2, y2);
            arrow.LineTo(
                x2 + radius * .10f * MathF.Sin(back) + side * MathF.Sin(left),
                y2 - radius * .10f * MathF.Cos(back) - side * MathF.Cos(left));
            arrow.LineTo(
                x2 + radius * .10f * MathF.Sin(back) - side * MathF.Sin(left),
                y2 - radius * .10f * MathF.Cos(back) + side * MathF.Cos(left));
            arrow.Close();
            canvas.FillColor = color;
            canvas.FillPath(arrow);
        }

        private static float DegToRad(float d) => d * MathF.PI / 180f;
        private static int Normalize(float d)
        {
            int value = (int)MathF.Round(d) % 360;
            return value < 0 ? value + 360 : value;
        }
    }
}
