namespace SailMonitor.Services
{
    using Microsoft.Maui.Controls.Shapes;
    using SailMonitor.Models;

    public class WindPointDisplay
    {
        public Label title;
        public Label fieldDir;
        public Label fieldSpd;
        public Label speedUofM;
        public Label statsSpd;
        public string name1;
        public string name2;
        public VerticalStackLayout verticalStackLayout;
        public Setup setup;
        public string UofM;

        private readonly string precision;
        private readonly string description;
        private readonly Border card;
        private readonly Label dirStats;
        private readonly HorizontalStackLayout speedRow;

        public WindPointDisplay(string name1, string name2, Grid owner, Setup setup, int row, int column, string precision, string description, string uofm)
        {
            this.setup = setup;
            this.description = description;
            this.name1 = name1;
            this.name2 = name2;
            this.precision = precision;
            UofM = uofm;

            title = new Label
            {
                Text = description + " Wind",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            fieldDir = new Label
            {
                Text = "---°",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            dirStats = new Label
            {
                Text = "Dir  Min --   Avg --   Max --",
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };

            fieldSpd = new Label
            {
                Text = "--",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            speedUofM = new Label
            {
                Text = UofM,
                VerticalTextAlignment = TextAlignment.End,
                Margin = new Thickness(3, 0, 0, 4)
            };

            speedRow = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 0
            };
            speedRow.Add(fieldSpd);
            speedRow.Add(speedUofM);

            statsSpd = new Label
            {
                Text = "Spd  Min --   Avg --   Max --",
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };

            verticalStackLayout = new VerticalStackLayout
            {
                Spacing = 0,
                Padding = new Thickness(6, 3),
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };
            verticalStackLayout.Add(title);
            verticalStackLayout.Add(fieldDir);
            verticalStackLayout.Add(dirStats);
            verticalStackLayout.Add(speedRow);
            verticalStackLayout.Add(statsSpd);

            card = new Border
            {
                Content = verticalStackLayout,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Margin = new Thickness(4),
                Padding = 0,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            owner.Children.Add(card);
            Grid.SetRow(card, row);
            Grid.SetColumn(card, column);
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

                fieldDir.Text = $"{shown.ToString(precision)}°{suffix}";
                dirStats.Text = $"Dir  Min {dir.Min.ToString(precision)}°   Avg {dir.Average.ToString(precision)}°   Max {dir.Max.ToString(precision)}°";
            }

            var spd = dataPoints.FirstOrDefault(d => d.name == name2);
            if (spd != null)
            {
                fieldSpd.Text = spd.Current.ToString(precision);
                statsSpd.Text = $"Spd  Min {spd.Min.ToString(precision)}   Avg {spd.Average.ToString(precision)}   Max {spd.Max.ToString(precision)}";
            }
        }

        public void Resize(double width, double height)
        {
            double baseSize = Math.Min(width, height);
            bool portrait = height > width;
            title.FontSize = Math.Clamp(baseSize * (portrait ? 0.017 : 0.019), 11, 22);
            fieldDir.FontSize = Math.Clamp(baseSize * (portrait ? 0.058 : 0.070), 26, 64);
            fieldSpd.FontSize = Math.Clamp(baseSize * (portrait ? 0.058 : 0.070), 26, 64);
            speedUofM.FontSize = Math.Clamp(baseSize * 0.016, 10, 20);
            dirStats.FontSize = Math.Clamp(baseSize * (portrait ? 0.012 : 0.014), 9, 17);
            statsSpd.FontSize = Math.Clamp(baseSize * (portrait ? 0.012 : 0.014), 9, 17);
        }

        public void ApplyTheme(Setup settings)
        {
            setup = settings;
            var muted = settings.Night ? Color.FromArgb("#C77A7A") : Color.FromArgb("#52627A");
            var border = settings.Night ? Color.FromArgb("#3A2020") : Color.FromArgb("#D9E2EE");
            var cardColor = settings.Night ? Color.FromArgb("#0C0C0C") : Color.FromArgb("#FBFCFE");

            title.TextColor = settings.foreColor;
            fieldDir.TextColor = settings.foreColor;
            fieldSpd.TextColor = settings.foreColor;
            speedUofM.TextColor = muted;
            dirStats.TextColor = muted;
            statsSpd.TextColor = muted;
            card.BackgroundColor = cardColor;
            card.Stroke = border;
        }

        public void OnSetupChanged(Setup settings) => ApplyTheme(settings);
    }
}
