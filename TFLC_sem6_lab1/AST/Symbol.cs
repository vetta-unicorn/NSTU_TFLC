using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.AST
{
    public class Symbol
    {
        public string Name { get; set; }
        public string Type { get; set; } // например: int
        public int? Value { get; set; }
    }

    public class SymbolTable
    {
        private Dictionary<string, Symbol> _symbols = new();

        public bool Declare(string name, string type)
        {
            if (_symbols.ContainsKey(name))
                return false;

            _symbols[name] = new Symbol { Name = name, Type = type };
            return true;
        }

        public Symbol Lookup(string name)
        {
            return _symbols.ContainsKey(name) ? _symbols[name] : null;
        }
    }
}
