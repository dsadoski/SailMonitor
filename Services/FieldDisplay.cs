namespace SailMonitor.Services
{
    using Microsoft.Maui.Controls.Shapes;
    using SailMonitor.Models;

    public class FieldDisplay
    {
        public Label title;
        public Label field;
        public Label stats;
        public string name;
        public VerticalStackLayout verticalStackLayout;
        public HorizontalStackLayout horizontalStackLayout;

        private readonly FieldData fieldDataSeed;
        private readonly string precision;
        private readonly string description;
        private readonly string unitOfMeasure;
        private readonly Border card;
        private readonly Setup setup;

        public FieldDisplay(string name, Grid owner, Setup setup, int row, int column, string precision, string description, string unitOfMeasure)
        {
            this.name = name;
            this.precision = precision;
            this.description = description;
            this.unitOfMeasure = unitOfMeasure;
            this.setup = setup;
            fieldDataSeed = new FieldData(name, unitOfMeasure);

            title = new Label
            {
                Text = description,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            field = new Label
            {
                Text = "--",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };

            stats = new Label
            {
                Text = "Min --   Avg --   Max --",
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };

            var unit = new Label
            {
                Text = unitOfMeasure,
                VerticalTextAlignment = TextAlignment.End,
                Margin = new Thickness(3, 0, 0, 5)
            };

            horizontalStackLayout = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 0
            };
            horizontalStackLayout.Add(field);
            if (unitOfMeasure != "°")
                horizontalStackLayout.Add(unit);

            verticalStackLayout = new VerticalStackLayout
            {
                Spacing = 0,
                Padding = new Thickness(8, 5),
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };
            verticalStackLayout.Add(title);
            verticalStackLayout.Add(horizontalStackLayout);
            verticalStackLayout.Add(stats);

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
            var data = dataPoints.FirstOrDefault(d => d.name == name);
            if (data == null) return;

            field.Text = data.Current.ToString(precision);
            stats.Text = $"Min {data.Min.ToString(precision)}   Avg {data.Average.ToString(precision)}   Max {data.Max.ToString(precision)}";
        }

        public void Resize(double width, double height)
        {
            double baseSize = Math.Min(width, height);
            title.FontSize = Math.Clamp(baseSize * 0.021, 13, 24);
            field.FontSize = Math.Clamp(baseSize * 0.105, 38, 96);
            stats.FontSize = Math.Clamp(baseSize * 0.017, 11, 20);

            foreach (var child in horizontalStackLayout.Children.OfType<Label>())
                if (child != field)
                    child.FontSize = Math.Clamp(baseSize * 0.021, 12, 22);
        }

        public void ApplyTheme(Setup settings)
        {
            var muted = settings.Night ? Color.FromArgb("#C77A7A") : Color.FromArgb("#52627A");
            var border = settings.Night ? Color.FromArgb("#3A2020") : Color.FromArgb("#D9E2EE");
            var cardColor = settings.Night ? Color.FromArgb("#0C0C0C") : Color.FromArgb("#FBFCFE");

            title.TextColor = settings.foreColor;
            field.TextColor = settings.foreColor;
            stats.TextColor = muted;
            foreach (var child in horizontalStackLayout.Children.OfType<Label>())
                child.TextColor = child == field ? settings.foreColor : muted;

            card.BackgroundColor = cardColor;
            card.Stroke = border;
        }
    }
}
