using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Utils
{
    public class FileUtils
    {
        private const long MaxMemorySize = 2 * 1024 * 1024; // 2 MB

        public static bool IsBigText(string text)
        {
            if (text.Length * sizeof(char) > MaxMemorySize)
            {
                return true;
            }
            return false;
        }


        public static void WriteFile(string path, string text)
        {
            File.WriteAllTextAsync(path, text).Wait();
        }

    }
}
