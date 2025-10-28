using SailMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SailMonitor.Services
{
    public interface IContentViewHost 
    {
        void OnAppEvent(string eventName, Record record);
    }
}
