using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.Handlers
{
    public class KeyWords
    {
        public void HighlightKeywords(RichTextBox InputTextBox, string[] keywords, Color keywordColor)
        {
            int selectionStart = InputTextBox.SelectionStart;
            int selectionLength = InputTextBox.SelectionLength;

            InputTextBox.SelectAll();
            InputTextBox.SelectionColor = Color.Black;

            foreach (string keyword in keywords)
            {
                int index = 0;
                while (index < InputTextBox.TextLength)
                {
                    index = InputTextBox.Text.IndexOf(keyword, index, StringComparison.Ordinal);
                    if (index == -1)
                        break;

                    if (IsWholeWord(index, keyword.Length, InputTextBox))
                    {
                        InputTextBox.Select(index, keyword.Length);
                        InputTextBox.SelectionColor = keywordColor;
                    }

                    index += keyword.Length;
                }
            }

            InputTextBox.Select(selectionStart, selectionLength);
            InputTextBox.SelectionColor = Color.Black;
        }

        private bool IsWholeWord(int index, int length, RichTextBox InputTextBox)
        {
            if (index > 0)
            {
                char prevChar = InputTextBox.Text[index - 1];
                if (char.IsLetterOrDigit(prevChar) || prevChar == '_')
                    return false;
            }

            if (index + length < InputTextBox.TextLength)
            {
                char nextChar = InputTextBox.Text[index + length];
                if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                    return false;
            }

            return true;
        }
    }

    public class DrawLines
    {
        public void DrawLineNumbers(Graphics g, RichTextBox InputTextBox)
        {
            if (InputTextBox.Lines.Length == 0) return;

            int firstCharIndex = InputTextBox.GetCharIndexFromPosition(new Point(0, 0));
            int firstLine = InputTextBox.GetLineFromCharIndex(firstCharIndex);

            int lastCharIndex = InputTextBox.GetCharIndexFromPosition(new Point(0, InputTextBox.ClientSize.Height));
            int lastLine = InputTextBox.GetLineFromCharIndex(lastCharIndex);

            for (int line = firstLine; line <= lastLine + 1 && line < InputTextBox.Lines.Length; line++)
            {
                int charIndex = InputTextBox.GetFirstCharIndexFromLine(line);
                if (charIndex == -1) continue;

                Point linePos = InputTextBox.GetPositionFromCharIndex(charIndex);

                using (Brush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString((line + 1).ToString(),
                               InputTextBox.Font,
                               brush,
                               5,
                               linePos.Y);
                }
            }
        }
    }
}
