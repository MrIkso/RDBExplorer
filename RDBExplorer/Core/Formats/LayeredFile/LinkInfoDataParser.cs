using System.Text.RegularExpressions;

namespace RDBExplorer.Core.Formats.LayeredFile
{
    public class LinkInfoDataParser
    {
        public static List<string> ParseLinkInfo(string linkInfoPath)
        {
            if (!File.Exists(linkInfoPath)) 
                return new List<string>();

            string content = File.ReadAllText(linkInfoPath);
            List<string> enumMembers = ParseEnumToArray(content);
            List<string> fileNames = new List<string>();
            foreach (string enumMember in enumMembers)
            {
                string[] parts = enumMember.Split('_');
                if (parts.Length >= 4)
                {
                    string extension = parts[parts.Length - 1].ToLower();

                    int skipCount = 2;
                    int takeCount = parts.Length - skipCount - 1;

                    string baseName = string.Join("_", parts, skipCount, takeCount);
                    string fileName = $"{baseName}.{extension}";

                    fileNames.Add(fileName);
                }
            }

            return fileNames;
        }

        public static List<string> ParseEnumToArray(string input)
        {
            string pattern = @"INDEX_K300_[A-Z0-9_]+";

            MatchCollection matches = Regex.Matches(input, pattern);

            return matches
                .Cast<Match>()
                .Select(m => m.Value)
                .Where(v => v != "INDEX_K300_MAX")
                .ToList();
        }
    }
}
