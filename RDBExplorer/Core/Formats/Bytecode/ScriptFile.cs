namespace RDBExplorer.Core.Formats.Bytecode
{
    public class ScriptFile
    {
        public string Version { get; set; }
        public List<AccessorNameEntry> AccessorNames { get; set; } = new();
        public List<SymbolEntry> Symbols { get; set; } = new();
        public List<ExternalEntry> Externals { get; set; } = new();
        public List<ushort[]> IdTables { get; set; } = new();
        public List<BuiltInEntry> BuiltIns { get; set; } = new();
        public List<string> Literals { get; set; } = new();
        public byte[] Bytecode { get; set; }

        public struct AccessorNameEntry
        {
            public uint Hash { get; set; }
            public string Name { get; set; }
        }

        public struct SymbolEntry
        {
            public ushort Kind { get; set; }
            public ushort Index { get; set; }
            public ushort Id { get; set; }
        }

        public struct ExternalEntry
        {
            public ushort Kind { get; set; }
            public ushort Val1 { get; set; }
        }

        public struct BuiltInEntry
        {
            public ushort Id { get; set; }
            public ushort Index { get; set; }
        }

        public enum OpCode : ushort
        {
            ASSIGN_VAR = 1,
            ASSIGN_STACK = 2,
            PUSH_ENV_REG = 3,
            ADD = 4,
            LOGIC_AND_STRIP = 5,
            BIT_AND = 6,
            BIT_OR = 7,
            BIT_XOR = 8,
            DIV = 9,
            CMP_EQ = 10,
            CMP_GT = 11,
            CMP_GE = 12,
            CMP_LT = 13,
            CMP_LE = 14,
            MUL = 15,
            CMP_NE = 16,
            JUMP_IF_TRUE = 17, // read Int32
            SUB = 18,
            MOD = 19,
            PUSH_FALSE = 21,
            PUSH_TRUE = 22,
            CALL_DEREF_FUNC = 25, // read u16
            CALL_HOST_VAR = 27,
            CALL_HOST_REG_LOAD = 28,
            CALL_HOST_REG_READ = 29,
            CALL_INTERNAL = 31, // read u16
            GOTO_FORWARD = 34, // read Int32
            GOTO_BACKWARD = 35, // read Int32
            JUMP_IF_FALSE = 36, // read Int32
            POP_ENV = 37,
            POP_ENV_STACK = 38,
            RETURN_INTERNAL = 39,
            RETURN_UNWIND = 40,
            PUSH_INT = 41, // read Int32
            PUSH_INT_0 = 42,
            PUSH_INT_1 = 43,
            LITERAL_INTS = 44,
            PUSH_FLOAT = 45, // read Float
            LITERAL_FLOATS = 46,
            PUSH_STR_LIT = 47, // read u16
            PUSH_COMPLEX = 48, // read u32
            JUMP_IF_NOT = 49, // read Int32
            UNARY_MINUS = 50,
            LOGIC_NOT = 51,
            STACK_FRAME_POP = 52,
            WIND_FRAME = 53, // read u16 (count)
            TO_FLOAT = 54,
            TO_INT_CEIL = 55,
            TO_INT_FLOOR = 56,
            TO_INT_ROUND = 57,
            GET_VAR_GLOBAL = 58, // read u16
            GET_VAR_REFERRED = 59, // read u16
            GET_VAR_LOCAL_60 = 60, // read u16, u16
            GET_VAR_LOCAL_61 = 61, // read u16, u16
            GET_VAR_LOCAL_62 = 62, // read u16, u16
            GET_LOCAL_REG = 63,
            DEREF_VAR = 64, // read u16
            DEREF_GLOBAL_VAL = 65,
            DEREF_LOCAL = 67,
            DEREF_LOCAL_VAL = 68,
            ARRAY_REF_CHILD = 73,
            ARRAY_GET_VAL = 74,
            EXPAND_ARRAY = 76,
            GET_PROP_OBJECT = 77, // read u16
            GET_PROP_VALUE = 78, // read u16
            LOAD_FRAME = 80, // read u16
            STORE_FRAME = 81, // read u16
            TERMINATE = 82
        }
    }

}
