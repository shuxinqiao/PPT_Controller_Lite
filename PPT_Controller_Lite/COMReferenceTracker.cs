using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PPT_Controller_Lite
{
    public class COMReferenceTracker : IDisposable
    {
        private List<dynamic> objects = new List<dynamic>();

        public dynamic T(dynamic obj)
        {
            if (Marshal.IsComObject(obj) == false)
            {
                throw new ArgumentException("obj is not a ComObject.");
            }
            lock (objects)
            {
                objects.Add(obj);
                return obj;
            }
        }

        public void Dispose()
        {
            foreach (var obj in objects)
            {
                try
                {
                    Marshal.FinalReleaseComObject(obj);
                }
                catch (InvalidComObjectException ex)
                {
                    Debug.WriteLine(ex);
                }
                catch (COMException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
