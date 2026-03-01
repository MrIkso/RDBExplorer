namespace RDBExplorer.Core.Formats.Bytecode
{
    public class SctriptNameResolver
    {
        private ScriptFile _script;
        public SctriptNameResolver(ScriptFile script) => _script = script;

        public string GetNameFromSymbol(ushort symbolIndex)
        {
            if (symbolIndex == 0xFFFF) 
                return "null";

            if (symbolIndex < _script.Symbols.Count)
            {
                var symbol = _script.Symbols[symbolIndex];

                if (symbol.Id < _script.AccessorNames.Count)
                {
                    var nameEntry = _script.AccessorNames[symbol.Id];
                    return nameEntry.Name;
                }
                return $"Symbol(Kind:{symbol.Kind}, Id:{symbol.Id}, Index:{symbol.Index})";
            }

            return $"UnkSymbol_{symbolIndex}";
        }
    }
}
