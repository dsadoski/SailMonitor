namespace SailMonitor.Models
{
    public class Record
    {
        public int ID;
        public int userId;
        public int boatId;
        public double latitude;
        public double longitude;
        public long gpsTicks;
        public long time;
        public IOVariable depth;
        public double headingMag;
        public double headingTrue;
        public double SOG;
        public double COG;
        public double SOW;
        public double windTrueDir;
        public IOVariable windTrueSpeed;
        public double windTrueCompass;
        public double windAppDir;
        public IOVariable windAppSpeed;
        public double VPWSPD;
        public int POLARDATA;
        public double waterTemp;
        public double voltage;
        public string ErrMessage;
        public Location location;

        public Record()
        {
            ErrMessage = string.Empty;

            ID = 0;
            userId = 0;
            boatId = 0;
            latitude = 0;
            longitude = 0;
            gpsTicks = 0;
            time = 0;
            depth = new IOVariable(Units.Meters, "Depth");
            headingMag = 0;
            headingTrue = 0;
            SOG = 0;
            COG = 0;
            SOW = 0;
            windTrueDir = 0;
            windTrueSpeed = new IOVariable(Units.Knots, "Wind Speed");
            windTrueCompass = 0;
            windAppDir = 0;
            windAppSpeed = new IOVariable(Units.Knots, "Wind Speed");
            VPWSPD = 0;
            POLARDATA = 0;
            waterTemp = 0;
            voltage = 0;
            location = new Location();
        }

        public Record Copy()
        {
            Record P = new Record();
            P.ID = ID;
            P.userId = userId;
            P.boatId = boatId;
            P.latitude = latitude;
            P.longitude = longitude;
            P.gpsTicks = gpsTicks;
            P.time = time;
            P.depth = depth;
            P.headingMag = headingMag;
            P.headingTrue = headingTrue;
            P.SOG = SOG;
            P.COG = COG;
            P.SOW = SOW;
            P.windTrueDir = windTrueDir;
            P.windTrueSpeed = windTrueSpeed;
            P.windTrueCompass = windTrueCompass;
            P.windAppDir = windAppDir;
            P.windAppSpeed = windAppSpeed.Copy();
            P.VPWSPD = VPWSPD;
            P.ErrMessage = ErrMessage;
            P.waterTemp = waterTemp;
            P.voltage = voltage;

            if (location != null)
            {
                P.location = new Location(location);
            }
            else
            {
                location = new Location();
            }

            return P;
        }

        public void Makefromlive()
        {
            userId = 1;
            boatId = 1;
            time = System.DateTime.Now.Ticks;
        }
    }
}
