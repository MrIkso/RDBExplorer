namespace RDBExplorer.Core.Formats.LangFile
{
    public class BatchLangProcessor
    {
        /// <summary>
        /// Extracts strings from binary files (.dat, .bin) and saves them as CSV files.
        /// </summary>
        /// <param name="dir">Input directory containing binary files.</param>
        /// <param name="saveOutDir">Base directory for output.</param>
        /// <param name="useNewParser">Switch between NewLangFileParser and the original LangFileParser.</param>
        public static void ParseFromDir(string dir, string saveOutDir, bool useNewParser = false)
        {
            // Set up export subdirectory
            string saveDir = Path.Combine(saveOutDir, "Export");
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            // Define supported binary extensions
            var extensions = new[] { ".dat", ".bin" };

            // Search for files matching the extensions (case-insensitive)
            var files = Directory.EnumerateFiles(dir, "*.*")
                                 .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()));

            foreach (var file in files)
            {
                try
                {
                    // Construct output path: e.g., "text_en.bin.csv"
                    string saveFileName = $"{Path.GetFileName(file)}.csv";
                    string savePath = Path.Combine(saveDir, saveFileName);

                    if (useNewParser)
                    {
                        var parser = new NewLangFileParser();
                        parser.ParseLangFile(file);
                        parser.SaveStringsToCsv(savePath);
                    }
                    else
                    {
                        var parser = new LangFileParser();
                        parser.ParseLangFile(file);
                        parser.SaveStringsToCsv(savePath);
                    }

                    Console.WriteLine($"[Success] Exported: {Path.GetFileName(file)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to parse {file}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Reads CSV files and converts them back into binary format.
        /// </summary>
        /// <param name="dir">Directory containing the edited CSV files.</param>
        /// <param name="saveOutDir">Base directory for output.</param>
        /// <param name="useNewParser">Switch between NewLangFileParser and the original LangFileParser.</param>
        public static void ConvertToBinary(string dir, string saveOutDir, bool useNewParser = false)
        {
            // Set up result subdirectory
            string saveDir = Path.Combine(saveOutDir, "Result");
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            // Search for all CSV files in the directory
            var files = Directory.GetFiles(dir, "*.csv");

            foreach (var file in files)
            {
                try
                {
                    string saveFileName = Path.GetFileNameWithoutExtension(file);
                    string savePath = Path.Combine(saveDir, saveFileName);

                    if (useNewParser)
                    {
                        var parser = new NewLangFileParser();
                        parser.LoadStringsFromCsv(file);
                        parser.SaveBinaryFile(savePath);
                    }
                    else
                    {
                        var parser = new LangFileParser();
                        parser.LoadStringsFromCsv(file);
                        parser.SaveBinaryFile(savePath);
                    }

                    Console.WriteLine($"[Success] Compiled: {saveFileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to convert {file}: {ex.Message}");
                }
            }
        }
    }
}