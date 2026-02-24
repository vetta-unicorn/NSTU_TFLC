using System.Globalization;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using TFLC_sem6_lab1.ButtonHandlers;
using TFLC_sem6_lab1.HelpForms;
using TFLC_sem6_lab1.Handlers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace TFLC_sem6_lab1
{
    public partial class MainForm : Form
    {
        private bool isOpened = false;
        private string currentFilePath = "";
        private string fileText = "";
        ProcessFile processFile;
        private string userPath = @"Files\HelpForm.html";
        private string abPath = @"Files\AboutForm.html";
        string userHelpPath;
        private string aboutPath;
        private System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
        private FontDialog fontDialog1;

        Label lblSampleText;
        System.Windows.Forms.Button btnSelectFont;
        private Panel passPanelContainer;
        Font newFont;

        private ResourceManager resourceManager;
        private string currLang = "ru";

        private Panel lineNumberPanel;

        private ToolStripStatusLabel statusLabel = new ToolStripStatusLabel("Готов к работе");
        private ToolStripStatusLabel cursorPositionLabel = new ToolStripStatusLabel("Стр: 1, Стлб: 1");
        private ToolStripStatusLabel fileInfoLabel = new ToolStripStatusLabel("Новый файл");

        private bool isTextModified = false;

        private string[] keywords = { "class", "public", "private", "void", "int", "string", "if", "else", "for", "while" };
        private Color keywordColor = Color.Purple;

        KeyWords KeyWords = new KeyWords();
        StatusStripHandler statusStripHandler = new StatusStripHandler();
        DrawLines drawLines = new DrawLines();
        LangHandler langHandler = new LangHandler();
        HotKeys hotKeys = new HotKeys();

        private Dictionary<TabPage, RichTextBox> tabEditors = new Dictionary<TabPage, RichTextBox>();

        public MainForm()
        {
            InitializeComponent();

            resourceManager = new ResourceManager("TFLC_sem6_lab1.Resources",
                                              typeof(MainForm).Assembly);
            processFile = new ProcessFile();
            OutputTextBox.Enabled = false;
            InputTextBox.Enabled = false;
            InputTextBox.TextChanged += InputTextBox_IsChanged;
            CreateLineNumberedRichTextBox();

            userHelpPath = Path.Combine(Directory.GetCurrentDirectory(), userPath);
            aboutPath = Path.Combine(Directory.GetCurrentDirectory(), abPath);

            fontDialog1 = new FontDialog();

            SetEvent();

            statusStripHandler.InitializeStatusStrip(statusLabel, cursorPositionLabel,
                fileInfoLabel, statusStrip1);
            AttachEvents();

            foreach (ToolStripMenuItem item in InstrumentMenu.Items)
            {
                item.MouseEnter += MenuItem_MouseEnter;
                item.MouseLeave += MenuItem_MouseLeave;
            }

            foreach (ToolStripMenuItem menuItem in MainMenu.Items)
            {
                hotKeys.ConfigureMenuHotkeys(menuItem);
            }

            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Length > 0)
            {
                string filePath = files[0];

                if (System.IO.Path.GetExtension(filePath).ToLower() == ".txt")
                {
                    processFile.OpenDropFile(filePath, OutputTextBox, InputTextBox, currentFilePath);
                }
                else
                {
                    OutputTextBox.LogLocalized("OpenError");
                }
            }
        }

        private void InputTextBox_IsChanged(object sender, EventArgs e)
        {
            KeyWords.HighlightKeywords(InputTextBox, keywords, keywordColor);
        }

        private void AttachEvents()
        {
            InputTextBox.SelectionChanged += UpdateCursorPosition;
            InputTextBox.KeyUp += UpdateCursorPosition;
            InputTextBox.MouseUp += UpdateCursorPosition;

            InputTextBox.TextChanged += (s, e) =>
            {
                isTextModified = true;
                statusStripHandler.UpdateFileInfo(currentFilePath, fileInfoLabel, isTextModified);
                statusStripHandler.UpdateStatus("Текст изменен", statusLabel);
            };
        }

        private void UpdateCursorPosition(object sender, EventArgs e)
        {
            int currentPosition = InputTextBox.SelectionStart;
            int line = InputTextBox.GetLineFromCharIndex(currentPosition) + 1;
            int column = currentPosition - InputTextBox.GetFirstCharIndexFromLine(line - 1) + 1;

            cursorPositionLabel.Text = $"Стр: {line}, Стлб: {column}";
        }

        private void CreateLineNumberedRichTextBox()
        {
            Point originalLocation = InputTextBox.Location;
            Size originalSize = InputTextBox.Size;

            lineNumberPanel = new Panel
            {
                Width = 40,
                BackColor = Color.LightGray,
                Location = new Point(originalLocation.X, originalLocation.Y),
                Height = originalSize.Height,
                Anchor = InputTextBox.Anchor
            };
            InputTextBox.Location = new Point(originalLocation.X + 40, originalLocation.Y);
            InputTextBox.Width = originalSize.Width - 40;
            InputTextBox.WordWrap = false;

            lineNumberPanel.Paint += LineNumberPanel_Paint;
            InputTextBox.VScroll += RichTextBox_Scroll;
            InputTextBox.TextChanged += RichTextBox_TextChanged;
            InputTextBox.FontChanged += RichTextBox_FontChanged;

            this.Controls.Add(lineNumberPanel);

            this.Controls.SetChildIndex(InputTextBox, 0);
        }

        private void LineNumberPanel_Paint(object sender, PaintEventArgs e)
        {
            drawLines.DrawLineNumbers(e.Graphics, InputTextBox);
        }

        private void RichTextBox_Scroll(object sender, EventArgs e)
        {
            lineNumberPanel.Invalidate();
        }

        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {
            lineNumberPanel.Invalidate();
        }

        private void RichTextBox_FontChanged(object sender, EventArgs e)
        {
            lineNumberPanel.Invalidate();
        }

        private void MenuItem_MouseEnter(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem?.ToolTipText != null)
            {
                toolTip.Show(menuItem.ToolTipText.ToString(), this,
                    Control.MousePosition.X - this.Location.X,
                    Control.MousePosition.Y - this.Location.Y + 20);
            }
        }

        private void MenuItem_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

        public void SetEvent()
        {
            foreach (ToolStripMenuItem item in MainMenu.Items)
            {
                if (item.Text == "Файл") { FileHandler(item); }
                else if (item.Text == "Правка") { EditionHandler(item); }
                else if (item.Text == "Справка") { HelpFormsHandler(item); }
                else if (item.Text == "Настройки") { SettingsHandler(item); }
                else if (item.Text == "Пуск") { item.Click += StartFile; }
            }

            foreach (ToolStripMenuItem item in InstrumentMenu.Items)
            {
                if (item.Name == "Создать") { item.Click += CreateFile; }
                else if (item.Name == "Открыть") { item.Click += OpenFile; }
                else if (item.Name == "Сохранить") { item.Click += SaveFile; }
                else if (item.Name == "Отменить") { item.Click += UndoText; }
                else if (item.Name == "Повторить") { item.Click += RedoText; }
                else if (item.Name == "Копировать") { item.Click += CopyText; }
                else if (item.Name == "Вырезать") { item.Click += CutText; }
                else if (item.Name == "Вставить") { item.Click += PasteText; }
                else if (item.Name == "Пуск") { item.Click += StartFile; }
                else if (item.Name == "Справка") { item.Click += ShowHelpForm; }
                else if (item.Name == "ОПрограмме") { item.Click += ShowAboutForm; }
            }
        }

        private void StartFile(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            OutputTextBox.LogLocalizedError(Application.ExecutablePath, 1, 1, "Test Error");
            OutputTextBox.LogLocalizedError(aboutPath, 2, 1, "New Test Error");
        }

        private void CreateFile(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            isOpened = true;
            InputTextBox.Enabled = true;
            InputTextBox.Text = "";
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            isOpened = true;
            currentFilePath = processFile.OpenTxtFile(InputTextBox, OutputTextBox, currentFilePath);
            InputTextBox.Enabled = true;
            fileText = InputTextBox.Text;
        }

        private void SaveFile(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            processFile.SaveTxtFile(InputTextBox, OutputTextBox, currentFilePath, isOpened);
            fileText = InputTextBox.Text;
        }

        private void SaveAsFile(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            processFile.SaveTxtFileAs(InputTextBox, OutputTextBox, currentFilePath, isOpened);
            fileText = InputTextBox.Text;
        }

        private void ExitFromFile(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            if (fileText != InputTextBox.Text)
            {
                OutputTextBox.LogLocalized("SaveBeforeExit");
                return;
            }
            processFile.ExitFile(InputTextBox);
        }

        private void ExitFromProgram(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            if (fileText != InputTextBox.Text)
            {
                OutputTextBox.LogLocalized("SaveBeforeExit");
                return;
            }
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
        }

        private void UndoText(object sender, EventArgs e)
        {
            if (isOpened && InputTextBox.CanUndo)
            {
                InputTextBox.Undo();
            }
        }

        private void RedoText(object sender, EventArgs e)
        {
            if (InputTextBox.CanRedo)
            {
                InputTextBox.Redo();
            }
        }

        private void CutText(object sender, EventArgs e)
        {
            if (InputTextBox.SelectedText.Length > 0)
            {
                InputTextBox.Cut();
            }
        }

        private void CopyText(object sender, EventArgs e)
        {
            if (InputTextBox.SelectedText.Length > 0)
            {
                InputTextBox.Copy();
            }
        }

        private void PasteText(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                InputTextBox.Paste();
            }
        }

        private void DeleteText(object sender, EventArgs e)
        {
            if (InputTextBox.SelectedText.Length > 0)
            {
                InputTextBox.SelectedText = "";
            }
        }

        private void SelectAllText(object sender, EventArgs e)
        {
            InputTextBox.SelectAll();
        }

        private void ShowHelpForm(object sender, EventArgs e)
        {
            using (var helper = new HelpForm(userHelpPath))
            {
                helper.ShowDialog();
            }
        }

        private void ShowAboutForm(object sender, EventArgs e)
        {
            using (var helper = new HelpForm(aboutPath))
            {
                helper.ShowDialog();
            }
        }

        private void ChangeTextStyle(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                ApplyFontToAllControls(this, fontDialog1.Font);
            }
        }

        private void ApplyFontToAllControls(Control parent, Font newFont)
        {
            foreach (Control control in parent.Controls)
            {
                control.Font = newFont;

                if (control.HasChildren)
                {
                    ApplyFontToAllControls(control, newFont);
                }
            }
        }

        private void SetLocalizeEng(object sender, EventArgs e)
        {
            langHandler.ChangeLanguage("en", MainMenu, InstrumentMenu, resourceManager);
            MainMenu.Update();
            InstrumentMenu.Update();
            this.Invalidate(true);
            RichTextBoxExtensions.RefreshResources();
            currLang = "en";

            statusLabel.Text = "Ready to work";
            cursorPositionLabel.Text = "Line: 1, Column: 1";
            fileInfoLabel.Text = "New file";
        }

        private void SetLocalizeRu(object sender, EventArgs e)
        {
            langHandler.ChangeLanguage("ru-RU", MainMenu, InstrumentMenu, resourceManager);
            MainMenu.Update();
            InstrumentMenu.Update();
            this.Invalidate(true);
            RichTextBoxExtensions.RefreshResources();
            currLang = "ru";

            statusLabel.Text = "Готов к работе";
            cursorPositionLabel.Text = "Стр: 1, Стлб: 1";
            fileInfoLabel.Text = "Новый файл";

        }

        private void FileHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem createItem = new ToolStripMenuItem();
            createItem.Text = "Создать";
            createItem.Click += CreateFile;
            createItem.Tag = "CreateFile";
            item.DropDownItems.Add(createItem);

            ToolStripMenuItem openItem = new ToolStripMenuItem();
            openItem.Text = "Открыть";
            openItem.Click += OpenFile;
            openItem.Tag = "OpenFile";
            item.DropDownItems.Add(openItem);

            ToolStripMenuItem saveItem = new ToolStripMenuItem();
            saveItem.Text = "Сохранить";
            saveItem.Click += SaveFile;
            saveItem.Tag = "SaveFile";
            item.DropDownItems.Add(saveItem);

            ToolStripMenuItem saveAsItem = new ToolStripMenuItem();
            saveAsItem.Text = "Сохранить как";
            saveAsItem.Click += SaveAsFile;
            saveAsItem.Tag = "SaveAsFile";
            item.DropDownItems.Add(saveAsItem);

            ToolStripMenuItem exitFileItem = new ToolStripMenuItem();
            exitFileItem.Text = "Закрыть файл";
            exitFileItem.Click += ExitFromFile;
            exitFileItem.Tag = "CloseFile";
            item.DropDownItems.Add(exitFileItem);

            ToolStripMenuItem exitItem = new ToolStripMenuItem();
            exitItem.Text = "Выход";
            exitItem.Click += ExitFromProgram;
            exitItem.Tag = "ExitFile";
            item.DropDownItems.Add(exitItem);
        }

        private void EditionHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem undoItem = new ToolStripMenuItem();
            undoItem.Text = "Отменить";
            undoItem.Click += UndoText;
            undoItem.Tag = "UndoText";
            item.DropDownItems.Add(undoItem);

            ToolStripMenuItem redoItem = new ToolStripMenuItem();
            redoItem.Text = "Повторить";
            redoItem.Click += RedoText;
            redoItem.Tag = "RedoText";
            item.DropDownItems.Add(redoItem);

            ToolStripMenuItem cutItem = new ToolStripMenuItem();
            cutItem.Text = "Вырезать";
            cutItem.Click += CutText;
            cutItem.Tag = "CutText";
            item.DropDownItems.Add(cutItem);

            ToolStripMenuItem copyItem = new ToolStripMenuItem();
            copyItem.Text = "Копировать";
            copyItem.Click += CopyText;
            copyItem.Tag = "CopyText";
            item.DropDownItems.Add(copyItem);

            ToolStripMenuItem pasteItem = new ToolStripMenuItem();
            pasteItem.Text = "Вставить";
            pasteItem.Click += PasteText;
            pasteItem.Tag = "PasteText";
            item.DropDownItems.Add(pasteItem);

            ToolStripMenuItem deleteItem = new ToolStripMenuItem();
            deleteItem.Text = "Удалить";
            deleteItem.Click += DeleteText;
            deleteItem.Tag = "DeleteText";
            item.DropDownItems.Add(deleteItem);

            ToolStripMenuItem selectItem = new ToolStripMenuItem();
            selectItem.Text = "Выделить все";
            selectItem.Click += SelectAllText;
            selectItem.Tag = "SelectAllText";
            item.DropDownItems.Add(selectItem);
        }

        private void HelpFormsHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem helpItem = new ToolStripMenuItem();
            helpItem.Text = "Вызов справки";
            helpItem.Click += ShowHelpForm;
            helpItem.Tag = "ShowHelp";
            item.DropDownItems.Add(helpItem);

            ToolStripMenuItem aboutItem = new ToolStripMenuItem();
            aboutItem.Text = "О программе";
            aboutItem.Click += ShowAboutForm;
            aboutItem.Tag = "ShowAbout";
            item.DropDownItems.Add(aboutItem);
        }

        private void SettingsHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem textItem = new ToolStripMenuItem();
            textItem.Text = "Настройки шрифта";
            textItem.Click += ChangeTextStyle;
            textItem.Tag = "ChangeTextStyle";
            item.DropDownItems.Add(textItem);

            ToolStripMenuItem langItem = new ToolStripMenuItem();
            langItem.Text = "Сменить язык";
            langItem.Tag = "ChangeLang";

            ToolStripMenuItem engItem = new ToolStripMenuItem();
            engItem.Text = "Английский";
            engItem.Click += SetLocalizeEng;
            engItem.Tag = "SetLocalizeEng";
            langItem.DropDownItems.Add(engItem);

            ToolStripMenuItem ruItem = new ToolStripMenuItem();
            ruItem.Text = "Русский";
            ruItem.Click += SetLocalizeRu;
            ruItem.Tag = "SetLocalizeRu";
            langItem.DropDownItems.Add(ruItem);

            item.DropDownItems.Add(langItem);

        }
    }
}
