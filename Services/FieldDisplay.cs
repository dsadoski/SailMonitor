using SailMonitor.Models;

namespace SailMonitor.Services
{
    public class FieldDisplay
    {
        public Label title;
        public Label field;
        public Label stats;
        public string name;
        public VerticalStackLayout verticalStackLayout;
        private Grid grid;
        private FieldData fieldData;
        private string precision;
        private int column;
        private int row;
        private string description;

        public FieldDisplay(string name, Grid owner, Setup setup, int row, int column, string precision, string description)
        {
            this.column = column;
            this.row = row;
            this.name = name;
            this.precision = precision;
            fieldData = new FieldData(this.name);
            grid = owner;
            verticalStackLayout = new VerticalStackLayout();
            title = new Label();
            title.Text = description;
            title.FontSize = 16;
            field = new Label();
            field.FontSize = 96;
            stats = new Label();
            stats.FontSize = 12;

            title.TextColor = setup.foreColor;
            field.TextColor = setup.foreColor;
            stats.TextColor = setup.foreColor;

            verticalStackLayout.Add(title);
            verticalStackLayout.Add(field);
            verticalStackLayout.Add(stats);
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
                field.Text = fieldData.Current.ToString($"{precision}");
                stats.Text = fieldData.Min.ToString($"{precision}") + " - " + fieldData.Average.ToString($"{precision}") + " -" + fieldData.Max.ToString($"{precision}");
            }
        }

        public void Resize(double width, double height)
        {
            double baseSize = Math.Min(width, height);

            double headerSize = baseSize * 0.012; // e.g., "Heading"
            double valueSize = baseSize * 0.072; // e.g., "123.45"

            title.FontSize = headerSize;
            stats.FontSize = valueSize;
            stats.FontSize = headerSize;
        }
    }
}
