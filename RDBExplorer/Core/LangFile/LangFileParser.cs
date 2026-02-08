using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace RDBExplorer.Core.LangFile
{
    public class LangFileParser
    {
        public List<KTGLString> Strings = new List<KTGLString>();

        public List<KTGLString> ParseLangFile(string filePath)
        {
            var entries = new List<KTGLString>();
            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                while (reader.BaseStream.Position + 8 <= reader.BaseStream.Length)
                {
                    var stringEntry = new KTGLString
                    {
                        Hash = reader.ReadInt32(),
                        Length = reader.ReadInt32(),
                    };

                    if (stringEntry.Length == 1)
                    {
                        byte[] stringBytes = reader.ReadBytes(2);
                        stringEntry.Value = Encoding.Unicode.GetString(stringBytes).TrimEnd('\0');
                    }
                    else
                    {
                        byte[] stringBytes = reader.ReadBytes(stringEntry.Length * 2);
                        stringEntry.Value = Encoding.Unicode.GetString(stringBytes).TrimEnd('\0');
                    }

                    entries.Add(stringEntry);
                }
            }
            Strings = entries;
            return entries;
        }

        public void SaveStringsToCsv(string savePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                NewLine = Environment.NewLine,
                ShouldQuote = args => true
            };

            using (var writer = new StreamWriter(savePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(Strings);
            }
        }

        public void LoadStringsFromCsv(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
            };

            using (var reader = new StreamReader(csvPath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                Strings = csv.GetRecords<KTGLString>().ToList();
            }
        }

        public void SaveBinaryFile(string filePath)
        {
            using (var writer = new BinaryWriter(File.Create(filePath)))
            {
                foreach (var str in Strings)
                {
                    string textToWrite = (str.Value ?? "") + "\0";
                    byte[] data = Encoding.Unicode.GetBytes(textToWrite);
                    int newLength = textToWrite.Length;

                    writer.Write(str.Hash);
                    writer.Write(newLength);
                    writer.Write(data);
                }
            }
        }
    }
}
