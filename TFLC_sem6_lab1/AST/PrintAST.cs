using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.AST
{
    public class PrintAST
    {
        public string GetNodeLabel(AstNode node)
        {
            switch (node)
            {
                case VariableNode v:
                    return $"VariableNode (name: \"{v.Name}\")";

                case LiteralNode l:
                    return $"LiteralNode (value: {l.Value})";

                case UnaryOpNode u:
                    return $"UnaryOpNode (op: {u.Operator})";

                case BinaryOpNode b:
                    return $"BinaryOpNode (op: {b.Operator})";

                default:
                    return node.GetType().Name;
            }
        }

        public List<AstNode> GetChildren(AstNode node)
        {
            var children = new List<AstNode>();

            switch (node)
            {
                case DoWhileNode d:
                    children.Add(d.Body);
                    children.Add(d.Condition);
                    break;

                case BlockNode b:
                    children.AddRange(b.Statements);
                    break;

                case UnaryOpNode u:
                    children.Add(u.Variable);
                    break;

                case BinaryOpNode b:
                    children.Add(b.Left);
                    children.Add(b.Right);
                    break;
            }

            return children;
        }
    }
}
