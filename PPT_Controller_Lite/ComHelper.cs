using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.ComponentModel;

namespace PPT_Controller_Lite
{
    public static class ComHelper
    {
        public static bool IsProgIDInstalled(string progID)
        {
            return Registry.ClassesRoot.GetSubKeyNames().Contains(progID);
        }

        public static object CreateInstanceFromProgID(string progID, bool throwOnError = true)
        {
            Type type = Type.GetTypeFromProgID(progID, throwOnError);
            return Activator.CreateInstance(type);
        }
    }
}
