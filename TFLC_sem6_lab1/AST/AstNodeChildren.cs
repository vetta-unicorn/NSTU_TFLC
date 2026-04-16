using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.AST
{
    public class DoWhileNode : AstNode
    {
        public AstNode Body { get; set; }
        public AstNode Condition { get; set; }
    }

    public class BlockNode : AstNode
    {
        public List<AstNode> Statements { get; set; } = new();
    }

    public class UnaryOpNode : AstNode
    {
        public string Operator { get; set; } // ++ или --
        public VariableNode Variable { get; set; }
    }

    public class BinaryOpNode : AstNode
    {
        public string Operator { get; set; } // <, >, == и т.д.
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }
    }

    public class VariableNode : AstNode
    {
        public string Name { get; set; }
    }

    public class LiteralNode : AstNode
    {
        public int Value { get; set; }
    }

}
