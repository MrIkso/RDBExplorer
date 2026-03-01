using RDBExplorer.Utils;
using System.Text;

namespace RDBExplorer.Core.Formats.Bytecode
{
    public class ScriptParser
    {
        public ScriptFile Parse(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
            var script = new ScriptFile();

            byte versionLen = reader.ReadByte();
            script.Version = reader.ReadEncodedString(versionLen);
            if (!script.Version.Contains("2011-04-29T14:26:48+0900"))
            {
                throw new Exception("Wrong bytecode file");
            }
            ushort nameCount = reader.ReadUInt16();
            for (int i = 0; i < nameCount; i++)
            {
                ushort len = reader.ReadUInt16();
                uint hash = reader.ReadUInt32();
                string name = reader.ReadEncodedString(len + 1);
                script.AccessorNames.Add(new ScriptFile.AccessorNameEntry { Hash = hash, Name = name });
            }

            ushort symbolCount = reader.ReadUInt16();
            for (int i = 0; i < symbolCount; i++)
            {
                ushort id = reader.ReadUInt16();    // v30[0]
                ushort index = reader.ReadUInt16(); // v29
                ushort kind = reader.ReadUInt16();  // v28[0]

                script.Symbols.Add(new ScriptFile.SymbolEntry
                {
                    Id = id, // index AccessorNames
                    Index = index,
                    Kind = kind
                });
            }

            ushort extCount = reader.ReadUInt16();
            for (int i = 0; i < extCount; i++)
            {
                script.Externals.Add(new ScriptFile.ExternalEntry
                {
                    Kind = reader.ReadUInt16(),
                    Val1 = reader.ReadUInt16(),
                });
            }

            for (int t = 0; t < 3; t++)
            {
                ushort count = reader.ReadUInt16();
                ushort[] table = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    table[i] = reader.ReadUInt16();
                }
                script.IdTables.Add(table);
            }

            ushort builtInCount = reader.ReadUInt16();
            for (int i = 0; i < builtInCount; i++)
            {
                script.BuiltIns.Add(new ScriptFile.BuiltInEntry
                {
                    Id = reader.ReadUInt16(),
                    Index = reader.ReadUInt16()
                });
            }

            ushort literalCount = reader.ReadUInt16();
            for (int i = 0; i < literalCount; i++)
            {
                ushort len = reader.ReadUInt16();
                string lit = reader.ReadEncodedString(len + 1, Encoding.UTF8);
                script.Literals.Add(lit);
            }

            // bytecode
            uint bytecodeSize = reader.ReadUInt32();

            if (ms.Position % 2 != 0)
            {
                reader.ReadByte();
            }

            script.Bytecode = reader.ReadBytes((int)bytecodeSize);

            return script;
        }

        public void PrintInfo(ScriptFile script)
        {
            Console.WriteLine($"--- KTGL Script Info ---");
            Console.WriteLine($"Version: {script.Version}");
            Console.WriteLine($"Functions found: {script.AccessorNames.Count}");
            foreach (var name in script.AccessorNames)
            {
                Console.WriteLine($"  [0x{name.Hash:X8}] {name.Name}");
            }

            Console.WriteLine($"\nLiterals found: {script.Literals.Count}");
            for (int i = 0; i < script.Literals.Count; i++)
            {
                Console.WriteLine($"  [{i}] {script.Literals[i]}");
            }

            Console.WriteLine($"\nBytecode Size: {script.Bytecode.Length} bytes");

            Disassemble(script);
            List<Statement> statements = BuildAst(script);
            string code = GenerateCode(statements);
            Console.WriteLine(code);
        }

