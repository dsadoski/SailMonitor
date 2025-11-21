namespace SailMonitor.Services
{
    using SailMonitor.Models;

    public class WindPointDisplay
    {
        public Label title;
        public Label fieldDir;
        public Label fieldSpd;
        public Label statsDir;
        public Label statsSpd;
        public string name1;
        public string name2;
        public VerticalStackLayout verticalStackLayout;
        private Grid grid;
        private FieldData fieldDataDir;
        private FieldData fieldDataSpd;
        private string precision;
        private int column;
        private int row;
        private string description;

        public WindPointDisplay(string name1, string name2, Grid owner, Setup setup, int row, int column, string precision, string description)
        {
            this.description = description;
            this.column = column;
            this.row = row;
            this.name1 = name1;
            this.name2 = name2;
            this.precision = precision;
            fieldDataDir = new FieldData(this.name1);
            grid = owner;
            verticalStackLayout = new VerticalStackLayout();
            title = new Label();
            title.Text = this.description;
            title.FontSize = 16;
            fieldDir = new Label();

            //fieldDir.FontSize = 36;
            fieldSpd = new Label();

            //fieldSpd.FontSize = 36;

            statsDir = new Label();
            statsDir.FontSize = 12;
            statsSpd = new Label();
            statsSpd.FontSize = 12;

            title.TextColor = setup.foreColor;
            fieldDir.TextColor = setup.foreColor;
            fieldSpd.TextColor = setup.foreColor;
            statsDir.TextColor = setup.foreColor;
            statsSpd.TextColor = setup.foreColor;

            verticalStackLayout.Add(title);
            verticalStackLayout.Add(fieldDir);
            verticalStackLayout.Add(statsDir);
            verticalStackLayout.Add(fieldSpd);
            verticalStackLayout.Add(statsSpd);

            grid.Children.Add(verticalStackLayout);
            grid.SetRow(verticalStackLayout, this.row);
            grid.SetColumn(verticalStackLayout, this.column);
        }

        public void OnAppEvent(string eventName, Record record, List<FieldData> dataPoints)
        {
            fieldDataDir = dataPoints.FirstOrDefault(d => d.name == name1);
            fieldDir.Text = fieldDataDir.Current.ToString($"{precision}") + "°";

            fieldDataSpd = dataPoints.FirstOrDefault(d => d.name == name2);
            fieldSpd.Text = fieldDataSpd.Current.ToString($"{precision}");
        }

        public void Update(List<FieldData> dataPoints)
        {
            fieldDataDir = dataPoints.FirstOrDefault(d => d.name == name1);
            if (fieldDataDir != null)
            {
                var current = fieldDataDir.Current;

                string txt = string.Empty;
                if (name1 == "AWD")
                {
                    txt = fieldDataDir.Current.ToString($"{precision}") + "°";
                    if (current > 180)
                    {
                        current = 360 - current;
                        fieldDir.TextColor = Colors.Red;
                        txt = current.ToString($"{precision}") + "°" + "P";
                    }
                    else
                    {
                        fieldDir.TextColor = Colors.Green;
                        txt = current.ToString($"{precision}") + "°" + "S";
                    }
                }
                else
                {
                    txt = fieldDataDir.Current.ToString($"{precision}") + "°";
                }

                fieldDir.Text = txt;
                statsDir.Text = fieldDataDir.Min.ToString($"{precision}") + " - " + fieldDataDir.Average.ToString($"{precision}") + " -" + fieldDataDir.Max.ToString($"{precision}");
            }

            fieldDataSpd = dataPoints.FirstOrDefault(d => d.name == name2);
            if (fieldDataSpd != null)
            {
                fieldSpd.Text = fieldDataSpd.Current.ToString($"{precision}");
                statsSpd.Text = fieldDataSpd.Min.ToString($"{precision}") + " - " + fieldDataSpd.Average.ToString($"{precision}") + " -" + fieldDataSpd.Max.ToString($"{precision}");
            }
        }

        public void Resize(double width, double height)
        {
            double baseSize = Math.Min(width, height);

            double headerSize = baseSize * 0.012; // e.g., "Heading"
            double valueSize = baseSize * 0.072; // e.g., "123.45"

            title.FontSize = headerSize;
            fieldSpd.FontSize = valueSize;
            statsSpd.FontSize = headerSize;
            fieldDir.FontSize = valueSize;
            statsDir.FontSize = headerSize;
        }

        public void OnSetupChanged(Setup settings)
        {
            title.TextColor = settings.foreColor;
            fieldDir.TextColor = settings.foreColor;
        }
    }
}
