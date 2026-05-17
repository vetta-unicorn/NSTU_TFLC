using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.AST
{
    public class TACOptimizer
    {
        public int ConstantFoldingCount { get; private set; }
        public int CopyPropagationCount { get; private set; }

        public List<TacInstruction> AfterConstantFolding { get; private set; }
        public List<TacInstruction> AfterCopyPropagation { get; private set; }

        public TACOptimizer()
        {
            Reset();
        }

        public void Reset()
        {
            ConstantFoldingCount = 0;
            CopyPropagationCount = 0;
            AfterConstantFolding = null;
            AfterCopyPropagation = null;
        }

        public List<TacInstruction> Optimize(List<TacInstruction> instructions)
        {
            Reset();
            var optimized = new List<TacInstruction>(instructions);

            AfterConstantFolding = ApplyConstantFoldingWithCopy(optimized);

            AfterCopyPropagation = ApplyCopyPropagationWithCopy(AfterConstantFolding);

            return AfterCopyPropagation;
        }

        private List<TacInstruction> ApplyConstantFoldingWithCopy(List<TacInstruction> instructions)
        {
            var result = new List<TacInstruction>();
            foreach (var instr in instructions)
            {
                result.Add(new TacInstruction(instr.Op, instr.Arg1, instr.Arg2, instr.Result));
            }

            bool changed = true;
            int maxPasses = 5;
            int pass = 0;

            while (changed && pass < maxPasses)
            {
                changed = false;
                pass++;

                for (int i = 0; i < result.Count; i++)
                {
                    var instr = result[i];

                    if (IsArithmeticOrLogicalOp(instr.Op))
                    {
                        bool isLeftConst = IsConstant(instr.Arg1);
                        bool isRightConst = IsConstant(instr.Arg2);

                        if (isLeftConst && isRightConst)
                        {
                            int leftVal = GetConstantValue(instr.Arg1);
                            int rightVal = GetConstantValue(instr.Arg2);
                            int computedResult = ComputeOperation(instr.Op, leftVal, rightVal);

                            result[i] = new TacInstruction("=", computedResult.ToString(), "", instr.Result);
                            ConstantFoldingCount++;
                            changed = true;
                        }
                    }
                    else if (instr.Op == "uminus" && IsConstant(instr.Arg1))
                    {
                        int val = GetConstantValue(instr.Arg1);
                        result[i] = new TacInstruction("=", (-val).ToString(), "", instr.Result);
                        ConstantFoldingCount++;
                        changed = true;
                    }
                }
            }

            return result;
        }

        private List<TacInstruction> ApplyCopyPropagationWithCopy(List<TacInstruction> instructions)
        {
            var result = new List<TacInstruction>();
            foreach (var instr in instructions)
            {
                result.Add(new TacInstruction(instr.Op, instr.Arg1, instr.Arg2, instr.Result));
            }

            bool changed = true;
            int maxPasses = 5;
            int pass = 0;

            while (changed && pass < maxPasses)
            {
                changed = false;
                pass++;

                Dictionary<string, string> copyMap = new Dictionary<string, string>();

                for (int i = 0; i < result.Count; i++)
                {
                    var instr = result[i];

                    if (instr.Op == "=" && IsVariableOrConstant(instr.Arg1))
                    {
                        string source = instr.Arg1;
                        string target = instr.Result;

                        if (!copyMap.ContainsKey(target))
                        {
                            copyMap[target] = source;
                        }
                    }
                    else if (!string.IsNullOrEmpty(instr.Result))
                    {
                        if (copyMap.ContainsKey(instr.Result))
                        {
                            copyMap.Remove(instr.Result);
                        }
                    }
                }

                for (int i = 0; i < result.Count; i++)
                {
                    var instr = result[i];
                    bool instrChanged = false;

                    if (instr.Op == "=" && copyMap.ContainsKey(instr.Result))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(instr.Arg1) && copyMap.ContainsKey(instr.Arg1))
                    {
                        string original = instr.Arg1;
                        string newValue = copyMap[instr.Arg1];

                        if (original != newValue)
                        {
                            instr.Arg1 = newValue;
                            instrChanged = true;
                            CopyPropagationCount++;
                        }
                    }

                    if (!string.IsNullOrEmpty(instr.Arg2) && copyMap.ContainsKey(instr.Arg2))
                    {
                        string original = instr.Arg2;
                        string newValue = copyMap[instr.Arg2];

                        if (original != newValue)
                        {
                            instr.Arg2 = newValue;
                            instrChanged = true;
                            CopyPropagationCount++;
                        }
                    }

                    if (instrChanged)
                    {
                        result[i] = instr;
                        changed = true;
                    }
                }

                if (changed)
                {
                    for (int i = result.Count - 1; i >= 0; i--)
                    {
                        var instr = result[i];
                        if (instr.Op == "=" && IsVariableOrConstant(instr.Arg1))
                        {
                            string target = instr.Result;
                            bool used = false;

                            for (int j = 0; j < result.Count; j++)
                            {
                                if (j == i) continue;
                                var other = result[j];
                                if ((other.Arg1 == target || other.Arg2 == target) && other.Op != "label")
                                {
                                    used = true;
                                    break;
                                }
                            }

                            if (!used && IsTemporaryVariable(target))
                            {
                                result.RemoveAt(i);
                                changed = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool IsArithmeticOrLogicalOp(string op)
        {
            return op == "+" || op == "-" || op == "*" || op == "/" ||
                   op == "<" || op == ">" || op == "<=" || op == ">=" ||
                   op == "==" || op == "!=";
        }

        private bool IsConstant(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return int.TryParse(value, out _);
        }

        private int GetConstantValue(string value)
        {
            if (int.TryParse(value, out int result))
                return result;
            return 0;
        }

        private bool IsVariable(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return !IsConstant(value) && value != "0" && value != "1" && !value.StartsWith("\"");
        }

        private bool IsVariableOrConstant(string value)
        {
            return IsVariable(value) || IsConstant(value);
        }

        private bool IsTemporaryVariable(string value)
        {
            return !string.IsNullOrEmpty(value) && value.StartsWith("t");
        }

        private int ComputeOperation(string op, int left, int right)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right != 0 ? left / right : 0,
                "<" => left < right ? 1 : 0,
                ">" => left > right ? 1 : 0,
                "<=" => left <= right ? 1 : 0,
                ">=" => left >= right ? 1 : 0,
                "==" => left == right ? 1 : 0,
                "!=" => left != right ? 1 : 0,
                _ => 0
            };
        }

        public string GetOptimizationStats()
        {
            return $"Свертка констант: {ConstantFoldingCount} применений\n" +
                   $"Распространение копий: {CopyPropagationCount} применений";
        }
    }
}
