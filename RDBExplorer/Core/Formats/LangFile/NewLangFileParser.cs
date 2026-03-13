using CsvHelper;
using CsvHelper.Configuration;
using RDBExplorer.Utils;
using System.Globalization;
using System.Text;

namespace RDBExplorer.Core.Formats.LangFile
{
    public class StringEntry
    {
        public int Index { get; set; }
        public int LanguageIndex { get; set; }
        public int LangCount { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class NewLangFileParser
    {
        public List<StringEntry> Strings = new List<StringEntry>();
     
        public List<StringEntry> ParseLangFile(string filePath)
        {
            var entries = new List<StringEntry>();
            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                int tableCount = reader.ReadInt32();
                if (tableCount <= 0)
                {
                    throw new Exception("Not a valid language file");
                }

                // read table pointers

                int[] pointers = new int[tableCount];
                for (int i = 0; i < tableCount; i ++)
                {
                    pointers[i] = reader.ReadInt32();
                }

                for (int j = 0; j < tableCount; j ++)
                {
                    int dataStartPointer = pointers[j];
                    if (dataStartPointer == 0)
                    {
                        continue;
                    }
                    reader.BaseStream.Seek(dataStartPointer, SeekOrigin.Begin);
                    int stringCount = reader.ReadInt32();
                    if (stringCount == 0)
                    {
                        continue;
                    }
  
                    int[] offsets = new int[stringCount];
                    for (int i = 0; i < stringCount; i++)
                    {
                        offsets[i] = reader.ReadInt32();
                    }

                    for (int i = 0; i < stringCount; i++)
                    {
                        if (offsets[i] == 0)
                        {
                            continue;
                        }
                        reader.BaseStream.Seek(dataStartPointer + offsets[i], SeekOrigin.Begin);

                        var result = reader.ReadNullTerminatedUnicode();
                        string cleanText = TextEscaper.Escape(result);

                        entries.Add(new StringEntry
                        {
                            Index = i,
                            LangCount = tableCount,
                            LanguageIndex = j,
                            Text = cleanText,
                            //IsUTF8 = !result.isUtf16
                        });
                    }
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
                Strings = csv.GetRecords<StringEntry>().ToList();
            }
        }

        public void SaveBinaryFile(string filePath)
        {
            if (Strings.Count == 0)
                return;

            int langIdx = Strings[0].LanguageIndex;
            int langCount = Strings[0].LangCount;

            using (var stream = File.Create(filePath))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(langCount); // total language slots
                long headerPointersStart = stream.Position;

                // temporary array to store pointers for the header
                int[] headerPointers = new int[langCount];
                // skip space for langCount pointers (will fill later)
                for (int i = 0; i < langCount; i++)
                {
                    writer.Write(0);
                }

                writer.PadToAlignment(16);

                // need to iterate through all language slots and write either our real data or a dummy table
                for (int i = 0; i < langCount; i++)
                {
                    // save the current position as the pointer for this language slot
                    headerPointers[i] = (int)stream.Position;

                    if (i == langIdx)
                    {
                        // write current languge string table
                        long dataStartPos = stream.Position;
                        writer.Write(Strings.Count);

                        long offsetsTablePos = stream.Position;
                        // placeholder for offsets
                        for (int s = 0; s < Strings.Count; s++)
                        {
                            writer.Write(0);
                        }
                        writer.PadToAlignment(16);

                        int[] offsets = new int[Strings.Count];
                        for (int s = 0; s < Strings.Count; s++)
                        {
                            offsets[s] = (int)(stream.Position - dataStartPos);
                            string cleanText = TextEscaper.Unescape(Strings[s].Text);
                            byte[] data = Encoding.Unicode.GetBytes(cleanText);
                            writer.Write(data);
                            writer.Write((short)0); // 0x00 00
                            writer.PadToAlignment(16);
                        }

                        // go back and fill the offsets for this specific table
                        long currentPos = stream.Position;
                        stream.Seek(offsetsTablePos, SeekOrigin.Begin);
                        foreach (int off in offsets)
                        {
                            writer.Write(off);
                        }
                        stream.Seek(currentPos, SeekOrigin.Begin);
                    }
                    else
                    {
                        // write valid empty table has 0 strings and padding
                        writer.Write(0); // stringCount = 0
                        writer.PadToAlignment(16);
                    }
                }

                long endOfFile = stream.Position;
                stream.Seek(headerPointersStart, SeekOrigin.Begin);
                for (int i = 0; i < langCount; i++)
                {
                    writer.Write(headerPointers[i]);
                }

                stream.Seek(endOfFile, SeekOrigin.Begin);
            }
        }
    }
}
