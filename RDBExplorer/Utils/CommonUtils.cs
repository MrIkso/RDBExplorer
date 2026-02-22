using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Utils
{
    internal class CommonUtils
    {
        public static int ParseVersion(uint version)
        {
            int b4 = (int)((version >> 24) & 0xFF) - 0x30;
            int b3 = (int)((version >> 16) & 0xFF) - 0x30; 
            int b2 = (int)((version >> 8) & 0xFF) - 0x30; 
            int b1 = (int)(version & 0xFF) - 0x30;

            return (b4 * 1000) + (b3 * 100) + (b2 * 10) + b1;
        }
    }
}
