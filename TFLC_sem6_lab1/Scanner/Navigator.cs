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
                var match = System.Text.RegularExpressions.Regex.Match(location, @"строка (\d+), (\d+)-(\d+)");

                if (match.Success)
                {
                    int line = int.Parse(match.Groups[1].Value);
                    int startPos = int.Parse(match.Groups[2].Value);
                    int endPos = int.Parse(match.Groups[3].Value);

                    SetCursorToPosition(line, startPos, endPos, InputTextBox);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при парсинге местоположения: {ex.Message}");
            }
        }

        private void SetCursorToPosition(int line, int startPos, int endPos, RichTextBox InputTextBox)
        {
            if (InputTextBox != null)
            {
                string[] lines = InputTextBox.Lines;

                if (line <= lines.Length)
                {
                    int charIndex = 0;
                    for (int i = 0; i < line - 1; i++)
                    {
                        charIndex += lines[i].Length + 1;
                    }
                    charIndex += startPos - 1;

                    InputTextBox.Focus();
                    InputTextBox.Select(charIndex, endPos - startPos + 1);
                    InputTextBox.ScrollToCaret();
                }
            }
        }
    }
}