        public string Disassemble(ScriptFile script)
        {
            StringBuilder sb = new StringBuilder();
            if (script.Bytecode == null || script.Bytecode.Length == 0)
                return string.Empty;

            var resolver = new SctriptNameResolver(script);
            using var ms = new MemoryStream(script.Bytecode);
            using var reader = new BinaryReader(ms);

            sb.AppendLine("Script Disassembler");
            sb.AppendLine($"Script Version :{script.Version}");
            
            sb.AppendLine($"{"Addr",-7} | {"Op",-4} | {"Mnemonic",-20} | {"Arguments & Details",-30} | {"Raw Bytes"} ");
            sb.AppendLine(new string('-', 95));

            while (ms.Position < ms.Length)
            {
                long addr = ms.Position;
                ushort opRaw = reader.ReadUInt16();

                if (opRaw == 0x0000)
                {
                    sb.AppendLine($"{addr:X4}    | 0000 | {"NOP",-20} | {"-",-30} | 00 00");
                    continue;
                }
                if (opRaw == 0xFFFF)
                {
                    sb.AppendLine($"{addr:X4}    | FFFF | {"SECTION_BOUNDARY",-20} | {"-",-30} | FF FF");
                    continue;
                }

                ScriptFile.OpCode op = (ScriptFile.OpCode)opRaw;
                string opName = op.ToString();
                string details = "";
                string rawHex = "";

                switch (op)
                {
                    case ScriptFile.OpCode.ASSIGN_VAR:
                    case ScriptFile.OpCode.CALL_DEREF_FUNC:
                    case ScriptFile.OpCode.CALL_HOST_VAR:
                    case ScriptFile.OpCode.CALL_HOST_REG_READ:
                    case ScriptFile.OpCode.CALL_INTERNAL:
                    case ScriptFile.OpCode.PUSH_STR_LIT:
                    case ScriptFile.OpCode.GET_VAR_GLOBAL:
                    case ScriptFile.OpCode.GET_VAR_REFERRED:
                    case ScriptFile.OpCode.DEREF_VAR:
                    case ScriptFile.OpCode.GET_PROP_OBJECT:
                    case ScriptFile.OpCode.GET_PROP_VALUE:
                    case ScriptFile.OpCode.LOAD_FRAME:
                    case ScriptFile.OpCode.STORE_FRAME:
                        ushort u16Arg = reader.ReadUInt16();
                        string resolved = (op == ScriptFile.OpCode.PUSH_STR_LIT)
                            ? (u16Arg < script.Literals.Count ? $"\"{script.Literals[u16Arg]}\"" : "ERR_STR")
                            : resolver.GetNameFromSymbol(u16Arg);
                        details = $"idx: {u16Arg} ({resolved})";
                        rawHex = BitConverter.ToString(new byte[] { (byte)(opRaw & 0xFF), (byte)(opRaw >> 8), (byte)(u16Arg & 0xFF), (byte)(u16Arg >> 8) });
                        break;

                    case ScriptFile.OpCode.GOTO_FORWARD:
                    case ScriptFile.OpCode.GOTO_BACKWARD:
                    case ScriptFile.OpCode.JUMP_IF_TRUE:
                    case ScriptFile.OpCode.JUMP_IF_FALSE:
                    case ScriptFile.OpCode.JUMP_IF_NOT:
                    case ScriptFile.OpCode.LOGIC_AND_STRIP:
                        int offset = reader.ReadInt32();
                        long target = (op == ScriptFile.OpCode.GOTO_BACKWARD) ? (addr - offset + 6) : (addr + offset + 6);
                        details = $"offset: {offset} -> TARGET: {target:X4}";
                        break;

                    case ScriptFile.OpCode.PUSH_INT:
                        int iVal = reader.ReadInt32();
                        details = $"int: {iVal}";
                        break;
                    case ScriptFile.OpCode.PUSH_FLOAT:
                        float fVal = reader.ReadSingle();
                        details = $"float: {fVal}f";
                        break;
                    case ScriptFile.OpCode.PUSH_COMPLEX:
                        uint cVal = reader.ReadUInt32();
                        details = $"complex: 0x{cVal:X8}";
                        break;

                    case ScriptFile.OpCode.GET_VAR_LOCAL_60:
                    case ScriptFile.OpCode.GET_VAR_LOCAL_61:
                    case ScriptFile.OpCode.GET_VAR_LOCAL_62:
                    case ScriptFile.OpCode.CALL_HOST_REG_LOAD:
                        ushort arg1 = reader.ReadUInt16();
                        ushort arg2 = reader.ReadUInt16();
                        details = (op == ScriptFile.OpCode.CALL_HOST_REG_LOAD)
                            ? $"func: {resolver.GetNameFromSymbol(arg1)}, reg: {arg2}"
                            : $"space: {arg1}, index: {arg2}";
                        break;

                    case ScriptFile.OpCode.WIND_FRAME:
                        ushort wCount = reader.ReadUInt16();
                        details = $"frame_count: {wCount}";
                        sb.AppendLine($"{addr:X4}    | {opRaw:X4} | {opName,-20} | {details}");
                        for (int i = 0; i < wCount; i++)
                        {
                            uint v = reader.ReadUInt32(); ushort k = reader.ReadUInt16(); ushort ex = reader.ReadUInt16();
                            sb.AppendLine($"         |      | {"[LocalData]",-20} | val: {v}, kind: {k}, extra: {ex}");
                        }
                        continue; // Skip the main print since we did it here

                    case ScriptFile.OpCode.LITERAL_INTS:
                        ushort liCount = reader.ReadUInt16();
                        sb.AppendLine($"{addr:X4}    | {opRaw:X4} | {opName,-20} | count: {liCount}");
                        for (int i = 0; i < liCount; i++)
                        {
                            sb.AppendLine($"         |      | {"[IntLiteral]",-20} | value: {reader.ReadInt32()}");
                        }
                        continue;

                    case ScriptFile.OpCode.LITERAL_FLOATS:
                        ushort lfCount = reader.ReadUInt16();
                        sb.AppendLine($"{addr:X4}    | {opRaw:X4} | {opName,-20} | count: {lfCount}");
                        for (int i = 0; i < lfCount; i++)
                        {
                            sb.AppendLine($"         |      | {"[FloatLiteral]",-20} | value: {reader.ReadSingle()}f");
                        }
                        continue;

                    default:
                        details = "(no args)";
                        break;
                }

                if (!string.IsNullOrEmpty(opName))
                {
                    if (rawHex == string.Empty)
                    {
                        long endPos = ms.Position;
                        ms.Position = addr;
                        byte[] bytes = reader.ReadBytes((int)(endPos - addr));
                        rawHex = BitConverter.ToString(bytes).Replace("-", " ");
                    }
                    sb.AppendLine($"{addr:X4}    | {opRaw:X2} | {opName,-20} | {details,-30} | {rawHex}");
                }
            }
            return sb.ToString();
        }

