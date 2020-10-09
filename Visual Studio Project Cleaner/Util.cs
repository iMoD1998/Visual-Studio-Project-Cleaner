using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_Studio_Project_Cleaner
{
    class Util
    {
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string ConvertBytesToString(long Size)
        {
            if (Size == 0)
                return "0 " + SizeSuffixes[0];
            ;
            int Magnitude = (int)Math.Log(Size, 1024);
            double AdjustedSize = (Size / Math.Pow(1024, Magnitude));

            return string.Format("{0:n2} {1}", AdjustedSize, SizeSuffixes[Magnitude]);
        }
    }
}
