namespace SailMonitor.Services
{
    using Microsoft.Maui.Controls.Shapes;
    using SailMonitor.Models;

    public class WindPointDisplay
    {
        private readonly string name1;
        private readonly string name2;
        private readonly string precision;
        private readonly Label title;
        private readonly Label fieldDir;
        private readonly Label fieldSpd;
        private readonly Label speedUofM;
        private readonly Grid layout;
        private readonly Grid speedRow;
        private readonly Border card;
        private readonly InstrumentStatistics speedStatistics;
        private readonly InstrumentStatistics directionStatistics;

        public WindPointDisplay(string name1, string name2, Grid owner, Setup setup, int row, int column, string precision, string description, string uofm)
        {
            this.name1 = name1;
            this.name2 = name2;
            this.precision = precision;
            title = InstrumentStatistics.CreateLabel(description.ToUpperInvariant() + " WIND");
            title.FontAttributes = FontAttributes.Bold;
            fieldSpd = InstrumentStatistics.CreateLabel("--");
            fieldSpd.FontAttributes = FontAttributes.Bold;
            speedUofM = InstrumentStatistics.CreateLabel(uofm);
            fieldDir = InstrumentStatistics.CreateLabel(name1 == "AWD" ? "Angle --°" : "Direction --°");
            speedStatistics = new InstrumentStatistics();
            directionStatistics = new InstrumentStatistics();
            speedRow = new Grid { ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star)
            }};
            speedRow.Add(fieldSpd, 0, 0);
            speedRow.Add(speedUofM, 1, 0);
            layout = new Grid
            {
                Padding = new Thickness(8, 4),
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(new GridLength(0.13, GridUnitType.Star)),
                    new RowDefinition(new GridLength(0.35, GridUnitType.Star)),
                    new RowDefinition(new GridLength(0.20, GridUnitType.Star)),
                    new RowDefinition(new GridLength(0.14, GridUnitType.Star)),
                    new RowDefinition(new GridLength(0.18, GridUnitType.Star))
                }
            };
            layout.Add(title, 0, 0);
            layout.Add(speedRow, 0, 1);
            layout.Add(speedStatistics, 0, 2);
            layout.Add(fieldDir, 0, 3);
            layout.Add(directionStatistics, 0, 4);
            card = new Border
            {
                Content = layout,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Margin = new Thickness(3),
                Padding = 0
            };
            owner.Add(card, column, row);
            layout.SizeChanged += (_, _) => Resize(0, 0);
            ApplyTheme(setup);
        }

        public void Update(List<FieldData> dataPoints)
        {
            var dir = dataPoints.FirstOrDefault(d => d.name == name1);
            if (dir != null)
            {
                double shown = dir.Current;
                string suffix = string.Empty;
                if (name1 == "AWD")
                {
                    if (shown > 180)
                    {
                        shown = 360 - shown;
                        suffix = " P";
                    }
                    else
                    {
                        suffix = " S";
                    }
                }
                fieldDir.Text = (name1 == "AWD" ? "Angle " : "Direction ") + shown.ToString(precision) + "°" + suffix;
                directionStatistics.Update(dir, precision, "°");
            }
            var spd = dataPoints.FirstOrDefault(d => d.name == name2);
            if (spd != null)
            {
                fieldSpd.Text = spd.Current.ToString(precision);
                speedStatistics.Update(spd, precision);
            }
            Resize(0, 0);
        }

        public void Resize(double width, double height)
        {
            double w = layout.Width - 16;
            double h = layout.Height - 8;
            if (w <= 0 || h <= 0)
            {
                return;
            }
            title.FontSize = InstrumentStatistics.Fit(title.Text, w, h * 0.13, 22);
            fieldSpd.FontSize = InstrumentStatistics.Fit(fieldSpd.Text, w * 0.75, h * 0.35, 110);
            speedUofM.FontSize = InstrumentStatistics.Fit(speedUofM.Text, w * 0.25, h * 0.20, 22);
            fieldDir.FontSize = InstrumentStatistics.Fit(fieldDir.Text, w, h * 0.14, 24);
            speedStatistics.Resize();
            directionStatistics.Resize();
        }

        public void ApplyTheme(Setup settings)
        {
            // The same blue/green identity is used by the compass pointers, in both modes.
            var accent = Color.FromArgb(name1 == "AWD"
                ? (settings.Night ? "#1687E8" : "#1267C4")
                : (settings.Night ? "#18B957" : "#087A32"));
            var muted = settings.Night ? Color.FromArgb("#C77A7A") : Color.FromArgb("#52627A");
            card.BackgroundColor = settings.Night ? Color.FromArgb("#0C0C0C") : Color.FromArgb("#FBFCFE");
            card.Stroke = settings.Night ? Color.FromArgb("#3A2020") : Color.FromArgb("#D9E2EE");
            layout.BackgroundColor = Colors.Transparent;
            speedRow.BackgroundColor = Colors.Transparent;
            foreach (var label in new[] { title, fieldSpd, speedUofM, fieldDir })
            {
                label.BackgroundColor = Colors.Transparent;
                label.TextColor = label == fieldDir ? settings.foreColor : accent;
            }
            speedUofM.Text = settings.WindSpeed.SelectedUnit;
            speedStatistics.ApplyTheme(settings.foreColor, muted);
            directionStatistics.ApplyTheme(settings.foreColor, muted);
            Resize(0, 0);
        }

        public void OnSetupChanged(Setup settings) => ApplyTheme(settings);
    }
}
