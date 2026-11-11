namespace SailMonitor.Services
{
    using Microsoft.Maui.Controls.Shapes;
    using SailMonitor.Models;

    public class FieldDisplay
    {
        private readonly string name;
        private readonly string precision;
        private readonly Label title;
        private readonly Label field;
        private readonly Label unit;
        private readonly Grid layout;
        private readonly Grid valueRow;
        private readonly Border card;
        private readonly InstrumentStatistics statistics;

        public FieldDisplay(string name, Grid owner, Setup setup, int row, int column, string precision, string description, string unitOfMeasure)
        {
            this.name = name;
            this.precision = precision;
            title = InstrumentStatistics.CreateLabel(description);
            title.FontAttributes = FontAttributes.Bold;
            field = InstrumentStatistics.CreateLabel("--");
            field.FontAttributes = FontAttributes.Bold;
            unit = InstrumentStatistics.CreateLabel(unitOfMeasure == "°" ? "" : unitOfMeasure);
            valueRow = new Grid { ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star)
            }};
            valueRow.Add(field, 0, 0);
            valueRow.Add(unit, 1, 0);
            if (unitOfMeasure == "°")
            {
                Grid.SetColumnSpan(field, 2);
                unit.IsVisible = false;
            }
            statistics = new InstrumentStatistics();
            layout = new Grid
            {
                Padding = new Thickness(8, 4),
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(new GridLength(0.20, GridUnitType.Star)),
                    new RowDefinition(new GridLength(0.49, GridUnitType.Star)),
                    new RowDefinition(new GridLength(0.31, GridUnitType.Star))
                }
            };
            layout.Add(title, 0, 0);
            layout.Add(valueRow, 0, 1);
            layout.Add(statistics, 0, 2);
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
            var data = dataPoints.FirstOrDefault(d => d.name == name);
            if (data == null)
            {
                return;
            }
            field.Text = data.Current.ToString(precision) + (name == "HDG" ? "°" : "");
            statistics.Update(data, precision, name == "HDG" ? "°" : "");
            Resize(0, 0);
        }

        public void Resize(double width, double height)
        {
            // Measure the card, never the whole page, to leave room for the statistics.
            double w = layout.Width - 16;
            double h = layout.Height - 8;
            if (w <= 0 || h <= 0)
            {
                return;
            }
            title.FontSize = InstrumentStatistics.Fit(title.Text, w, h * 0.20, 22);
            field.FontSize = InstrumentStatistics.Fit(field.Text, w * (unit.IsVisible ? 0.75 : 1), h * 0.49, 96);
            unit.FontSize = InstrumentStatistics.Fit(unit.Text, w * 0.25, h * 0.30, 20);
            statistics.Resize();
        }

        public void ApplyTheme(Setup settings)
        {
            var muted = settings.Night ? Color.FromArgb("#C77A7A") : Color.FromArgb("#52627A");
            var cardColor = settings.Night ? Color.FromArgb("#0C0C0C") : Color.FromArgb("#FBFCFE");
            card.BackgroundColor = cardColor;
            card.Stroke = settings.Night ? Color.FromArgb("#3A2020") : Color.FromArgb("#D9E2EE");
            layout.BackgroundColor = Colors.Transparent;
            valueRow.BackgroundColor = Colors.Transparent;
            foreach (var label in new[] { title, field, unit })
            {
                label.BackgroundColor = Colors.Transparent;
                label.TextColor = label == unit ? muted : settings.foreColor;
            }
            unit.Text = name == "DPT" ? settings.Depth.SelectedUnit : name == "HDG" ? "" : settings.Speed.SelectedUnit;
            statistics.ApplyTheme(settings.foreColor, muted);
            Resize(0, 0);
        }
    }
}
