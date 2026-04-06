using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TFLC_sem6_lab1.Grammar;

namespace TFLC_sem6_lab1.RegularExpressions
{
    public class Expression
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }


        public class RegexSearcher
        {
            public void DisplayExpressions(List<Expression> expressions, DataGridView SyntaxTable)
            {
                SyntaxTable.Rows.Clear();

                foreach (var error in expressions)
                {
                    string locationText = "";
                    if (error.StartPosition >= 0 && error.EndPosition >= error.StartPosition)
                    {
                        locationText = $"строка {error.Line + 1}, {error.StartPosition}-{error.EndPosition}";
                    }
                    else if (error.Line >= 0)
                    {
                        locationText = $"строка {error.Line + 1}";
                    }
                    else
                    {
                        locationText = "местоположение не определено";
                    }

                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(SyntaxTable,
                        error.Message,
                        locationText
                    );

                    SyntaxTable.Rows.Add(row);
                }
            }

            public List<Expression> FindAllMatches(string filePath, string pattern)
            {
                var matches = new List<Expression>();

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return matches;

                if (string.IsNullOrEmpty(pattern))
                    return matches;

                try
                {
                    string text = File.ReadAllText(filePath, Encoding.UTF8);

                    RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

                    Regex regex = new Regex(pattern, options);
                    MatchCollection matchCollection = regex.Matches(text);

                    string[] lines = text.Split('\n');

                    foreach (Match match in matchCollection)
                    {
                        int lineNumber = GetLineNumber(text, match.Index, lines);

                        int positionInLine = GetPositionInLine(text, match.Index, lines, lineNumber) - 7;

                        var expression = new Expression
                        {
                            Message = match.Value,
                            Line = lineNumber,
                            StartPosition = positionInLine,
                            EndPosition = positionInLine + match.Length - 1
                        };

                        matches.Add(expression);
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Нет доступа к файлу: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Неизвестная ошибка: {ex.Message}");
                }

                return matches;
            }

            private int GetPositionInLine(string text, int globalIndex, string[] lines, int lineNumber)
            {
                if (lineNumber <= 0 || lineNumber > lines.Length)
                    return 0;

                int lineStartIndex = 0;
                for (int i = 0; i < lineNumber - 1; i++)
                {
                    lineStartIndex += lines[i].Length + 1; 
                }

                return globalIndex - lineStartIndex;
            }

            private int GetLineNumber(string text, int position, string[] lines)
            {
                int currentPosition = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    int lineLength = lines[i].Length;
                    int lineEndPosition = currentPosition + lineLength;

                    if (position >= currentPosition && position <= lineEndPosition)
                    {
                        return i;
                    }

                    currentPosition = lineEndPosition;
                }

                return 1;
            }

            public List<Expression> FindFiles(string filePath)
            {
                string pattern = @"\b[^\\/:*?""<>|\s]+\.(doc|docx|pdf|jpg|jpeg|png|gif)\b";

                return FindAllMatches(filePath, pattern);
            }

            public List<Expression> FindNumbers(string filePath)
            {
                string pattern = @"-?\d+(?:[.,]\d+)?(?!\d)";

                return FindAllMatches(filePath, pattern);
            }

            public bool IsPasswordStrong(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return false;
                string pattern = @"^(?=.*[А-ЯЁ])(?=.*[а-яё])(?=.*\d)(?=.*[()#?!|@$%^&*\-_.]).{14,}$";

                return Regex.IsMatch(password, pattern, RegexOptions.None);
            }

            public List<Expression> FindStrongPasswords(string filePath)
            {
                var strongPasswords = new List<Expression>();

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return strongPasswords;

                try
                {
                    string text = File.ReadAllText(filePath, Encoding.UTF8);

                    if (string.IsNullOrEmpty(text))
                        return strongPasswords;

                    string[] potentialPasswords = Regex.Split(text, @"[\s,;]+");

                    string[] lines = text.Split('\n');

                    foreach (string potential in potentialPasswords)
                    {
                        if (IsPasswordStrong(potential))
                        {
                            int index = text.IndexOf(potential);
                            if (index >= 0)
                            {
                                int positionInLine = GetPositionInLine(text, index, lines, GetLineNumber(text, index, lines)) - 7;

                                var expression = new Expression
                                {
                                    Message = potential,
                                    Line = GetLineNumber(text, index, lines),
                                    StartPosition = positionInLine,
                                    EndPosition = positionInLine + potential.Length - 1
                                };
                                strongPasswords.Add(expression);
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Нет доступа к файлу: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
                }

                return strongPasswords;
            }
        }
    }
}

   

