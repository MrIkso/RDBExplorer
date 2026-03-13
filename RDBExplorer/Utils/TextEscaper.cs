using System.Text;
using System.Text.RegularExpressions;

namespace RDBExplorer.Utils
{
    public static class TextEscaper
    {
        private static readonly Dictionary<char, string> EscapeTable = new Dictionary<char, string>
        {
            { '\r', "{cr}" },
            { '\n', "{lf}" },
            { '\t', "{tab}" },
            { '\0', "{nul}" },
        };

        public static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                // check if it's in our manual EscapeTable (for {lf}, {cr}, etc.)
                if (EscapeTable.TryGetValue(c, out string tag))
                {
                    sb.Append(tag);
                }
                // Check if the character is a "control" or "non-printable" character
                // We allow standard alphanumeric, symbols and space.
                // Also check for Private Use Area (E000-F8FF) which is common in games.
                else if (char.IsControl(c) || (c >= '\uE000' && c <= '\uF8FF'))
                {
                    // Hex format: {UXXXX}
                    sb.Append($"{{U{(int)c:X4}}}");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string Unescape(string text)
        {
            if (string.IsNullOrEmpty(text)) 
                return text;

            string result = text;
            foreach (var pair in EscapeTable)
            {
                result = result.Replace(pair.Value, pair.Key.ToString());
            }

            result = Regex.Replace(result, @"\{U([0-9A-Fa-f]{4})\}", m =>
                ((char)Convert.ToUInt16(m.Groups[1].Value, 16)).ToString());

            return result;
        }
    }
}
