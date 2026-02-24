using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFLC_sem6_lab1.Handlers
{
    public static class RichTextBoxExtensions
    {
        private static ResourceManager _resourceManager;
        private static bool _headerAdded = false;

        static RichTextBoxExtensions()
        {
            _resourceManager = new ResourceManager("TFLC_sem6_lab1.Resources",
                                                  typeof(MainForm).Assembly);
        }

        public static void RefreshResources()
        {
            _resourceManager = new ResourceManager("TFLC_sem6_lab1.Resources",
                                                  typeof(MainForm).Assembly);
        }

        public static void LogLocalized(this RichTextBox rtb, string messageKey)
        {
            string message = _resourceManager.GetString(messageKey);
            rtb.AppendText((message ?? messageKey) + Environment.NewLine);
        }

        public static void LogLocalized(this RichTextBox rtb, string messageKey, params object[] args)
        {
            string message = _resourceManager.GetString(messageKey);
            if (message != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            rtb.AppendText((message ?? messageKey) + Environment.NewLine);
        }

      
        public static void LogLocalizedError(this RichTextBox rtb, string filePath, int line, int column, string messageKey)
        {
            if (rtb.Font.Name != "Consolas")
            {
                rtb.Font = new Font("Consolas", 10, FontStyle.Regular);
            }

            string message = _resourceManager.GetString(messageKey) ?? messageKey;

            string formattedMessage = string.Format("{0}\t{1}\t{2}\t{3}",
                filePath,
                line,
                column,
                message);

            int start = rtb.TextLength;
            rtb.AppendText(formattedMessage + Environment.NewLine);

        }


        public static void LogLocalizedError(this RichTextBox rtb, string filePath, int line, int column, string messageKey, params object[] args)
        {
            if (rtb.Font.Name != "Consolas")
            {
                rtb.Font = new Font("Consolas", 10, FontStyle.Regular);
            }

            string messageTemplate = _resourceManager.GetString(messageKey) ?? messageKey;
            string message = args.Length > 0 ? string.Format(messageTemplate, args) : messageTemplate;

            string formattedMessage = string.Format("{0}\t{1}\t{2}\t{3}",
                filePath,
                line,
                column,
                message);

            int start = rtb.TextLength;
            rtb.AppendText(formattedMessage + Environment.NewLine);
        }
    }

    public class StatusStripHandler
    {
        public void InitializeStatusStrip(ToolStripStatusLabel statusLabel, 
            ToolStripStatusLabel cursorPositionLabel, ToolStripStatusLabel fileInfoLabel,
            StatusStrip statusStrip1)
        {
            
            statusLabel.Spring = true;

            var separator1 = new ToolStripStatusLabel(" | ");
            var separator2 = new ToolStripStatusLabel(" | ");

            statusStrip1.Items.Clear();
            statusStrip1.Items.AddRange(new ToolStripItem[] {
            statusLabel,
            separator1,
            cursorPositionLabel,
            separator2,
            fileInfoLabel
        });
        }
        public void UpdateFileInfo(string currentFilePath, ToolStripStatusLabel fileInfoLabel,
            bool isTextModified)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                fileInfoLabel.Text = isTextModified ? "Новый файл*" : "Новый файл";
            }
            else
            {
                string fileName = Path.GetFileName(currentFilePath);
                fileInfoLabel.Text = isTextModified ? $"{fileName}*" : fileName;
            }
        }

        public void UpdateStatus(string message, ToolStripStatusLabel statusLabel)
        {
            statusLabel.Text = message;
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) =>
            {
                statusLabel.Text = "Готов к работе";
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
    }

    public class HotKeys
    {
        public void ConfigureMenuHotkeys(ToolStripMenuItem menuItem)
        {
            switch (menuItem.Text)
            {
                case string s when s == Resources.CopyText:
                    menuItem.ShortcutKeys = Keys.Control | Keys.C;
                    break;
                case string s when s == Resources.CutText:
                    menuItem.ShortcutKeys = Keys.Control | Keys.X;
                    break;
                case string s when s == Resources.PasteText:
                    menuItem.ShortcutKeys = Keys.Control | Keys.V;
                    break;
                case string s when s == Resources.SelectAllText:
                    menuItem.ShortcutKeys = Keys.Control | Keys.A;
                    break;
                case string s when s == Resources.DeleteText:
                    menuItem.ShortcutKeys = Keys.Delete;
                    break;
                case string s when s == Resources.CreateFile:
                    menuItem.ShortcutKeys = Keys.Control | Keys.N;
                    break;
                case string s when s == Resources.OpenFile:
                    menuItem.ShortcutKeys = Keys.Control | Keys.O;
                    break;
                case string s when s == Resources.CloseFile:
                    menuItem.ShortcutKeys = Keys.Control | Keys.F4;
                    break;
                case string s when s == Resources.ExitFile:
                    menuItem.ShortcutKeys = Keys.Control | Keys.F3;
                    break;
                case string s when s == Resources.RedoText:
                    menuItem.ShortcutKeys = Keys.Control | Keys.Right;
                    break;
                case string s when s == Resources.UndoText:
                    menuItem.ShortcutKeys = Keys.Control | Keys.Left;
                    break;
                case string s when s == Resources.ShowHelp:
                    menuItem.ShortcutKeys = Keys.Control | Keys.F1;
                    break;
                case string s when s == Resources.ShowAbout:
                    menuItem.ShortcutKeys = Keys.Control | Keys.H;
                    break;
            }

            foreach (ToolStripMenuItem subItem in menuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                ConfigureMenuHotkeys(subItem);
            }
        }
    }
}
