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
}
