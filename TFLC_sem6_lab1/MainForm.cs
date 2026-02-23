using System.Globalization;
using System.Resources;
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
        private FontDialog fontDialog1;

        Label lblSampleText;
        System.Windows.Forms.Button btnSelectFont;
        private Panel passPanelContainer;
        Font newFont;

        private ResourceManager resourceManager;
        private string currLang = "ru";
        public MainForm()
        {
            InitializeComponent();

            resourceManager = new ResourceManager("TFLC_sem6_lab1.Resources",
                                              typeof(MainForm).Assembly);
            processFile = new ProcessFile();
            OutputTextBox.Enabled = false;
            InputTextBox.Enabled = false;

            userHelpPath = Path.Combine(Directory.GetCurrentDirectory(), userPath);
            aboutPath = Path.Combine(Directory.GetCurrentDirectory(), abPath);

            fontDialog1 = new FontDialog();

            SetEvent();

            foreach (ToolStripMenuItem item in InstrumentMenu.Items)
            {
                item.MouseEnter += MenuItem_MouseEnter;
                item.MouseLeave += MenuItem_MouseLeave;
            }
        }


        private void ApplyResourcesToMenu(ToolStripMenuItem item)
        {
            if (item.Tag != null && item.Tag is string resourceKey)
            {
                item.Text = resourceManager.GetString(resourceKey);
            }
            foreach (ToolStripMenuItem childItem in item.DropDownItems)
            {
                ApplyResourcesToMenu(childItem);
            }
        }

        private void ApplyResourcesToMenu()
        {
            foreach (ToolStripMenuItem topLevelItem in MainMenu.Items)
            {
                ApplyResourcesToMenu(topLevelItem);
            }
        }

        private void ApplyResourcesToInstrumentMenu(ToolStripMenuItem item)
        {
            if (item.Tag != null && item.Tag is string resourceKey)
            {
                item.ToolTipText = resourceManager.GetString(resourceKey);
            }
            foreach (ToolStripMenuItem childItem in item.DropDownItems)
            {
                ApplyResourcesToInstrumentMenu(childItem);
            }
        }

        private void ApplyResourcesToInstrumentMenu()
        {
            foreach (ToolStripMenuItem topLevelItem in InstrumentMenu.Items)
            {
                ApplyResourcesToInstrumentMenu(topLevelItem);
            }
        }

        private void ChangeLanguage(string cultureName)
        {
            CultureInfo newCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;

            ApplyResourcesToMenu();
            ApplyResourcesToInstrumentMenu();
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
                else if ( item.Text == "Справка") { HelpFormsHandler(item); }
                else if (item.Text == "Настройки") { SettingsHandler(item); }
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
                //else if (item.Tag == "Пуск") { item.Click += ; }
                else if (item.Name == "Справка") { item.Click += ShowHelpForm; }
                else if (item.Name == "О программе") { item.Click += ShowAboutForm; }
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
            ChangeLanguage("en");
            MainMenu.Update();
            InstrumentMenu.Update();
            this.Invalidate(true);
            currLang = "en";
        }

        private void SetLocalizeRu(object sender, EventArgs e)
        {
            ChangeLanguage("ru-RU");
            MainMenu.Update();
            InstrumentMenu.Update();
            this.Invalidate(true);
            currLang = "ru";
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
