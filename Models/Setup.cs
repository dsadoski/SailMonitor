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
        public bool KeepActive;
        public bool UseGPSPOS;
        public bool UseGPSHEADING;
        public bool UseGPSSOG;

        public Color foreColor;
        public Color backColor;
        


        public Setup()
        {
            Port = Preferences.Get("Port", 10110);
            Night = Preferences.Get("Night", false);
            
            KeepActive = Preferences.Get("KeepActive", true);
            UseGPSPOS = Preferences.Get("UseGPSPOS", true);
            UseGPSHEADING = Preferences.Get("UseGPSHEADING", true);
            UseGPSSOG = Preferences.Get("UseGPSSPOG", true);
            SetColor();
            
        }

        public void Save()
        {
            Preferences.Set("Port", Port);
            Preferences.Set("Night", Night);
            
            Preferences.Set("KeepActive", KeepActive);
            Preferences.Set("UseGPSPOS", UseGPSPOS);
            Preferences.Set("UseGPSHEADING", UseGPSHEADING);
            Preferences.Set("UseGPSSOG", UseGPSSOG);
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
