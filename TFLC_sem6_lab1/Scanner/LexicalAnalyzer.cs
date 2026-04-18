using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TFLC_sem6_lab1.Scanner
{
    public class TokenDict
    {
        public Dictionary<int, string> tokens { get; }
        public TokenDict()
        {
            tokens = new Dictionary<int, string>
            {
                {1, "id" },
                {2, "while" },
                {3, "do" },
                {4, "целое число без знака" },
                {5, "оператор сложения" },
                {6, "оператор инкремента" },
                {7, "оператор вычитания" },
                {8, "оператор декремента" },
                {9, "открывающая фигурная скоба" },
                {10, "закрывающая фигурная скобка" },
                {11, "открывающая круглая скобка" },
                {12, "закрывающая круглая скобка" },
                {13, "оператор меньше" },
                {14, "оператор меньше или равно" },
                {15, "оператор больше" },
                {16, "оператор больше или равно" },
                {17, "точка с запятой" },
                {18, "оператор присваивания" },
                {19, "оператор равенства" },
                {20, "оператор неравенства" },
                {-1, "ERROR: неизвестный токен" }
            };
        }
    }

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
        public TableLine(int code, int line_number, int start_pos, int end_pos)
        {
            this.code = code;
            this.type = "default";
            this.token = "default";
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
        public TokenDict tokenDict = new TokenDict();
        public LexicalAnalyzer()
        {
            tokenDict = new TokenDict();
        }
        public List<TableLine> AnalyzeText(string filePath)
        {
            int lineNumber = -1;
            List<TableLine> tabs = new List<TableLine>();
            try
            {
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

                        // code 17 ;
                        if (pos < line.Length && line.Substring(pos, 1) == ";")
                        {
                            tabs.Add(new TableLine(17, tokenDict.tokens[17],
                                ";", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 2 do
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "do"
                            && (pos + 2 < line.Length && line[pos + 2] == ' ' || pos + 2 >= line.Length))
                        {
                            tabs.Add(new TableLine(3, tokenDict.tokens[3], "do", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 3 while
                        else if (pos + 4 < line.Length && line.Substring(pos, 5) == "while"
                            && (pos + 5 < line.Length && line[pos + 5] == ' ' || pos + 5 >= line.Length))
                        {
                            tabs.Add(new TableLine(2, tokenDict.tokens[2], "while", lineNumber, pos, pos + 5));
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
                            tabs.Add(new TableLine(4, tokenDict.tokens[4], token, lineNumber, pos, endPos));
                            pos = endPos;
                            found = true;
                        }

                        // code 6 ++
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "++")
                        {
                            tabs.Add(new TableLine(6, tokenDict.tokens[6], "++", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 5 +
                        else if (pos < line.Length && line.Substring(pos, 1) == "+")
                        {
                            tabs.Add(new TableLine(5, tokenDict.tokens[5], "+", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 8 --
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "--")
                        {
                            tabs.Add(new TableLine(8, tokenDict.tokens[8], "--", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 7 -
                        else if (pos < line.Length && line.Substring(pos, 1) == "-")
                        {
                            tabs.Add(new TableLine(7, tokenDict.tokens[7], "-", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 9 {
                        else if (pos < line.Length && line.Substring(pos, 1) == "{")
                        {
                            tabs.Add(new TableLine(9, tokenDict.tokens[9],
                                "{", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 10 }
                        else if (pos < line.Length && line.Substring(pos, 1) == "}")
                        {
                            tabs.Add(new TableLine(10, tokenDict.tokens[10],
                                "}", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 11 (
                        else if (pos < line.Length && line.Substring(pos, 1) == "(")
                        {
                            tabs.Add(new TableLine(11, tokenDict.tokens[11],
                                "(", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 12 )
                        else if (pos < line.Length && line.Substring(pos, 1) == ")")
                        {
                            tabs.Add(new TableLine(12, tokenDict.tokens[12],
                                ")", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 13 <
                        else if (pos < line.Length && line.Substring(pos, 1) == "<"
                            && line.Substring(pos, 2) != "<=")
                        {
                            tabs.Add(new TableLine(13, tokenDict.tokens[13],
                                "<", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 14 <=
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "<=")
                        {
                            tabs.Add(new TableLine(14, tokenDict.tokens[14],
                                "<=", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 15 >
                        else if (pos < line.Length && line.Substring(pos, 1) == ">"
                            && line.Substring(pos, 2) != ">=")
                        {
                            tabs.Add(new TableLine(15, tokenDict.tokens[15],
                                ">", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 16 >=
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == ">=")
                        {
                            tabs.Add(new TableLine(16, tokenDict.tokens[16],
                                ">=", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 18 =
                        else if (pos < line.Length && line.Substring(pos, 1) == "="
                            && (pos + 2 < line.Length && line.Substring(pos, 2) != "==")
                            || pos + 2 >= line.Length)
                        {
                            tabs.Add(new TableLine(18, tokenDict.tokens[18],
                                "=", lineNumber, pos, pos + 1));
                            pos += 1;
                            found = true;
                        }

                        // code 19 ==
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "==")
                        {
                            tabs.Add(new TableLine(19, tokenDict.tokens[19],
                                "==", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 20 !=
                        else if (pos + 1 < line.Length && line.Substring(pos, 2) == "!=")
                        {
                            tabs.Add(new TableLine(20, tokenDict.tokens[20],
                                "!=", lineNumber, pos, pos + 2));
                            pos += 2;
                            found = true;
                        }

                        // code 1 id
                        else if (line[pos] == '$')
                        {
                            int endPos = pos + 1;
                            while (endPos < line.Length &&
                                  (char.IsLetterOrDigit(line[endPos]) || line[endPos] == '_'))
                            {
                                endPos++;
                            }

                            string token = line.Substring(pos, endPos - pos);
                            tabs.Add(new TableLine(1, tokenDict.tokens[1], token, lineNumber, pos, endPos));
                            pos = endPos;
                            found = true;
                        }

                        // code -1 ERROR
                        if (!found)
                        {
                            int errorEnd = pos + 1;
                            while (errorEnd < line.Length && !char.IsWhiteSpace(line[errorEnd])
                                && line[errorEnd] != '+' && line[errorEnd] != '-'
                                && line[errorEnd] != ';' && line[errorEnd] != ')'
                                && line[errorEnd] != '}')
                            {
                                errorEnd++;
                            }

                            string errorToken = line.Substring(pos, errorEnd - pos);
                            tabs.Add(new TableLine(-1, tokenDict.tokens[-1],
                                errorToken, lineNumber, pos, errorEnd));
                            pos = errorEnd;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}");
                return new List<TableLine>(); // Возвращаем пустой список
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
