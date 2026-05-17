using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.AST
{
    public class ThreeAddressCode
    {
        public List<TacInstruction> Instructions { get; set; } = new List<TacInstruction>();

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Instructions);
        }
    }

    public class TacInstruction
    {
        public string Op { get; set; }  
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Result { get; set; }

        public TacInstruction(string op, string arg1 = "", string arg2 = "", string result = "")
        {
            Op = op;
            Arg1 = arg1;
            Arg2 = arg2;
            Result = result;
        }

        public override string ToString()
        {
            switch (Op)
            {
                case "=":
                    return $"{Result} = {Arg1}";
                case "+":
                case "-":
                case "*":
                case "/":
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "==":
                case "!=":
                    return $"{Result} = {Arg1} {Op} {Arg2}";
                case "jz":
                    return $"if {Arg1} == 0 goto {Result}";
                case "jmp":
                    return $"goto {Result}";
                case "label":
                    return $"{Result}:";
                default:
                    return $"{Op} {Result}";
            }
        }
    }
}
