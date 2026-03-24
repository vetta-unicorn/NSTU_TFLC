using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.Scanner
{

    public class Navigator
    {
        public void NavigateToErrorLocation(string location, RichTextBox InputTextBox)
        {
            try
            {
                if (string.IsNullOrEmpty(location))
                    return;

                var match = System.Text.RegularExpressions.Regex.Match(location, @"строка (\d+), (\d+)-(\d+)");

                if (match.Success)
                {
                    int line = int.Parse(match.Groups[1].Value);
                    int startPos = int.Parse(match.Groups[2].Value);
                    int endPos = int.Parse(match.Groups[3].Value);

                    if (startPos >= 0 && endPos >= startPos)
                    {
                        SetCursorToPosition(line, startPos, endPos, InputTextBox);
                    }
                    else
                    {
                        SetCursorToLine(line, InputTextBox);
                    }
                }
                else
                {
                    var simpleMatch = System.Text.RegularExpressions.Regex.Match(location, @"строка (\d+)");
                    if (simpleMatch.Success)
                    {
                        int line = int.Parse(simpleMatch.Groups[1].Value);
                        SetCursorToLine(line, InputTextBox);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при парсинге местоположения: {ex.Message}");
            }
        }

        private void SetCursorToLine(int line, RichTextBox InputTextBox)
        {
            if (InputTextBox != null)
            {
                string[] lines = InputTextBox.Lines;

                if (line <= lines.Length && line > 0)
                {
                    int charIndex = 0;
                    for (int i = 0; i < line - 1; i++)
                    {
                        charIndex += lines[i].Length + 1;
                    }

                    int lineLength = lines[line - 1].Length;

                    InputTextBox.Focus();
                    InputTextBox.Select(charIndex, lineLength);
                    InputTextBox.ScrollToCaret();
                }
            }
        }

        private void SetCursorToPosition(int line, int startPos, int endPos, RichTextBox InputTextBox)
        {
            if (InputTextBox != null)
            {
                string[] lines = InputTextBox.Lines;

                if (line <= lines.Length && line > 0)
                {
                    int lineLength = lines[line - 1].Length;

                    if (startPos > lineLength)
                        startPos = lineLength;
                    if (endPos > lineLength)
                        endPos = lineLength;
                    if (startPos < 0)
                        startPos = 0;
                    if (endPos < startPos)
                        endPos = startPos;

                    int charIndex = 0;
                    for (int i = 0; i < line - 1; i++)
                    {
                        charIndex += lines[i].Length + 1;
                    }
                    charIndex += startPos;

                    int selectionLength = endPos - startPos + 1;
                    if (selectionLength <= 0)
                        selectionLength = 1;

                    InputTextBox.Focus();
                    InputTextBox.Select(charIndex, selectionLength);
                    InputTextBox.ScrollToCaret();
                }
            }
        }
    }
}
//    {
//        public void NavigateToErrorLocation(string location, RichTextBox InputTextBox)
//        {
//            try
//            {
//                var match = System.Text.RegularExpressions.Regex.Match(location, @"строка (\d+), (\d+)-(\d+)");

//                if (match.Success)
//                {
//                    int line = int.Parse(match.Groups[1].Value);
//                    int startPos = int.Parse(match.Groups[2].Value);
//                    int endPos = int.Parse(match.Groups[3].Value);

//                    SetCursorToPosition(line, startPos, endPos, InputTextBox);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при парсинге местоположения: {ex.Message}");
//            }
//        }

//        private void SetCursorToPosition(int line, int startPos, int endPos, RichTextBox InputTextBox)
//        {
//            if (InputTextBox != null)
//            {
//                string[] lines = InputTextBox.Lines;

//                if (line <= lines.Length)
//                {
//                    int charIndex = 0;
//                    for (int i = 0; i < line - 1; i++)
//                    {
//                        charIndex += lines[i].Length + 1;
//                    }
//                    charIndex += startPos - 1;

//                    InputTextBox.Focus();
//                    InputTextBox.Select(charIndex, endPos - startPos + 1);
//                    InputTextBox.ScrollToCaret();
//                }
//            }
//        }
//    }
//}
