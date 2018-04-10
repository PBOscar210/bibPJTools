using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibPJTools
{
    public static class ValTools
    {
        public static string valOrNull(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return null;
            else
                return val;
        }

        public static long? valOrNull(long val)
        {
            if (val == 0)
                return null;
            else
                return val;
        }

        public static Guid? valOrNull(Guid val)
        {
            if (val == Guid.Empty)
                return null;
            else
                return val;
        }
    }
}
