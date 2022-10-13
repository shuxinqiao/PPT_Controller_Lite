using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPT_Controller_Lite
{
    public static class PowerPointHelper
    {
        public static bool IsPowerPointInstalled()
        {
            return ComHelper.IsProgIDInstalled("PowerPoint.Application");
        }

        public static object CreatePowerPointApplication()
        {
            return ComHelper.CreateInstanceFromProgID("PowerPoint.Application");
        }

        public static object CreatePowerPointApplication(this COMReferenceTracker t)
        {
            var app = CreatePowerPointApplication();
            return t.T(app);
        }
    }
}
