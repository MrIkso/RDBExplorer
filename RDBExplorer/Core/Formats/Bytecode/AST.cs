using System.Text;

namespace RDBExplorer.Core.Formats.Bytecode
{
    public abstract class AstNode { }

    public abstract class Expression : AstNode
    {
        public KtglType Type = KtglType.Unknown;
    }

    public abstract class Statement : AstNode
    {
        public long Address;
    }

    public class LiteralExpr : Expression
    {
        public object Value;
        public KtglType DataType;

        public override string ToString() => DataType switch
        {
            KtglType.String => $"\"{Value}\"",
            KtglType.Decimal => $"{Value}f",
            KtglType.Boolean => Value?.ToString()?.ToLower() ?? "false",
            _ => Value?.ToString() ?? "null"
        };
    }

    public class VariableExpr : Expression
    {
        public string Name;
        public override string ToString() => Name;
    }

    public class BinaryOpExpr : Expression
    {
        public Expression Left, Right;
        public string Op;
        public override string ToString() => $"({Left} {Op} {Right})";
    }

    public class UnaryOpExpr : Expression
    {
        public Expression Operand;
        public string Op;

        public override string ToString()
        {
            if (Operand == null) return $"{Op}???";
            return Op switch
            {
                "(float)" or "ceil" or "floor" or "round" => $"{Op}({Operand})",
                "..." => $"...{Operand}",
                "&" => $"&{Operand}",
                "*" => $"*{Operand}",
                "&local" => $"&local({Operand})",
                "*local" => $"*local({Operand})",
                _ => $"{Op}{Operand}"
            };
        }
    }

    public class CallExpr : Expression
    {
        public string Name;
        public List<Expression> Args = new();
        public override string ToString() => $"{Name}({string.Join(", ", Args)})";
    }

    public class ArrayAccessNode : Expression
    {
        public Expression Parent;
        public Expression Index;
        public override string ToString() => $"{Parent}[{Index}]";
    }

    public class MemberAccessExpr : Expression
    {
        public Expression Parent;
        public string Member;
        public override string ToString() => $"{Parent}.{Member}";
    }

    public class AssignmentStmt : Statement
    {
        public string VarName;
        public Expression Value;
        public KtglType Type;
        public bool IsDeclaration;

        public override string ToString() =>
            $"{(IsDeclaration ? "var " : "")}{VarName} = {Value};";
    }

    public class CallStmtNode : Statement
    {
        public CallExpr Call;
        public override string ToString() => $"{Call};";
    }

    public class JumpStmt : Statement
    {
        public long Target;
        public Expression Condition;  
        public bool JumpIfTrue;
        public bool IsShortCircuitAnd;

        public override string ToString()
        {
            string label = $"label_{Target:X4}";
            if (Condition == null)
                return $"goto {label};";

            string scTag = IsShortCircuitAnd ? " /* && short-circuit */" : "";
            if (JumpIfTrue)
                return $"if ({Condition}) goto {label};{scTag}";
            else
                return $"if (!({Condition})) goto {label};{scTag}";
        }
    }

    public class ReturnStmt : Statement
    {
        public bool HasValue;
        public bool IsUnwind;
        public Expression ReturnValue;

        public override string ToString()
        {
            string retPart = HasValue && ReturnValue != null
                ? $"return {ReturnValue};"
                : "return;";
            return IsUnwind ? $"{retPart} // unwind" : retPart;
        }
    }

    public class LabelStmt : Statement
    {
        public override string ToString() => $"\nlabel_{Address:X4}:";
    }

    public class TerminateStmt : Statement
    {
        public override string ToString() => "terminate();";
    }

    public class CommentStmt : Statement
    {
        public string Text;
        public override string ToString() => Text;
    }

    public class WindFrameStmt : Statement
    {
        public int LocalCount;
        public List<LocalVarDef> Locals = new();

        public override string ToString()
        {
            if (Locals.Count == 0)
                return $"// --- Initialize Stack Frame ({LocalCount} variables) ---";

            var sb = new StringBuilder();
            sb.AppendLine($"// --- Initialize Stack Frame ({LocalCount} variables) ---");
            foreach (var loc in Locals)
            {
                string typeName = loc.Type switch
                {
                    KtglType.Unknown => "unknown",
                    KtglType.Integer => "integer",
                    KtglType.Boolean => "boolean",
                    KtglType.Decimal => "decimal",
                    KtglType.String => "string",
                    KtglType.CodeOffset => "codeoffset",
                    KtglType.BinderPosition => "binderposition",
                    _ => loc.Type.ToString().ToLower()
                };

                if (loc.Type == KtglType.Decimal && loc.InitVal == 3)
                    typeName = "vec3";

                string initStr = loc.Type == KtglType.CodeOffset
                    ? loc.InitVal.ToString()
                    : "0";

                if (loc.SymId == 65535)
                    sb.AppendLine($"    {typeName} {loc.Name} = {initStr}; // sentinel");
                else
                    sb.AppendLine($"    {typeName} {loc.Name} = {initStr}; // ID: {loc.SymId}");
            }
            return sb.ToString().TrimEnd();
        }
    }

    public class LocalVarDef
    {
        public int Index;
        public string Name;
        public KtglType Type;
        public int InitVal;
        public int SymId;
    }
}