        public List<Statement> BuildAst(ScriptFile script)
        {
            var resolver = new SctriptNameResolver(script);
            var jumpTargets = PreScanJumpTargets(script.Bytecode);

            using var ms = new MemoryStream(script.Bytecode);
            using var reader = new BinaryReader(ms);

            var statements = new List<Statement>();
            var evalStack = new Stack<CEvalProxy>();
            var rhsMarkers = new Stack<int>();

            var frameStack = new Stack<Dictionary<int, (string name, KtglType type)>>();

            Dictionary<int, (string name, KtglType type)> CurrentFrame()
                => frameStack.Count > 0 ? frameStack.Peek() : new Dictionary<int, (string, KtglType)>();

            string ResolveLocalBySpace(ushort space, ushort idx)
            {
                var frames = frameStack.ToArray(); // [0]=top (current)
                int frameIdx = space - 1;
                if (frameIdx >= 0 && frameIdx < frames.Length)
                {
                    if (frames[frameIdx].TryGetValue(idx, out var info))
                        return info.name;
                }
                return $"local_{idx}";
            }

            KtglType ResolveTypeBySpace(ushort space, ushort idx)
            {
                var frames = frameStack.ToArray();
                int frameIdx = space - 1;
                if (frameIdx >= 0 && frameIdx < frames.Length)
                {
                    if (frames[frameIdx].TryGetValue(idx, out var info))
                        return info.type;
                }
                return KtglType.Unknown;
            }

            while (ms.Position < ms.Length)
            {
                long addr = ms.Position;

                if (jumpTargets.Contains(addr))
                    statements.Add(new LabelStmt { Address = addr });

                ushort opRaw = reader.ReadUInt16();
                if (opRaw == 0 || opRaw == 0xFFFF)
                    continue;

                var op = (ScriptFile.OpCode)opRaw;

                switch (op)
                {
                    case ScriptFile.OpCode.ASSIGN_VAR:
                        {
                            ushort N = reader.ReadUInt16();
                            if (N == 0)
                                break;

                            var refNames = new string[N];
                            var refKinds = new KtglType[N];
                            for (int i = N - 1; i >= 0; i--)
                            {
                                if (evalStack.Count > 0)
                                {
                                    var top = evalStack.Pop();
                                    refNames[i] = top.Expr.ToString();
                                    refKinds[i] = top.Kind;
                                }
                                else
                                {
                                    Console.Error.WriteLine($"[WARN] ASSIGN_VAR: ref underflow at 0x{addr:X4}");
                                    refNames[i] = "<?>";
                                    refKinds[i] = KtglType.Unknown;
                                }
                            }

                            var valExprs = new Expression[N];
                            var valKinds = new KtglType[N];
                            for (int i = N - 1; i >= 0; i--)
                            {
                                if (evalStack.Count > 0)
                                {
                                    var v = evalStack.Pop();
                                    valExprs[i] = v.Expr;
                                    valKinds[i] = v.Kind;
                                }
                                else
                                {
                                    valExprs[i] = new VariableExpr { Name = $"param_{i + 1}" };
                                    valKinds[i] = KtglType.Unknown;
                                }
                            }

                            for (int i = 0; i < N; i++)
                                statements.Add(new AssignmentStmt
                                {
                                    VarName = refNames[i],
                                    Value = valExprs[i],
                                    Type = refKinds[i] != KtglType.Unknown ? refKinds[i] : valKinds[i]
                                });
                            break;
                        }

                    case ScriptFile.OpCode.ASSIGN_STACK:
                        {
                            string targetName;
                            if (evalStack.Count > 0)
                                targetName = evalStack.Pop().Expr.ToString();
                            else
                            {
                                Console.Error.WriteLine($"[WARN] ASSIGN_STACK: ref underflow at 0x{addr:X4}");
                                targetName = "<?>";
                            }

                            Expression val;
                            if (evalStack.Count > 0)
                                val = evalStack.Pop().Expr;
                            else
                            {
                                Console.Error.WriteLine($"[WARN] ASSIGN_STACK: val underflow at 0x{addr:X4}");
                                val = new VariableExpr { Name = "<?>" };
                            }

                            statements.Add(new AssignmentStmt { VarName = targetName, Value = val });
                            break;
                        }

                    case ScriptFile.OpCode.PUSH_ENV_REG:
                        if (evalStack.Count > 0 && evalStack.Peek().Expr is CallExpr unusedCall)
                        {
                            evalStack.Pop();
                            statements.Add(new CallStmtNode { Call = unusedCall });
                        }
                        rhsMarkers.Push(evalStack.Count);
                        break;

                    //Arithmetic
                    case ScriptFile.OpCode.ADD:
                        HandleBinary(evalStack, "+", addr: addr);
                        break;
                    case ScriptFile.OpCode.SUB:
                        HandleBinary(evalStack, "-", addr: addr);
                        break;
                    case ScriptFile.OpCode.MUL:
                        HandleBinary(evalStack, "*", addr: addr);
                        break;
                    case ScriptFile.OpCode.DIV:
                        HandleBinary(evalStack, "/", addr: addr);
                        break;
                    case ScriptFile.OpCode.MOD:
                        HandleBinary(evalStack, "%", KtglType.Integer, addr);
                        break;

                    case ScriptFile.OpCode.BIT_AND:
                        HandleBinary(evalStack, "&", KtglType.Integer, addr);
                        break;
                    case ScriptFile.OpCode.BIT_OR:
                        HandleBinary(evalStack, "|", KtglType.Integer, addr);
                        break;
                    case ScriptFile.OpCode.BIT_XOR:
                        HandleBinary(evalStack, "^", KtglType.Integer, addr);
                        break;

                    case ScriptFile.OpCode.CMP_EQ:
                        HandleBinary(evalStack, "==", KtglType.Boolean, addr);
                        break;
                    case ScriptFile.OpCode.CMP_NE:
                        HandleBinary(evalStack, "!=", KtglType.Boolean, addr);
                        break;
                    case ScriptFile.OpCode.CMP_GT:
                        HandleBinary(evalStack, ">", KtglType.Boolean, addr);
                        break;
                    case ScriptFile.OpCode.CMP_GE:
                        HandleBinary(evalStack, ">=", KtglType.Boolean, addr);
                        break;
                    case ScriptFile.OpCode.CMP_LT:
                        HandleBinary(evalStack, "<", KtglType.Boolean, addr);
                        break;
                    case ScriptFile.OpCode.CMP_LE:
                        HandleBinary(evalStack, "<=", KtglType.Boolean, addr);
                        break;

                    case ScriptFile.OpCode.LOGIC_AND_STRIP:
                        {
                            int offset = reader.ReadInt32();
                            long target = addr + offset + 6;
                            var cond = SafePop(evalStack, addr);
                            statements.Add(new JumpStmt
                            {
                                Target = target,
                                Condition = new UnaryOpExpr { Op = "!", Operand = cond.Expr },
                                IsShortCircuitAnd = true
                            });
                            break;
                        }

                    // Jumps 
                    case ScriptFile.OpCode.JUMP_IF_TRUE:
                        {
                            int offset = reader.ReadInt32();
                            var cond = SafePop(evalStack, addr);
                            statements.Add(new JumpStmt
                            {
                                Target = addr + offset + 6,
                                Condition = cond.Expr,
                                JumpIfTrue = true
                            });
                            break;
                        }
                    case ScriptFile.OpCode.JUMP_IF_FALSE:
                    case ScriptFile.OpCode.JUMP_IF_NOT:
                        {
                            int offset = reader.ReadInt32();
                            var cond = SafePop(evalStack, addr);
                            statements.Add(new JumpStmt
                            {
                                Target = addr + offset + 6,
                                Condition = cond.Expr
                            });
                            break;
                        }
                    case ScriptFile.OpCode.GOTO_FORWARD:
                        FlushUnused(evalStack, statements);
                        statements.Add(new JumpStmt { Target = addr + reader.ReadInt32() + 6 });
                        break;
                    case ScriptFile.OpCode.GOTO_BACKWARD:
                        FlushUnused(evalStack, statements);
                        statements.Add(new JumpStmt { Target = addr + reader.ReadInt32() + 6 });
                        break;

                    // PUSH_FALSE / PUSH_TRUE
                    case ScriptFile.OpCode.PUSH_FALSE:
                        evalStack.Push(new CEvalProxy(
                            new LiteralExpr { Value = false, DataType = KtglType.Boolean }, KtglType.Boolean));
                        break;
                    case ScriptFile.OpCode.PUSH_TRUE:
                        evalStack.Push(new CEvalProxy(
                            new LiteralExpr { Value = true, DataType = KtglType.Boolean }, KtglType.Boolean));
                        break;

                    case ScriptFile.OpCode.CALL_DEREF_FUNC:
                        {
                            ushort varRef = reader.ReadUInt16();
                            string varName = resolver.GetNameFromSymbol(varRef);
                            int marker = rhsMarkers.Count > 0 ? rhsMarkers.Pop() : 0;
                            var args = PopArgs(evalStack, marker, addr);
                            evalStack.Push(new CEvalProxy(
                                new CallExpr { Name = $"(*{varName})", Args = args },
                                KtglType.Unknown));
                            break;
                        }
                    case ScriptFile.OpCode.CALL_HOST_VAR:
                        {
                            ushort symIdx = reader.ReadUInt16();
                            string name = resolver.GetNameFromSymbol(symIdx);
                            int marker = rhsMarkers.Count > 0 ? rhsMarkers.Pop() : 0;
                            var args = PopArgs(evalStack, marker, addr);
                            var callExpr = new CallExpr { Name = name, Args = args };
                            evalStack.Push(new CEvalProxy(callExpr, KtglType.Unknown));
                            break;
                        }
                    case ScriptFile.OpCode.CALL_HOST_REG_LOAD:
                        {
                            ushort symIdx = reader.ReadUInt16();
                            ushort regIdx = reader.ReadUInt16();
                            string name = resolver.GetNameFromSymbol(symIdx);
                            int marker = rhsMarkers.Count > 0 ? rhsMarkers.Pop() : 0;
                            var args = PopArgs(evalStack, marker, addr);
                            statements.Add(new AssignmentStmt
                            {
                                VarName = $"reg_{regIdx}",
                                Value = new CallExpr { Name = name, Args = args }
                            });
                            break;
                        }
                    case ScriptFile.OpCode.CALL_HOST_REG_READ:
                        {
                            ushort symIdx = reader.ReadUInt16();
                            evalStack.Push(new CEvalProxy(
                                new CallExpr { Name = resolver.GetNameFromSymbol(symIdx), Args = new List<Expression>() },
                                KtglType.Unknown));
                            break;
                        }

                    case ScriptFile.OpCode.CALL_INTERNAL:
                        {
                            ushort varRef = reader.ReadUInt16();
                            string funcName = resolver.GetNameFromSymbol(varRef);
                            int marker = rhsMarkers.Count > 0 ? rhsMarkers.Pop() : 0;
                            var args = PopArgs(evalStack, marker, addr);
                            evalStack.Push(new CEvalProxy(
                                new CallExpr { Name = funcName, Args = args },
                                KtglType.Unknown));
                            break;
                        }

                    case ScriptFile.OpCode.POP_ENV:
                        statements.Add(new CommentStmt { Text = "// [pop_env]" });
                        break;
                    case ScriptFile.OpCode.POP_ENV_STACK:
                        statements.Add(new CommentStmt { Text = "// [pop_env_stack]" });
                        break;

                    case ScriptFile.OpCode.RETURN_INTERNAL:
                        {
                            FlushUnused(evalStack, statements);
                            bool hasVal = evalStack.Count > 0
                                && !(evalStack.Peek().Expr is LiteralExpr le && le.Value is bool);
                            if (hasVal) { var rv = evalStack.Pop().Expr; statements.Add(new ReturnStmt { HasValue = true, ReturnValue = rv }); }
                            else statements.Add(new ReturnStmt { HasValue = false });
                            break;
                        }
                    case ScriptFile.OpCode.RETURN_UNWIND:
                        {
                            FlushUnused(evalStack, statements);
                            bool hasVal = evalStack.Count > 0
                                && !(evalStack.Peek().Expr is LiteralExpr le && le.Value is bool);
                            if (hasVal) { var rv = evalStack.Pop().Expr; statements.Add(new ReturnStmt { HasValue = true, IsUnwind = true, ReturnValue = rv }); }
                            else statements.Add(new ReturnStmt { HasValue = false, IsUnwind = true });
                            break;
                        }

                    // Integer literals
                    case ScriptFile.OpCode.PUSH_INT:
                        evalStack.Push(new CEvalProxy(
                            new LiteralExpr { Value = reader.ReadInt32(), DataType = KtglType.Integer },
                            KtglType.Integer));
                        break;
                    case ScriptFile.OpCode.PUSH_INT_0:
                        evalStack.Push(new CEvalProxy(
                            new LiteralExpr { Value = 0, DataType = KtglType.Integer }, KtglType.Integer));
                        break;
                    case ScriptFile.OpCode.PUSH_INT_1:
                        evalStack.Push(new CEvalProxy(
                            new LiteralExpr { Value = 1, DataType = KtglType.Integer }, KtglType.Integer));
                        break;
                    case ScriptFile.OpCode.LITERAL_INTS:
                        {
                            ushort count = reader.ReadUInt16();
                            for (int i = 0; i < count; i++)
                                evalStack.Push(new CEvalProxy(
                                    new LiteralExpr { Value = reader.ReadInt32(), DataType = KtglType.Integer },
                                    KtglType.Integer));
                            break;
                        }

                    // Float literals
                    case ScriptFile.OpCode.PUSH_FLOAT:
                        evalStack.Push(new CEvalProxy(
                            new LiteralExpr { Value = reader.ReadSingle(), DataType = KtglType.Decimal },
                            KtglType.Decimal));
                        break;
                    case ScriptFile.OpCode.LITERAL_FLOATS:
                        {
                            ushort count = reader.ReadUInt16();
                            for (int i = 0; i < count; i++)
                                evalStack.Push(new CEvalProxy(
                                    new LiteralExpr { Value = reader.ReadSingle(), DataType = KtglType.Decimal },
                                    KtglType.Decimal));
                            break;
                        }
                    case ScriptFile.OpCode.PUSH_STR_LIT:
                        {
                            ushort litIdx = reader.ReadUInt16();
                            string lit = litIdx < script.Literals.Count ? script.Literals[litIdx] : $"?str_{litIdx}";
                            evalStack.Push(new CEvalProxy(
                                new LiteralExpr { Value = lit, DataType = KtglType.String },
                                KtglType.String));
                            break;
                        }
                    case ScriptFile.OpCode.PUSH_COMPLEX:
                        {
                            uint frameSize = reader.ReadUInt32();
                            long targetOffset = addr + frameSize + 6;
                            statements.Add(new CommentStmt
                            {
                                Text = $"// [push_complex: target_offset=0x{targetOffset:X4}, frame_size={frameSize}]"
                            });
                            break;
                        }

                    // Unary
                    case ScriptFile.OpCode.UNARY_MINUS:
                        {
                            var operand = SafePop(evalStack, addr);
                            evalStack.Push(new CEvalProxy(
                                new UnaryOpExpr { Op = "-", Operand = operand.Expr },
                                operand.Kind));
                            break;
                        }
                    case ScriptFile.OpCode.LOGIC_NOT:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "!", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Boolean));
                        break;

                    // STACK_FRAME_POP
                    case ScriptFile.OpCode.STACK_FRAME_POP:
                        FlushUnused(evalStack, statements);
                        if (frameStack.Count > 0) frameStack.Pop();
                        statements.Add(new CommentStmt { Text = "// [stack_frame_pop]" });
                        break;

                    case ScriptFile.OpCode.WIND_FRAME:
                        {
                            FlushUnused(evalStack, statements);
                            ushort count = reader.ReadUInt16();
                            var frame = new Dictionary<int, (string name, KtglType type)>();
                            var locals = new List<(int val, KtglType kind, int extra)>(count);

                            for (int i = 0; i < count; i++)
                            {
                                int val = reader.ReadInt32();
                                ushort kind = reader.ReadUInt16();
                                ushort extra = reader.ReadUInt16();
                                locals.Add((val, KindToType(kind), extra));
                            }
                            for (int i = 0; i < locals.Count; i++)
                            {
                                var (val, kind, extra) = locals[i];
                                string name = $"local_{i}";
                                frame[i] = (name, kind);
                            }

                            frameStack.Push(frame);
                            statements.Add(new WindFrameStmt
                            {
                                LocalCount = count,
                                Locals = locals.Select((l, i) => new LocalVarDef
                                {
                                    Index = i,
                                    Name = $"local_{i}",
                                    Type = l.kind,
                                    InitVal = l.val,
                                    SymId = l.extra
                                }).ToList()
                            });
                            break;
                        }

                    //Type conversions
                    case ScriptFile.OpCode.TO_FLOAT:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "(float)", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Decimal));
                        break;
                    case ScriptFile.OpCode.TO_INT_CEIL:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "ceil", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Integer));
                        break;
                    case ScriptFile.OpCode.TO_INT_FLOOR:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "floor", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Integer));
                        break;
                    case ScriptFile.OpCode.TO_INT_ROUND:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "round", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Integer));
                        break;

                    // Variable access
                    case ScriptFile.OpCode.GET_VAR_GLOBAL:
                        {
                            ushort symIdx = reader.ReadUInt16();
                            evalStack.Push(new CEvalProxy(
                                new VariableExpr { Name = resolver.GetNameFromSymbol(symIdx) },
                                KtglType.GlobalVarRef));
                            break;
                        }
                    case ScriptFile.OpCode.GET_VAR_REFERRED:
                        {
                            ushort varRef = reader.ReadUInt16();
                            evalStack.Push(new CEvalProxy(
                                new VariableExpr { Name = resolver.GetNameFromSymbol(varRef) },
                                KtglType.Unknown));
                            break;
                        }
                    case ScriptFile.OpCode.GET_VAR_LOCAL_60:
                        {
                            ushort space = reader.ReadUInt16();
                            ushort idx = reader.ReadUInt16();
                            string vname = ResolveLocalBySpace(space, idx);
                            evalStack.Push(new CEvalProxy(
                                new VariableExpr { Name = vname },
                                ResolveTypeBySpace(space, idx)));
                            statements.Add(new CommentStmt { Text = $"// [frame_store space={space} {vname}]" });
                            break;
                        }
                    case ScriptFile.OpCode.GET_VAR_LOCAL_61:
                        {
                            ushort space = reader.ReadUInt16();
                            ushort idx = reader.ReadUInt16();
                            string vname = ResolveLocalBySpace(space, idx);
                            KtglType vtype = ResolveTypeBySpace(space, idx);
                            evalStack.Push(new CEvalProxy(new VariableExpr { Name = vname }, vtype)
                            {
                                IsRef = true,
                                RefIndex = idx
                            });
                            break;
                        }
                    case ScriptFile.OpCode.GET_VAR_LOCAL_62:
                        {
                            ushort space = reader.ReadUInt16();
                            ushort idx = reader.ReadUInt16();
                            string vname = ResolveLocalBySpace(space, idx);
                            evalStack.Push(new CEvalProxy(
                                new VariableExpr { Name = vname },
                                ResolveTypeBySpace(space, idx)));
                            break;
                        }

                    case ScriptFile.OpCode.GET_LOCAL_REG:
                        evalStack.Push(new CEvalProxy(
                            new VariableExpr { Name = "this_env" }, KtglType.Unknown));
                        break;

                    // Dereference
                    case ScriptFile.OpCode.DEREF_VAR:
                        {
                            ushort varRef = reader.ReadUInt16();
                            evalStack.Push(new CEvalProxy(
                                new UnaryOpExpr
                                {
                                    Op = "&",
                                    Operand = new VariableExpr { Name = resolver.GetNameFromSymbol(varRef) }
                                },
                                KtglType.LocalVarRef));
                            break;
                        }
                    case ScriptFile.OpCode.DEREF_GLOBAL_VAL:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "*", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Unknown));
                        break;
                    case ScriptFile.OpCode.DEREF_LOCAL:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "&local", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.LocalVarRef));
                        break;
                    case ScriptFile.OpCode.DEREF_LOCAL_VAL:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "*local", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Unknown));
                        break;

                    // Array
                    case ScriptFile.OpCode.ARRAY_REF_CHILD:
                        {
                            var index = SafePop(evalStack, addr).Expr;
                            var array = SafePop(evalStack, addr).Expr;
                            evalStack.Push(new CEvalProxy(
                                new ArrayAccessNode { Parent = array, Index = index },
                                KtglType.LocalVarRef));
                            break;
                        }
                    case ScriptFile.OpCode.ARRAY_GET_VAL:
                        {
                            var index = SafePop(evalStack, addr).Expr;
                            var array = SafePop(evalStack, addr).Expr;
                            evalStack.Push(new CEvalProxy(
                                new ArrayAccessNode { Parent = array, Index = index },
                                KtglType.Unknown));
                            break;
                        }
                    case ScriptFile.OpCode.EXPAND_ARRAY:
                        evalStack.Push(new CEvalProxy(
                            new UnaryOpExpr { Op = "...", Operand = SafePop(evalStack, addr).Expr },
                            KtglType.Unknown));
                        break;

                    // Properties
                    case ScriptFile.OpCode.GET_PROP_OBJECT:
                        {
                            ushort propId = reader.ReadUInt16();
                            var obj = SafePop(evalStack, addr);
                            evalStack.Push(new CEvalProxy(
                                new MemberAccessExpr { Parent = obj.Expr, Member = resolver.GetNameFromSymbol(propId) },
                                KtglType.LocalVarRef));
                            break;
                        }
                    case ScriptFile.OpCode.GET_PROP_VALUE:
                        {
                            ushort propId = reader.ReadUInt16();
                            var obj = SafePop(evalStack, addr);
                            evalStack.Push(new CEvalProxy(
                                new MemberAccessExpr { Parent = obj.Expr, Member = resolver.GetNameFromSymbol(propId) },
                                KtglType.Unknown));
                            break;
                        }

                    // Frame registers
                    case ScriptFile.OpCode.LOAD_FRAME:
                        evalStack.Push(new CEvalProxy(
                            new VariableExpr { Name = $"reg_{reader.ReadUInt16()}" },
                            KtglType.Unknown));
                        break;
                    case ScriptFile.OpCode.STORE_FRAME:
                        statements.Add(new AssignmentStmt
                        {
                            VarName = $"reg_{reader.ReadUInt16()}",
                            Value = SafePop(evalStack, addr).Expr
                        });
                        break;

                    // TERMINATE
                    case ScriptFile.OpCode.TERMINATE:
                        statements.Add(new TerminateStmt());
                        break;

                    default:
                        statements.Add(new CommentStmt
                        {
                            Text = $"/* UNHANDLED Op 0x{opRaw:X4} at 0x{addr:X4} */"
                        });
                        break;
                }
            }

            return statements;
        }

        private static CEvalProxy SafePop(Stack<CEvalProxy> stack, long addr = 0)
        {
            if (stack.Count > 0)
                return stack.Pop();

            Console.Error.WriteLine($"[WARN] evalStack underflow at 0x{addr:X4}");
            return new CEvalProxy(new VariableExpr { Name = "<?>" }, KtglType.Unknown);
        }

        private static List<Expression> PopArgs(Stack<CEvalProxy> stack, int marker, long addr = 0)
        {
            var args = new List<Expression>();
            while (stack.Count > marker)
                args.Add(SafePop(stack, addr).Expr);
            args.Reverse();
            return args;
        }

        private void HandleBinary(Stack<CEvalProxy> stack, string op,
                                  KtglType? forcedType = null, long addr = 0)
        {
            if (stack.Count < 2)
            {
                Console.Error.WriteLine($"[WARN] HandleBinary({op}): only {stack.Count} items at 0x{addr:X4}");
                if (stack.Count == 1)
                {
                    var single = SafePop(stack, addr);
                    stack.Push(new CEvalProxy(
                        new UnaryOpExpr { Op = op, Operand = single.Expr },
                        forcedType ?? single.Kind));
                }
                return;
            }

            var right = SafePop(stack, addr);
            var left = SafePop(stack, addr);

            KtglType resultType = forcedType ?? (left.Kind == KtglType.Decimal || right.Kind == KtglType.Decimal
                    ? KtglType.Decimal
                    : KtglType.Integer);

            stack.Push(new CEvalProxy(
                new BinaryOpExpr { Left = left.Expr, Right = right.Expr, Op = op },
                resultType));
        }

        private struct CEvalProxy
        {
            public Expression Expr;
            public KtglType Kind;
            public bool IsRef;
            public int RefIndex;

            public CEvalProxy(Expression e, KtglType k)
            {
                Expr = e;
                Kind = k;
                IsRef = false;
                RefIndex = -1;
            }
        }

        private HashSet<long> PreScanJumpTargets(byte[] bytecode)
        {
            var targets = new HashSet<long>();
            using var ms = new MemoryStream(bytecode);
            using var reader = new BinaryReader(ms);
            while (ms.Position < ms.Length)
            {
                long addr = ms.Position;
                ushort op = reader.ReadUInt16();

                switch ((ScriptFile.OpCode)op)
                {
                    case ScriptFile.OpCode.JUMP_IF_TRUE:
                    case ScriptFile.OpCode.JUMP_IF_FALSE:
                    case ScriptFile.OpCode.JUMP_IF_NOT:
                    case ScriptFile.OpCode.GOTO_FORWARD:
                    case ScriptFile.OpCode.GOTO_BACKWARD:
                    case ScriptFile.OpCode.LOGIC_AND_STRIP:
                    case ScriptFile.OpCode.PUSH_INT:
                    case ScriptFile.OpCode.PUSH_FLOAT:
                    case ScriptFile.OpCode.PUSH_COMPLEX:
                        int val = reader.ReadInt32();
                        if (op <= (ushort)ScriptFile.OpCode.JUMP_IF_NOT || op == 34 || op == 35)
                        {
                            long target = (op == (ushort)ScriptFile.OpCode.GOTO_BACKWARD) ? (addr - val + 6) : (addr + val + 6);
                            targets.Add(target);
                        }
                        break;

                    case ScriptFile.OpCode.GET_VAR_LOCAL_60:
                    case ScriptFile.OpCode.GET_VAR_LOCAL_61:
                    case ScriptFile.OpCode.GET_VAR_LOCAL_62:
                    case ScriptFile.OpCode.CALL_HOST_REG_LOAD:
                        ms.Position += 4;
                        break;

                    case ScriptFile.OpCode.ASSIGN_VAR:
                    case ScriptFile.OpCode.CALL_DEREF_FUNC:
                    case ScriptFile.OpCode.CALL_HOST_VAR:
                    case ScriptFile.OpCode.CALL_HOST_REG_READ:
                    case ScriptFile.OpCode.CALL_INTERNAL:
                    case ScriptFile.OpCode.PUSH_STR_LIT:
                    case ScriptFile.OpCode.GET_VAR_GLOBAL:
                    case ScriptFile.OpCode.GET_VAR_REFERRED:
                    case ScriptFile.OpCode.DEREF_VAR:
                    case ScriptFile.OpCode.GET_PROP_OBJECT:
                    case ScriptFile.OpCode.GET_PROP_VALUE:
                    case ScriptFile.OpCode.LOAD_FRAME:
                    case ScriptFile.OpCode.STORE_FRAME:
                        ms.Position += 2;
                        break;
                    case ScriptFile.OpCode.WIND_FRAME:
                        ushort wCount = reader.ReadUInt16(); ms.Position += (wCount * 8); break;
                    case ScriptFile.OpCode.LITERAL_INTS:
                    case ScriptFile.OpCode.LITERAL_FLOATS:
                        ushort lCount = reader.ReadUInt16(); ms.Position += (lCount * 4); break;
                }
            }
            return targets;
        }

        private static void FlushUnused(Stack<CEvalProxy> stack, List<Statement> stmts)
        {
            while (stack.Count > 0 && stack.Peek().Expr is CallExpr call)
            {
                stack.Pop();
                stmts.Add(new CallStmtNode { Call = call });
            }
        }

        private static KtglType KindToType(ushort kind) => kind switch
        {
            0 => KtglType.Unknown,       // sentinel
            1 => KtglType.Integer,
            2 => KtglType.Boolean,
            3 => KtglType.Decimal,
            4 => KtglType.String,
            5 => KtglType.CodeOffset,
            6 => KtglType.BinderPosition,
            7 => KtglType.Decimal,       // vec3 component (float)
            _ => KtglType.Unknown
        };

        public string GenerateCode(List<Statement> ast)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var stmt in ast)
            {
                if (stmt is LabelStmt) sb.AppendLine(stmt.ToString());
                else sb.AppendLine("    " + stmt.ToString());
            }
            return sb.ToString();
        }
    }
}
