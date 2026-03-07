using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TFLC_sem6_lab1.Scanner
{
    public class TableLine
    {
        public int code {  get; set; }
        public string type { get; set; }
        public string token { get; set; }
        public int line_number { get; set; }
        public int start_pos { get; set; }
        public int end_pos { get; set; }
        public TableLine(int code, string type, string token, int line_number, int start_pos, int end_pos)
        {
            this.code = code;
            this.type = type;
            this.token = token;
            this.line_number = line_number;
            this.start_pos = start_pos;
            this.end_pos = end_pos;
        }
        public TableLine() { }
    }

    public class LexicalAnalyzer
    {
        public List<TableLine> AnalyzeText(string filePath)
        {
            int lineNumber = 0;
            List<TableLine> tabs = new List<TableLine>();

            foreach (string line in File.ReadLines(filePath))
            {
                int start_pos = -1;
                int end_pos = -1;
                
                // code 1 id
                if (line.Contains("$"))
                {
                    start_pos = line.IndexOf("$");
                    end_pos = start_pos + 1;
                    TableLine tab = new TableLine();
                    while (Char.IsLetter(line[end_pos]) || line[end_pos] == '_' || Char.IsDigit(line[end_pos]))
                    {
                        end_pos++;
                        tab = new TableLine(1, "id", 
                            line[start_pos..end_pos], lineNumber, start_pos, end_pos);
                    }
                    tabs.Add(tab);
                }
                if (line.Contains("do"))
                {
                    start_pos = line.IndexOf("d");
                    end_pos= start_pos + 2;
                    TableLine tab = new TableLine(2, "do", line[start_pos..end_pos],
                        lineNumber, start_pos, end_pos);
                    tabs.Add(tab);
                }
                if (line.Contains("while"))
                {
                    start_pos = line.IndexOf("w");
                    end_pos = start_pos + 5;
                    TableLine tab = new TableLine(2, "while", line[start_pos..end_pos],
                        lineNumber, start_pos, end_pos);
                    tabs.Add(tab);
                }
                lineNumber++;
            }
            return tabs;
        }
    }

    public class TokenDisplay
    {
        public int code { get; set; }
        public string type { get; set; }
        public string token { get; set; }
        public string Location { get; set; }
    }
}
