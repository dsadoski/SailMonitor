namespace SailMonitor.Services
{
    using SailMonitor.Models;

    public class FieldDisplay
    {
        public Label title;
        public Label field;
        public Label stats;
        public string name;
        public VerticalStackLayout verticalStackLayout;
        public HorizontalStackLayout horizontalStackLayout;
        private Grid grid;
        private FieldData fieldData;
        private string precision;
        private int column;
        private int row;
        private string description;
        string unitOfMeasure;

        public FieldDisplay(string name, Grid owner, Setup setup, int row, int column, string precision, string description, string unitOfMeasure)
        {
            this.column = column;
            this.row = row;
            this.name = name;
            this.precision = precision;
            this.unitOfMeasure = unitOfMeasure;
            fieldData = new FieldData(this.name, unitOfMeasure);
            grid = owner;
            verticalStackLayout = new VerticalStackLayout();
            title = new Label();
            title.Text = description;
            title.FontSize = 16;
            field = new Label();
            field.FontSize = 96;
            stats = new Label();
            stats.FontSize = 16;
            stats.Text = unitOfMeasure;

            title.TextColor = setup.foreColor;
            field.TextColor = setup.foreColor;
            stats.TextColor = setup.foreColor;

            verticalStackLayout.Add(title);
            horizontalStackLayout = new HorizontalStackLayout();
            horizontalStackLayout.VerticalOptions = LayoutOptions.Fill;
            horizontalStackLayout.Add(field);
            if (unitOfMeasure != "°")
            {
                stats.VerticalOptions = LayoutOptions.End;
            }
            else
            {
                stats.FontSize = field.FontSize;
                stats.VerticalOptions = LayoutOptions.Start;
            }
            horizontalStackLayout.Add(stats);

            verticalStackLayout.Add(horizontalStackLayout);
            //verticalStackLayout.Add(stats);
            grid.Children.Add(verticalStackLayout);
            grid.SetRow(verticalStackLayout, this.row);
            grid.SetColumn(verticalStackLayout, this.column);
            this.description = description;
        }

        public void Update(List<FieldData> dataPoints)
        {
            fieldData = dataPoints.FirstOrDefault(d => d.name == name);
            if (fieldData != null)
            {
                title.Text = description + " " + fieldData.Min.ToString($"{precision}") + " - " + fieldData.Average.ToString($"{precision}") + " -" + fieldData.Max.ToString($"{precision}"); ;
                field.Text = fieldData.Current.ToString($"{precision}");
            }
        }

        public void Resize(double width, double height)
        {
            double baseSize = Math.Min(width, height);

            double headerSize = baseSize * 0.016; // e.g., "Heading"
            double valueSize = baseSize * 0.120; // e.g., "123.45"

            title.FontSize = headerSize;

            field.FontSize = valueSize;
            field.FontAttributes = FontAttributes.Bold;
            if (unitOfMeasure != "°")
            {
                stats.FontSize = headerSize;
            }
            else
            {
                stats.FontSize = valueSize;
            }
        }
    }
}
