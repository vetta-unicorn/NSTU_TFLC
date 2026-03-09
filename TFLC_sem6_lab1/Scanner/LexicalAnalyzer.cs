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
        public bool IsError => code == -1;
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

    public class DisplayTokens
    {
        public void LoadAndDisplayTokens(string filePath, LexicalAnalyzer scanner, 
            DataGridView OutputTable)
        {
            try
            {
                List<TableLine> tokens = scanner.AnalyzeText(filePath);
                var displayTokens = new List<TokenDisplay>();
                foreach (var token in tokens)
                {
                    displayTokens.Add(new TokenDisplay
                    {
                        code = token.code,
                        type = token.type,
                        token = token.token,
                        Location = $"строка {token.line_number + 1}, {token.start_pos + 1}-{token.end_pos}"
                    });
                }

                OutputTable.DataSource = null;
                OutputTable.DataSource = displayTokens;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class LexicalAnalyzer
    {
        public List<TableLine> AnalyzeText(string filePath)
        {
            int lineNumber = -1;
            List<TableLine> tabs = new List<TableLine>();

            foreach (string line in File.ReadLines(filePath))
            {
                lineNumber++;

                int pos = 0;
                while (pos < line.Length)
                {
                    if (char.IsWhiteSpace(line[pos]))
                    {
                        pos++;
                        continue;
                    }

                    bool found = false;

                    // code 1 id
                    if (line[pos] == '$')
                    {
                        int endPos = pos + 1;
                        while (endPos < line.Length &&
                              (char.IsLetterOrDigit(line[endPos]) || line[endPos] == '_'))
                        {
                            endPos++;
                        }

                        string token = line.Substring(pos, endPos - pos);
                        tabs.Add(new TableLine(1, "id", token, lineNumber, pos, endPos));
                        pos = endPos;
                        found = true;
                    }

                    // code 2 do
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == "do")
                    {
                        tabs.Add(new TableLine(2, "do", "do", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code 3 while
                    else if (pos + 4 < line.Length && line.Substring(pos, 5) == "while")
                    {
                        tabs.Add(new TableLine(3, "while", "while", lineNumber, pos, pos + 5));
                        pos += 5;
                        found = true;
                    }
                    
                    // code 4 int
                    else if (char.IsDigit(line[pos]))
                    {
                        int endPos = pos;
                        while (endPos < line.Length && char.IsDigit(line[endPos]))
                        {
                            endPos++;
                        }

                        string token = line.Substring(pos, endPos - pos);
                        tabs.Add(new TableLine(4, "целое число без знака", token, lineNumber, pos, endPos));
                        pos = endPos;
                        found = true;
                    }

                    // code 5 +
                    else if (pos < line.Length && line.Substring(pos, 1) == "+" 
                        && line.Substring(pos, 2) != "++")
                    {
                        tabs.Add(new TableLine(5, "оператор сложения", "+", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 6 ++
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == "++")
                    {
                        tabs.Add(new TableLine(6, "оператор инкремента", "++", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code 7 -
                    else if (pos < line.Length && line.Substring(pos, 1) == "-"
                        && line.Substring(pos, 2) != "--")
                    {
                        tabs.Add(new TableLine(7, "оператор вычитания", "-", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 8 --
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == "--")
                    {
                        tabs.Add(new TableLine(8, "оператор декремента", "--", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code 9 {
                    else if (pos < line.Length && line.Substring(pos, 1) == "{")
                    {
                        tabs.Add(new TableLine(9, "открывающая фигурная скобка", 
                            "{", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 10 }
                    else if (pos < line.Length && line.Substring(pos, 1) == "}")
                    {
                        tabs.Add(new TableLine(9, "закрывающая фигурная скобка",
                            "}", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 11 (
                    else if (pos < line.Length && line.Substring(pos, 1) == "(")
                    {
                        tabs.Add(new TableLine(11, "открывающая круглая скобка",
                            "(", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 12 )
                    else if (pos < line.Length && line.Substring(pos, 1) == ")")
                    {
                        tabs.Add(new TableLine(12, "закрывающая круглая скобка",
                            ")", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 13 <
                    else if (pos < line.Length && line.Substring(pos, 1) == "<"
                        && line.Substring(pos, 2) != "<=")
                    {
                        tabs.Add(new TableLine(13, "оператор 'меньше'",
                            "<", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 14 <=
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == "<=")
                    {
                        tabs.Add(new TableLine(14, "оператор 'меньше или равно'",
                            "<=", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code 15 >
                    else if (pos < line.Length && line.Substring(pos, 1) == ">"
                        && line.Substring(pos, 2) != ">=")
                    {
                        tabs.Add(new TableLine(15, "оператор 'больше'",
                            ">", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 16 >=
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == ">=")
                    {
                        tabs.Add(new TableLine(16, "оператор 'больше или равно'",
                            ">=", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code 17 ;
                    else if (pos < line.Length && line.Substring(pos, 1) == ";")
                    {
                        tabs.Add(new TableLine(17, "точка с запятой",
                            ";", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 18 =
                    else if (pos < line.Length && line.Substring(pos, 1) == "="
                        && line.Substring(pos, 2) != "==")
                    {
                        tabs.Add(new TableLine(18, "оператор присваивания",
                            "=", lineNumber, pos, pos + 1));
                        pos += 1;
                        found = true;
                    }

                    // code 19 ==
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == "==")
                    {
                        tabs.Add(new TableLine(19, "оператор равенства",
                            "==", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code 20 !=
                    else if (pos + 1 < line.Length && line.Substring(pos, 2) == "!=")
                    {
                        tabs.Add(new TableLine(20, "оператор неравенства",
                            "!=", lineNumber, pos, pos + 2));
                        pos += 2;
                        found = true;
                    }

                    // code -1 ERROR
                    if (!found)
                    {
                        int errorEnd = pos + 1;
                        while (errorEnd < line.Length && !char.IsWhiteSpace(line[errorEnd]))
                        {
                            errorEnd++;
                        }

                        string errorToken = line.Substring(pos, errorEnd - pos);
                        tabs.Add(new TableLine(-1, "ERROR: неизвестный токен!",
                            errorToken, lineNumber, pos, errorEnd));
                        pos = errorEnd;
                    }
                }
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
