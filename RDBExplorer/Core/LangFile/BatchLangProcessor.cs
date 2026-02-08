using System;
using System.Collections.Generic;
namespace RDBExplorer.Core.LangFile
{
    public class BatchLangProcessor
    {
        public static void ParseFromDir(string dir, string saveOutDir)
        {
            string saveDir = Path.Combine(saveOutDir, "Export");
            if (!Directory.Exists(saveDir)) {
                Directory.CreateDirectory(saveDir);
            }

            var files = Directory.GetFiles(dir, "*.dat");

            foreach (var file in files)
            {
                string saveFileName = $"{Path.GetFileNameWithoutExtension(file)}.csv";
                string savePath = Path.Combine(saveDir, saveFileName);
                LangFileParser langFileParser = new LangFileParser();
                langFileParser.ParseLangFile(file);
                langFileParser.SaveStringsToCsv(savePath);
            }
        }

        public static void ConvertToBinary(string dir, string saveOutDir)
        {
            string saveDir = Path.Combine(saveOutDir, "Result");
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            var files = Directory.GetFiles(dir, "*.csv");

            foreach (var file in files)
            {
                string saveFileName = $"{Path.GetFileNameWithoutExtension(file)}.dat";
                string savePath = Path.Combine(saveDir, saveFileName);
                LangFileParser langFileParser = new LangFileParser();
              
                langFileParser.LoadStringsFromCsv(file);
                langFileParser.SaveBinaryFile(savePath);
            }
        }

    }
}
