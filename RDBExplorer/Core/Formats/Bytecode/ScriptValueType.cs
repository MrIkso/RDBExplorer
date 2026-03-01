namespace RDBExplorer.Core.Formats.Bytecode
{
    public enum KtglType : ushort
    {
        Unknown = 0,
        Integer = 1,         // GetInteger
        Boolean = 2,         // GetBoolean  
        Decimal = 3,         // GetDecimal / float
        String = 4,          // GetCstring
        CodeOffset = 5,      // GetCodeOffset
        BinderPosition = 6,  // GetBinderPosition
                             // 7 = ?            
                             // 8 = input type Assign, converted → 11, is NOT a separate kind
        Composition = 9,     // AssignToComposition
        External = 10,       // GetExternalVariable
        LocalVarRef = 11,    // local ref
                             // 12-14 = ?
        GlobalVarRef = 15,   // gloabal ref
    }

}
