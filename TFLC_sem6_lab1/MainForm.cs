using System.Text;
using System.Windows.Forms;
using TFLC_sem6_lab1.ButtonHandlers;
using TFLC_sem6_lab1.HelpForms;
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
        public MainForm()
        {
            InitializeComponent();
            processFile = new ProcessFile();
            OutputTextBox.Enabled = false;
            InputTextBox.Enabled = false;

            userHelpPath = Path.Combine(Directory.GetCurrentDirectory(), userPath);
            aboutPath = Path.Combine(Directory.GetCurrentDirectory(), abPath);

            SetEvent();

            foreach (ToolStripMenuItem item in InstrumentMenu.Items)
            {
                item.MouseEnter += MenuItem_MouseEnter;
                item.MouseLeave += MenuItem_MouseLeave;
            }
        }

        private void MenuItem_MouseEnter(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem?.Tag != null)
            {
                toolTip.Show(menuItem.Tag.ToString(), this,
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
                else if ( item.Text == "Справка") { HelpFormsHandler(item); }
            }

            foreach (ToolStripMenuItem item in InstrumentMenu.Items)
            {
                if (item.Tag == "Создать") { item.Click += CreateFile; }
                else if (item.Tag == "Открыть") { item.Click += OpenFile; }
                else if (item.Tag == "Сохранить") { item.Click += SaveFile; }
                else if (item.Tag == "Отменить") { item.Click += UndoText; }
                else if (item.Tag == "Повторить") { item.Click += RedoText; }
                else if (item.Tag == "Копировать") { item.Click += CopyText; }
                else if (item.Tag == "Вырезать") { item.Click += CutText; }
                else if (item.Tag == "Вставить") { item.Click += PasteText; }
                //else if (item.Tag == "Пуск") { item.Click += ; }
                else if (item.Tag == "Справка") { item.Click += ShowHelpForm; }
                else if (item.Tag == "О программе") { item.Click += ShowAboutForm; }
            }
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
                OutputTextBox.Text = "Для выхода сохраните файл!";
                return;
            }
            processFile.ExitFile(InputTextBox);
        }

        private void ExitFromProgram(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
            if (fileText != InputTextBox.Text)
            {
                OutputTextBox.Text = "Для выхода сохраните файл!";
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

        private void FileHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem createItem = new ToolStripMenuItem();
            createItem.Text = "Создать";
            createItem.Click += CreateFile;
            item.DropDownItems.Add(createItem);

            ToolStripMenuItem openItem = new ToolStripMenuItem();
            openItem.Text = "Открыть";
            openItem.Click += OpenFile;
            item.DropDownItems.Add(openItem);

            ToolStripMenuItem saveItem = new ToolStripMenuItem();
            saveItem.Text = "Сохранить";
            saveItem.Click += SaveFile;
            item.DropDownItems.Add(saveItem);

            ToolStripMenuItem saveAsItem = new ToolStripMenuItem();
            saveAsItem.Text = "Сохранить как";
            saveAsItem.Click += SaveAsFile;
            item.DropDownItems.Add(saveAsItem);

            ToolStripMenuItem exitFileItem = new ToolStripMenuItem();
            exitFileItem.Text = "Закрыть файл";
            exitFileItem.Click += ExitFromFile;
            item.DropDownItems.Add(exitFileItem);

            ToolStripMenuItem exitItem = new ToolStripMenuItem();
            exitItem.Text = "Выход";
            exitItem.Click += ExitFromProgram;
            item.DropDownItems.Add(exitItem);
        }

        private void EditionHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem undoItem = new ToolStripMenuItem();
            undoItem.Text = "Отменить";
            undoItem.Click += UndoText;
            item.DropDownItems.Add(undoItem);

            ToolStripMenuItem redoItem = new ToolStripMenuItem();
            redoItem.Text = "Повторить";
            redoItem.Click += RedoText;
            item.DropDownItems.Add(redoItem);

            ToolStripMenuItem cutItem = new ToolStripMenuItem();
            cutItem.Text = "Вырезать";
            cutItem.Click += CutText;
            item.DropDownItems.Add(cutItem);

            ToolStripMenuItem copyItem = new ToolStripMenuItem();
            copyItem.Text = "Копировать";
            copyItem.Click += CopyText;
            item.DropDownItems.Add(copyItem);

            ToolStripMenuItem pasteItem = new ToolStripMenuItem();
            pasteItem.Text = "Вставить";
            pasteItem.Click += PasteText;
            item.DropDownItems.Add(pasteItem);

            ToolStripMenuItem deleteItem = new ToolStripMenuItem();
            deleteItem.Text = "Удалить";
            deleteItem.Click += DeleteText;
            item.DropDownItems.Add(deleteItem);

            ToolStripMenuItem selectItem = new ToolStripMenuItem();
            selectItem.Text = "Выделить все";
            selectItem.Click += SelectAllText;
            item.DropDownItems.Add(selectItem);
        }

        private void HelpFormsHandler(ToolStripMenuItem item)
        {
            ToolStripMenuItem helpItem = new ToolStripMenuItem();
            helpItem.Text = "Вызов справки";
            helpItem.Click += ShowHelpForm;
            item.DropDownItems.Add(helpItem);

            ToolStripMenuItem aboutItem = new ToolStripMenuItem();
            aboutItem.Text = "О программе";
            aboutItem.Click += ShowAboutForm;
            item.DropDownItems.Add(aboutItem);
        }
    }
}
