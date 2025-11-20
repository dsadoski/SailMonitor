using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
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
        FieldData fieldData;
        string precision;
        int column;
        int row;
        string description;

        public FieldDisplay(string Name, Grid owner, Setup setup, int Row, int Column, string Precision, string description)
        {
            column = Column;
            row = Row;
            name = Name;
            precision = Precision;
            fieldData = new FieldData(name);
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
            grid.SetRow(verticalStackLayout, row);
            grid.SetColumn(verticalStackLayout, column);
            this.description = description;
        }


        public void Update(List<FieldData> DataPoints)
        {
            fieldData = DataPoints.FirstOrDefault(d => d.name == name);
            if (fieldData != null)
            {
                
                field.Text = fieldData.Current.ToString($"{precision}");
                stats.Text = fieldData.Min.ToString($"{precision}") + " - " + fieldData.Average.ToString($"{precision}") + " -" + fieldData.Max.ToString($"{precision}");
            }


        }

        public void Resize(double Width, double Height)
        {
            double baseSize = Math.Min(Width, Height);

            double headerSize = baseSize * 0.012; // e.g., "Heading"
            double valueSize = baseSize * 0.072; // e.g., "123.45"

            

            title.FontSize = headerSize;
            stats.FontSize = valueSize;
            stats.FontSize = headerSize;
        }
    }
}
