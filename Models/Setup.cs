namespace SailMonitor.Models
{
    public class Setup
    {
        public int Port;
        public bool Night;
        public bool KeepActive;
        public bool UseGPSPOS;
        public bool UseGPSHEADING;
        public bool UseGPSSOG;
        public Color foreColor;
        public Color backColor;
        public int saveFrequency;
        public UnitOfMeasure Depth;
        public UnitOfMeasure Speed;
        public UnitOfMeasure WindSpeed;

        public Setup()
        {
            Port = Preferences.Get("Port", 10110);
            Night = Preferences.Get("Night", false);
            KeepActive = Preferences.Get("KeepActive", true);
            UseGPSPOS = Preferences.Get("UseGPSPOS", true);
            UseGPSHEADING = Preferences.Get("UseGPSHEADING", true);
            UseGPSSOG = Preferences.Get("UseGPSSPOG", true);
            saveFrequency = Preferences.Get("saveFrequency", 15);
            SetColor();
            Depth = new UnitOfMeasure
            {
                Name = "Depth",
                Internal = Units.Meters,
                UnitList = new List<Unit>
                {
                    new Unit(Units.Meters, 1.0),
                    new Unit(Units.Feet, 3.28084),
                    new Unit(Units.Fathoms, 0.546807)
                }
            };
            Depth.SelectedUnit = Preferences.Get("DepthUnit", Units.Feet);
            Speed = new UnitOfMeasure
            {
                Name = "Speed",
                Internal = Units.Knots,
                UnitList = new List<Unit>
                {
                    new Unit(Units.Knots, 1.0),
                    new Unit(Units.KPH, 1.852),
                    new Unit(Units.MPH, 1.15078),
                }
            };
            Speed.SelectedUnit = Preferences.Get("SpeedUnit", Units.Knots);
            WindSpeed = new UnitOfMeasure
            {
                Name = "Wind Speed",
                Internal = Units.Knots,
                UnitList = new List<Unit>
                {
                    new Unit(Units.Knots, 1.0),
                    new Unit(Units.KPH, 0.539957),
                    new Unit(Units.MPH, 0.868976),
                    new Unit(Units.MPS, 1.94384)
                }
            };
            WindSpeed.SelectedUnit = Preferences.Get("WindSpeedUnit", "Mph");
        }

        public void Save()
        {
            Preferences.Set("Port", Port);
            Preferences.Set("Night", Night);

            Preferences.Set("KeepActive", KeepActive);
            Preferences.Set("UseGPSPOS", UseGPSPOS);
            Preferences.Set("UseGPSHEADING", UseGPSHEADING);
            Preferences.Set("UseGPSSOG", UseGPSSOG);
            Preferences.Set("saveFrequency", saveFrequency);
            Preferences.Set("DepthUnit", Depth.SelectedUnit);
            Preferences.Set("SpeedUnit", Speed.SelectedUnit);
            Preferences.Set("WindSpeedUnit", WindSpeed.SelectedUnit);

            SetColor();
        }

        public void SetColor()
        {
            if (Night == false)
            {
                foreColor = Colors.Black;
                backColor = Colors.White;
            }
            else
            {
                foreColor = Colors.Red;
                backColor = Colors.Black;
            }
        }
    }
}
