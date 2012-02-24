using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CommonLibrary.Utility
{
    public class DebugHelper
    {
        [Conditional("DEBUG"),Conditional("TRACE")]
        public void CheckState()
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            Trace.WriteLine("Entering CheckState for DOSearch:");
            Trace.Write("\tCalled by ");
            Trace.WriteLine(methodName);
            Debug.Assert(true, methodName, "** cannot be null");
            Trace.WriteLine("Exiting CheckState for DOSearch");
        }
    }
}
