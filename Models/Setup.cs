using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace SailMonitor.Models
{

    public class Setup
    {
        public int Port;
        public bool Night;
        public int Orientation;
        public bool KeepActive;
        public bool UseGPSPOS;
        public bool UseGPSHEADING;
        public bool UseGPSSOG;
        public bool Fakewind;


        public Setup()
        {
            Port = Preferences.Get("Port", 10110);
            Night = Preferences.Get("Night", false);
            Orientation = Preferences.Get("Orientation", 1);
            KeepActive = Preferences.Get("KeepActive", true);
            UseGPSPOS = Preferences.Get("UseGPSPOS", true);
            UseGPSHEADING = Preferences.Get("UseGPSHEADING", true);
            UseGPSSOG = Preferences.Get("UseGPSSPOG", true);
            Fakewind = false;
        }

        public void Save()
        {
            Preferences.Set("Port", Port);
            Preferences.Set("Night", Night);
            Preferences.Set("Orientation", Orientation);
            Preferences.Set("KeepActive", KeepActive);
            Preferences.Set("UseGPSPOS", UseGPSPOS);
            Preferences.Set("UseGPSHEADING", UseGPSHEADING);
            Preferences.Set("UseGPSSOG", UseGPSSOG);
        }

        public Setup Copy()
        {
            Setup P = new Setup();
            P.Night = Night;
            P.Port = Port;
            P.Orientation = Orientation;
            P.KeepActive = KeepActive;
            P.UseGPSPOS = UseGPSPOS;

            P.UseGPSHEADING = UseGPSHEADING;
            P.UseGPSSOG = UseGPSSOG;
            P.Fakewind = Fakewind;
            return P;

        }
    }
}
