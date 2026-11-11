namespace SailMonitor.Services
{
    using SailMonitor.Models;

    // Each statistic owns a column so long values cannot push its neighbours off-card.
    public class InstrumentStatistics : Grid
    {
        private readonly Label[] captions = new Label[3];
        private readonly Label[] values = new Label[3];

        public InstrumentStatistics()
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            };
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Star)
            };
            string[] names = { "MIN", "AVG", "MAX" };
            for (int i = 0; i < 3; i++)
            {
                captions[i] = CreateLabel(names[i]);
                values[i] = CreateLabel("--");
                values[i].FontAttributes = FontAttributes.Bold;
                Children.Add(captions[i]);
                Grid.SetColumn(captions[i], i);
                Grid.SetRow(captions[i], 0);
                Children.Add(values[i]);
                Grid.SetColumn(values[i], i);
                Grid.SetRow(values[i], 1);
            }
            SizeChanged += (_, _) => Resize();
        }

        public static Label CreateLabel(string text) => new Label
        {
            Text = text,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap,
            FontAutoScalingEnabled = false,
            Margin = 0,
            Padding = 0
        };

        // Conservative width allowance also handles a change in digit count at runtime.
        public static double Fit(string text, double width, double height, double maximum)
        {
            return Math.Max(1, Math.Min(maximum,
                Math.Min(Math.Max(1, height) / 1.5,
                    Math.Max(1, width) / (Math.Max(1, text.Length) * 0.70))));
        }

        public void Update(FieldData data, string precision, string suffix = "")
        {
            values[0].Text = data.Min.ToString(precision) + suffix;
            values[1].Text = data.Average.ToString(precision) + suffix;
            values[2].Text = data.Max.ToString(precision) + suffix;
            Resize();
        }

        public void Resize()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                captions[i].FontSize = Fit(captions[i].Text, Width / 3 - 4, Height / 2, 15);
                values[i].FontSize = Fit(values[i].Text, Width / 3 - 4, Height / 2, 22);
            }
        }

        public void ApplyTheme(Color foreground, Color muted)
        {
            BackgroundColor = Colors.Transparent;
            foreach (var label in captions)
            {
                label.TextColor = muted;
                label.BackgroundColor = Colors.Transparent;
            }
            foreach (var label in values)
            {
                label.TextColor = foreground;
                label.BackgroundColor = Colors.Transparent;
            }
        }
    }
}
