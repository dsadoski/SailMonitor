using SailMonitor.Models;

namespace SailMonitor.Services
{
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

        public WindPointDisplay(string Name1, string Name2, Grid owner, Setup setup, int Row, int Column, string Precision, string Description)
        {
            description = Description;
            column = Column;
            row = Row;
            name1 = Name1;
            name2 = Name2;
            precision = Precision;
            fieldDataDir = new FieldData(name1);
            grid = owner;
            verticalStackLayout = new VerticalStackLayout();
            title = new Label();
            title.Text = description;
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
            grid.SetRow(verticalStackLayout, row);
            grid.SetColumn(verticalStackLayout, column);
        }

        public void OnAppEvent(string eventName, Record record, List<FieldData> DataPoints)
        {
            fieldDataDir = DataPoints.FirstOrDefault(d => d.name == name1);
            fieldDir.Text = fieldDataDir.Current.ToString($"{precision}") + "°";

            fieldDataSpd = DataPoints.FirstOrDefault(d => d.name == name2);
            fieldSpd.Text = fieldDataSpd.Current.ToString($"{precision}");
        }

        public void Update(List<FieldData> DataPoints)
        {
            fieldDataDir = DataPoints.FirstOrDefault(d => d.name == name1);
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

            fieldDataSpd = DataPoints.FirstOrDefault(d => d.name == name2);
            if (fieldDataSpd != null)
            {
                fieldSpd.Text = fieldDataSpd.Current.ToString($"{precision}");
                statsSpd.Text = fieldDataSpd.Min.ToString($"{precision}") + " - " + fieldDataSpd.Average.ToString($"{precision}") + " -" + fieldDataSpd.Max.ToString($"{precision}");
            }
        }

        public void Resize(double Width, double Height)
        {
            double baseSize = Math.Min(Width, Height);

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
