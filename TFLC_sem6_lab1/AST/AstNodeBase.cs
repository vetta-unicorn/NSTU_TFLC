using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.AST
{
    public abstract class AstNode
    {
        public List<AstNode> Children { get; set; } = new();
    }
}
