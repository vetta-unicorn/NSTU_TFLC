using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.RegularExpressions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class FileExtensionAutomaton
    {
        private enum State
        {
            Start, Path, Dot, D, Do, Doc, D_o, Docx, P, Pd, Pdf,
            J, Jp, Jpg, Jpeg, Pn, Png, G, Gi, Gif, Accept, Reject
        }

        private State currentState;
        private string input;
        private int position;

        public bool Match(string fileName)
        {
            input = fileName;
            position = 0;
            currentState = State.Start;

            while (currentState != State.Accept && currentState != State.Reject && position <= input.Length)
            {
                char c = position < input.Length ? input[position] : '\0';

                switch (currentState)
                {
                    case State.Start:
                        if (c == '\0') currentState = State.Reject;
                        else if (c == '.') currentState = State.Reject;
                        else currentState = State.Path;
                        position++;
                        break;

                    case State.Path:
                        if (c == '\0') currentState = State.Reject;
                        else if (c == '.') currentState = State.Dot;
                        position++;
                        break;

                    case State.Dot:
                        if (c == 'd') currentState = State.D;
                        else if (c == 'p') currentState = State.P;
                        else if (c == 'j') currentState = State.J;
                        else if (c == 'g') currentState = State.G;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.D:
                        if (c == 'o') currentState = State.Do;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Do:
                        if (c == 'c') currentState = State.Doc;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Doc:
                        if (c == '\0') currentState = State.Accept;
                        else if (c == 'x') currentState = State.Docx;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Docx:
                        if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.P:
                        if (c == 'd') currentState = State.Pd;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Pd:
                        if (c == 'f') currentState = State.Pdf;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Pdf:
                        if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.J:
                        if (c == 'p') currentState = State.Jp;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Jp:
                        if (c == 'g') currentState = State.Jpg;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Jpg:
                        if (c == '\0') currentState = State.Accept;
                        else if (c == 'e') currentState = State.Jpeg;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Jpeg:
                        if (c == 'g')
                        {
                            position++;
                            if (position == input.Length) currentState = State.Accept;
                            else currentState = State.Reject;
                        }
                        else currentState = State.Reject;
                        break;

                    case State.G:
                        if (c == 'i') currentState = State.Gi;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Gi:
                        if (c == 'f') currentState = State.Gif;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Gif:
                        if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        position++;
                        break;

                    case State.Accept:
                    case State.Reject:
                        break;
                }
            }

            return currentState == State.Accept;
        }

        public List<Expression> FindAllMatches(string filePath)
        {
            List<Expression> results = new List<Expression>();

            if (!File.Exists(filePath))
            {
                results.Add(new Expression
                {
                    Message = $"Ошибка: файл не найден - {filePath}",
                    Line = -1,
                    StartPosition = -1,
                    EndPosition = -1
                });
                return results;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                string line = lines[lineNum];
                FindMatchesInLine(line, lineNum + 1, results);
            }

            return results;
        }

        private void FindMatchesInLine(string line, int lineNumber, List<Expression> results)
        {
            int pos = 0;

            while (pos < line.Length)
            {
                while (pos < line.Length && !IsValidPathStart(line[pos]))
                {
                    pos++;
                }

                if (pos >= line.Length) break;

                int startPos = pos;
                int endPos = TryMatchExtension(line, startPos);

                if (endPos > startPos)
                {
                    string matchedText = line.Substring(startPos, endPos - startPos);
                    results.Add(new Expression
                    {
                        Message = $"Найдено расширение файла: {matchedText}",
                        Line = lineNumber,
                        StartPosition = startPos,
                        EndPosition = endPos
                    });
                    pos = endPos + 1;
                }
                else
                {
                    pos++;
                }
            }
        }

        private bool IsValidPathStart(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ';
        }

        private int TryMatchExtension(string line, int startPos)
        {
            int currentPos = startPos;
            currentState = State.Start;

            while (currentState != State.Accept && currentState != State.Reject && currentPos <= line.Length)
            {
                char c = currentPos < line.Length ? line[currentPos] : '\0';

                switch (currentState)
                {
                    case State.Start:
                        if (c == '\0') currentState = State.Reject;
                        else if (c == '.') currentState = State.Reject;
                        else currentState = State.Path;
                        currentPos++;
                        break;

                    case State.Path:
                        if (c == '\0') currentState = State.Reject;
                        else if (c == '.') currentState = State.Dot;
                        currentPos++;
                        break;

                    case State.Dot:
                        if (c == 'd') currentState = State.D;
                        else if (c == 'p') currentState = State.P;
                        else if (c == 'j') currentState = State.J;
                        else if (c == 'g') currentState = State.G;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.D:
                        if (c == 'o') currentState = State.Do;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Do:
                        if (c == 'c') currentState = State.Doc;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Doc:
                        if (c == '\0') currentState = State.Accept;
                        else if (c == 'x') currentState = State.Docx;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Docx:
                        if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.P:
                        if (c == 'd') currentState = State.Pd;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Pd:
                        if (c == 'f') currentState = State.Pdf;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Pdf:
                        if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.J:
                        if (c == 'p') currentState = State.Jp;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Jp:
                        if (c == 'g') currentState = State.Jpg;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Jpg:
                        if (c == '\0') currentState = State.Accept;
                        else if (c == 'e') currentState = State.Jpeg;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Jpeg:
                        if (c == 'g')
                        {
                            currentPos++;
                            if (currentPos == line.Length || line[currentPos] == '\0' ||
                                !char.IsLetterOrDigit(line[currentPos]))
                                currentState = State.Accept;
                            else currentState = State.Reject;
                        }
                        else currentState = State.Reject;
                        break;

                    case State.G:
                        if (c == 'i') currentState = State.Gi;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Gi:
                        if (c == 'f') currentState = State.Gif;
                        else currentState = State.Reject;
                        currentPos++;
                        break;

                    case State.Gif:
                        if (c == '\0' || !char.IsLetterOrDigit(c))
                            currentState = State.Accept;
                        else currentState = State.Reject;
                        currentPos++;
                        break;
                }
            }

            if (currentState == State.Accept)
            {
                return currentPos - 1;
            }

            return -1;
        }
    }

    public class NumberAutomaton
    {
        private enum State
        {
            Start, Sign, IntegerPart, DecimalPoint, FractionalPart, Accept, Reject
        }

        private State currentState;
        private string input;
        private int position;

        public bool Match(string number)
        {
            input = number;
            position = 0;
            currentState = State.Start;

            while (currentState != State.Accept && currentState != State.Reject && position <= input.Length)
            {
                char c = position < input.Length ? input[position] : '\0';

                switch (currentState)
                {
                    case State.Start:
                        if (c == '-') currentState = State.Sign;
                        else if (char.IsDigit(c)) { currentState = State.IntegerPart; position++; }
                        else if (c == '.' || c == ',') currentState = State.Reject;
                        else if (c == '\0') currentState = State.Reject;
                        else currentState = State.Reject;
                        if (c == '-') position++;
                        break;

                    case State.Sign:
                        if (char.IsDigit(c)) { currentState = State.IntegerPart; position++; }
                        else if (c == '.' || c == ',') currentState = State.Reject;
                        else currentState = State.Reject;
                        break;

                    case State.IntegerPart:
                        if (char.IsDigit(c)) { position++; }
                        else if (c == '.' || c == ',') { currentState = State.DecimalPoint; position++; }
                        else if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        break;

                    case State.DecimalPoint:
                        if (char.IsDigit(c)) { currentState = State.FractionalPart; position++; }
                        else currentState = State.Reject;
                        break;

                    case State.FractionalPart:
                        if (char.IsDigit(c)) { position++; }
                        else if (c == '\0') currentState = State.Accept;
                        else currentState = State.Reject;
                        break;

                    case State.Accept:
                    case State.Reject:
                        break;
                }
            }

            return currentState == State.Accept;
        }

        public List<Expression> FindAllMatches(string filePath)
        {
            List<Expression> results = new List<Expression>();

            if (!File.Exists(filePath))
            {
                results.Add(new Expression
                {
                    Message = $"Ошибка: файл не найден - {filePath}",
                    Line = -1,
                    StartPosition = -1,
                    EndPosition = -1
                });
                return results;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                string line = lines[lineNum];
                FindMatchesInLine(line, lineNum + 1, results);
            }

            return results;
        }

        private void FindMatchesInLine(string line, int lineNumber, List<Expression> results)
        {
            int pos = 0;

            while (pos < line.Length)
            {
                while (pos < line.Length && !char.IsDigit(line[pos]) && line[pos] != '-' && line[pos] != '+' &&
                       line[pos] != '.' && line[pos] != ',')
                {
                    pos++;
                }

                if (pos >= line.Length) break;

                int startPos = pos;
                int endPos = TryMatchNumber(line, startPos);

                if (endPos > startPos)
                {
                    string matchedText = line.Substring(startPos, endPos - startPos + 1);
                    results.Add(new Expression
                    {
                        Message = $"Найдено число: {matchedText}",
                        Line = lineNumber,
                        StartPosition = startPos,
                        EndPosition = endPos
                    });
                    pos = endPos + 1;
                }
                else
                {
                    pos++;
                }
            }
        }

        private int TryMatchNumber(string line, int startPos)
        {
            int currentPos = startPos;
            currentState = State.Start;

            while (currentState != State.Accept && currentState != State.Reject && currentPos <= line.Length)
            {
                char c = currentPos < line.Length ? line[currentPos] : '\0';

                switch (currentState)
                {
                    case State.Start:
                        if (c == '-')
                        {
                            currentState = State.Sign;
                            currentPos++;
                        }
                        else if (char.IsDigit(c))
                        {
                            currentState = State.IntegerPart;
                            currentPos++;
                        }
                        else if (c == '.' || c == ',')
                            currentState = State.Reject;
                        else if (c == '\0')
                            currentState = State.Reject;
                        else
                            currentState = State.Reject;
                        break;

                    case State.Sign:
                        if (char.IsDigit(c))
                        {
                            currentState = State.IntegerPart;
                            currentPos++;
                        }
                        else if (c == '.' || c == ',')
                            currentState = State.Reject;
                        else
                            currentState = State.Reject;
                        break;

                    case State.IntegerPart:
                        if (char.IsDigit(c))
                        {
                            currentPos++;
                        }
                        else if (c == '.' || c == ',')
                        {
                            currentState = State.DecimalPoint;
                            currentPos++;
                        }
                        else if (c == '\0' || !char.IsDigit(c))
                        {
                            currentState = State.Accept;
                        }
                        else
                            currentState = State.Reject;
                        break;

                    case State.DecimalPoint:
                        if (char.IsDigit(c))
                        {
                            currentState = State.FractionalPart;
                            currentPos++;
                        }
                        else
                            currentState = State.Reject;
                        break;

                    case State.FractionalPart:
                        if (char.IsDigit(c))
                        {
                            currentPos++;
                        }
                        else if (c == '\0' || !char.IsDigit(c))
                        {
                            currentState = State.Accept;
                        }
                        else
                            currentState = State.Reject;
                        break;
                }
            }

            if (currentState == State.Accept)
            {
                return currentPos - 1;
            }

            return -1;
        }
    }

    public class PasswordStrengthAutomaton
    {
        private enum State
        {
            Start, HasUpper, HasLower, HasDigit, HasSpecial, Accept, Reject
        }

        private State currentState;
        private string password;
        private int index;
        private bool hasUpper, hasLower, hasDigit, hasSpecial;

        public bool IsStrong(string password)
        {
            this.password = password;
            index = 0;
            hasUpper = hasLower = hasDigit = hasSpecial = false;
            currentState = State.Start;

            if (password.Length < 14)
                return false;

            while (index < password.Length && currentState != State.Reject)
            {
                char c = password[index];

                switch (currentState)
                {
                    case State.Start:
                        if (char.IsUpper(c) && IsRussianUpper(c)) hasUpper = true;
                        else if (char.IsLower(c) && IsRussianLower(c)) hasLower = true;
                        else if (char.IsDigit(c)) hasDigit = true;
                        else if (IsSpecialChar(c)) hasSpecial = true;

                        if (hasUpper && hasLower && hasDigit && hasSpecial)
                            currentState = State.Accept;

                        index++;
                        break;

                    case State.Accept:
                        index = password.Length;
                        break;

                    case State.Reject:
                        break;
                }
            }

            return currentState == State.Accept || (hasUpper && hasLower && hasDigit && hasSpecial);
        }

        public List<Expression> FindAllMatches(string filePath)
        {
            List<Expression> results = new List<Expression>();

            if (!File.Exists(filePath))
            {
                results.Add(new Expression
                {
                    Message = $"Ошибка: файл не найден - {filePath}",
                    Line = -1,
                    StartPosition = -1,
                    EndPosition = -1
                });
                return results;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                string line = lines[lineNum];
                FindPasswordsInLine(line, lineNum + 1, results);
            }

            return results;
        }

        private void FindPasswordsInLine(string line, int lineNumber, List<Expression> results)
        {
            string[] separators = new string[] { " ", "\t", ",", ";", ":", "!", "?", ".", "(", ")", "[", "]", "{", "}" };
            string[] potentialPasswords = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string potentialPassword in potentialPasswords)
            {
                if (IsStrong(potentialPassword))
                {
                    int startPos = line.IndexOf(potentialPassword);
                    if (startPos >= 0)
                    {
                        results.Add(new Expression
                        {
                            Message = $"Найден надёжный пароль: {potentialPassword}",
                            Line = lineNumber,
                            StartPosition = startPos,
                            EndPosition = startPos + potentialPassword.Length - 1
                        });
                    }
                }
            }
        }

        private bool IsRussianUpper(char c)
        {
            return (c >= 'А' && c <= 'Я') || c == 'Ё';
        }

        private bool IsRussianLower(char c)
        {
            return (c >= 'а' && c <= 'я') || c == 'ё';
        }

        private bool IsSpecialChar(char c)
        {
            string specials = "()#?!|/@/$%\\^&*-_.";
            return specials.Contains(c);
        }
    }

}
