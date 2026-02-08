using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDBExplorer.Utils
{
    public class Hasher
    {

        public static int HashPath(string path)
        {
            string normalPath = "/" + path.Replace("\\", "/").ToLower();
            return CalculateFNV1(normalPath);
        }

        public static int CalculateFNV1(string str)
        {
            uint hash = 0x811C9DC5;
            byte[] data = Encoding.UTF8.GetBytes(str);

            foreach (byte b in data)
            {
                hash *= 0x1000193;
                hash ^= b;
            }

            return unchecked((int)hash);
        }
    }
}
