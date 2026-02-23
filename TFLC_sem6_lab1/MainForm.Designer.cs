namespace TFLC_sem6_lab1
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            InstrumentMenu = new MenuStrip();
            Создать = new ToolStripMenuItem();
            Открыть = new ToolStripMenuItem();
            Сохранить = new ToolStripMenuItem();
            Отменить = new ToolStripMenuItem();
            Повторить = new ToolStripMenuItem();
            Копировать = new ToolStripMenuItem();
            Вырезать = new ToolStripMenuItem();
            Вставить = new ToolStripMenuItem();
            Пуск = new ToolStripMenuItem();
            Справка = new ToolStripMenuItem();
            ОПрограмме = new ToolStripMenuItem();
            InputTextBox = new RichTextBox();
            OutputTextBox = new RichTextBox();
            MainMenu = new MenuStrip();
            MenuFile = new ToolStripMenuItem();
            MenuChange = new ToolStripMenuItem();
            MenuText = new ToolStripMenuItem();
            копироватьToolStripMenuItem1 = new ToolStripMenuItem();
            вырезатьToolStripMenuItem1 = new ToolStripMenuItem();
            вставитьToolStripMenuItem1 = new ToolStripMenuItem();
            методАнализаToolStripMenuItem = new ToolStripMenuItem();
            тестовыйПримерToolStripMenuItem = new ToolStripMenuItem();
            списокЛитературыToolStripMenuItem = new ToolStripMenuItem();
            исходныйКодПрограммыToolStripMenuItem = new ToolStripMenuItem();
            MenuStart = new ToolStripMenuItem();
            MenuHelp = new ToolStripMenuItem();
            MenuSettings = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            InstrumentMenu.SuspendLayout();
            MainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // InstrumentMenu
            // 
            InstrumentMenu.ImageScalingSize = new Size(20, 20);
            InstrumentMenu.Items.AddRange(new ToolStripItem[] { Создать, Открыть, Сохранить, Отменить, Повторить, Копировать, Вырезать, Вставить, Пуск, Справка, ОПрограмме });
            InstrumentMenu.Location = new Point(0, 28);
            InstrumentMenu.Name = "InstrumentMenu";
            InstrumentMenu.Size = new Size(936, 72);
            InstrumentMenu.Stretch = false;
            InstrumentMenu.TabIndex = 0;
            InstrumentMenu.Text = "menuStrip1";
            // 
            // Создать
            // 
            Создать.AutoSize = false;
            Создать.Image = (Image)resources.GetObject("Создать.Image");
            Создать.ImageScaling = ToolStripItemImageScaling.None;
            Создать.Name = "Создать";
            Создать.Size = new Size(64, 64);
            Создать.Tag = "CreateFile";
            Создать.ToolTipText = "Создать";
            // 
            // Открыть
            // 
            Открыть.AutoSize = false;
            Открыть.Image = (Image)resources.GetObject("Открыть.Image");
            Открыть.ImageScaling = ToolStripItemImageScaling.None;
            Открыть.Name = "Открыть";
            Открыть.Size = new Size(64, 64);
            Открыть.Tag = "OpenFile";
            Открыть.ToolTipText = "Открыть";
            // 
            // Сохранить
            // 
            Сохранить.AutoSize = false;
            Сохранить.Image = (Image)resources.GetObject("Сохранить.Image");
            Сохранить.ImageScaling = ToolStripItemImageScaling.None;
            Сохранить.Name = "Сохранить";
            Сохранить.Size = new Size(64, 64);
            Сохранить.Tag = "SaveFile";
            Сохранить.ToolTipText = "Сохранить";
            // 
            // Отменить
            // 
            Отменить.AutoSize = false;
            Отменить.Image = (Image)resources.GetObject("Отменить.Image");
            Отменить.ImageScaling = ToolStripItemImageScaling.None;
            Отменить.Name = "Отменить";
            Отменить.Size = new Size(64, 64);
            Отменить.Tag = "UndoText";
            Отменить.ToolTipText = "Отмена";
            // 
            // Повторить
            // 
            Повторить.AutoSize = false;
            Повторить.Image = (Image)resources.GetObject("Повторить.Image");
            Повторить.ImageScaling = ToolStripItemImageScaling.None;
            Повторить.Name = "Повторить";
            Повторить.Size = new Size(64, 64);
            Повторить.Tag = "RedoText";
            Повторить.ToolTipText = "Повторить";
            // 
            // Копировать
            // 
            Копировать.AutoSize = false;
            Копировать.Image = (Image)resources.GetObject("Копировать.Image");
            Копировать.ImageScaling = ToolStripItemImageScaling.None;
            Копировать.Name = "Копировать";
            Копировать.Size = new Size(64, 64);
            Копировать.Tag = "CopyText";
            Копировать.ToolTipText = "Копировать";
            // 
            // Вырезать
            // 
            Вырезать.AutoSize = false;
            Вырезать.Image = (Image)resources.GetObject("Вырезать.Image");
            Вырезать.ImageScaling = ToolStripItemImageScaling.None;
            Вырезать.Name = "Вырезать";
            Вырезать.Size = new Size(109, 64);
            Вырезать.Tag = "CutText";
            Вырезать.ToolTipText = "Вырезать";
            // 
            // Вставить
            // 
            Вставить.Image = (Image)resources.GetObject("Вставить.Image");
            Вставить.ImageScaling = ToolStripItemImageScaling.None;
            Вставить.Name = "Вставить";
            Вставить.Size = new Size(78, 68);
            Вставить.Tag = "PasteText";
            Вставить.ToolTipText = "Вставить";
            // 
            // Пуск
            // 
            Пуск.AutoSize = false;
            Пуск.Image = (Image)resources.GetObject("Пуск.Image");
            Пуск.ImageScaling = ToolStripItemImageScaling.None;
            Пуск.Name = "Пуск";
            Пуск.Size = new Size(64, 64);
            Пуск.Tag = "MenuStart";
            Пуск.ToolTipText = "Пуск";
            // 
            // Справка
            // 
            Справка.AutoSize = false;
            Справка.Image = (Image)resources.GetObject("Справка.Image");
            Справка.ImageScaling = ToolStripItemImageScaling.None;
            Справка.Name = "Справка";
            Справка.Size = new Size(64, 64);
            Справка.Tag = "ShowHelp";
            Справка.ToolTipText = "Справка";
            // 
            // ОПрограмме
            // 
            ОПрограмме.AutoSize = false;
            ОПрограмме.Image = (Image)resources.GetObject("ОПрограмме.Image");
            ОПрограмме.ImageScaling = ToolStripItemImageScaling.None;
            ОПрограмме.Name = "ОПрограмме";
            ОПрограмме.Size = new Size(64, 64);
            ОПрограмме.Tag = "ShowAbout";
            ОПрограмме.ToolTipText = "О программе";
            // 
            // InputTextBox
            // 
            InputTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            InputTextBox.Location = new Point(12, 111);
            InputTextBox.Name = "InputTextBox";
            InputTextBox.Size = new Size(863, 128);
            InputTextBox.TabIndex = 2;
            InputTextBox.Text = "";
            // 
            // OutputTextBox
            // 
            OutputTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            OutputTextBox.Location = new Point(12, 245);
            OutputTextBox.Name = "OutputTextBox";
            OutputTextBox.Size = new Size(863, 183);
            OutputTextBox.TabIndex = 3;
            OutputTextBox.Text = "";
            // 
            // MainMenu
            // 
            MainMenu.ImageScalingSize = new Size(20, 20);
            MainMenu.Items.AddRange(new ToolStripItem[] { MenuFile, MenuChange, MenuText, MenuStart, MenuHelp, MenuSettings });
            MainMenu.Location = new Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Size = new Size(936, 28);
            MainMenu.TabIndex = 4;
            MainMenu.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            MenuFile.Name = "MenuFile";
            MenuFile.Size = new Size(59, 24);
            MenuFile.Tag = "MenuFile";
            MenuFile.Text = "Файл";
            // 
            // MenuChange
            // 
            MenuChange.Name = "MenuChange";
            MenuChange.Size = new Size(74, 24);
            MenuChange.Tag = "MenuChange";
            MenuChange.Text = "Правка";
            // 
            // MenuText
            // 
            MenuText.DropDownItems.AddRange(new ToolStripItem[] { копироватьToolStripMenuItem1, вырезатьToolStripMenuItem1, вставитьToolStripMenuItem1, методАнализаToolStripMenuItem, тестовыйПримерToolStripMenuItem, списокЛитературыToolStripMenuItem, исходныйКодПрограммыToolStripMenuItem });
            MenuText.Name = "MenuText";
            MenuText.Size = new Size(59, 24);
            MenuText.Tag = "MenuText";
            MenuText.Text = "Текст";
            // 
            // копироватьToolStripMenuItem1
            // 
            копироватьToolStripMenuItem1.Name = "копироватьToolStripMenuItem1";
            копироватьToolStripMenuItem1.Size = new Size(288, 26);
            копироватьToolStripMenuItem1.Text = "Постановка задачи";
            // 
            // вырезатьToolStripMenuItem1
            // 
            вырезатьToolStripMenuItem1.Name = "вырезатьToolStripMenuItem1";
            вырезатьToolStripMenuItem1.Size = new Size(288, 26);
            вырезатьToolStripMenuItem1.Text = "Грамматика";
            // 
            // вставитьToolStripMenuItem1
            // 
            вставитьToolStripMenuItem1.Name = "вставитьToolStripMenuItem1";
            вставитьToolStripMenuItem1.Size = new Size(288, 26);
            вставитьToolStripMenuItem1.Text = "Классификация грамматики";
            // 
            // методАнализаToolStripMenuItem
            // 
            методАнализаToolStripMenuItem.Name = "методАнализаToolStripMenuItem";
            методАнализаToolStripMenuItem.Size = new Size(288, 26);
            методАнализаToolStripMenuItem.Text = "Метод анализа";
            // 
            // тестовыйПримерToolStripMenuItem
            // 
            тестовыйПримерToolStripMenuItem.Name = "тестовыйПримерToolStripMenuItem";
            тестовыйПримерToolStripMenuItem.Size = new Size(288, 26);
            тестовыйПримерToolStripMenuItem.Text = "Тестовый пример";
            // 
            // списокЛитературыToolStripMenuItem
            // 
            списокЛитературыToolStripMenuItem.Name = "списокЛитературыToolStripMenuItem";
            списокЛитературыToolStripMenuItem.Size = new Size(288, 26);
            списокЛитературыToolStripMenuItem.Text = "Список литературы";
            // 
            // исходныйКодПрограммыToolStripMenuItem
            // 
            исходныйКодПрограммыToolStripMenuItem.Name = "исходныйКодПрограммыToolStripMenuItem";
            исходныйКодПрограммыToolStripMenuItem.Size = new Size(288, 26);
            исходныйКодПрограммыToolStripMenuItem.Text = "Исходный код программы";
            // 
            // MenuStart
            // 
            MenuStart.Name = "MenuStart";
            MenuStart.Size = new Size(55, 24);
            MenuStart.Tag = "MenuStart";
            MenuStart.Text = "Пуск";
            // 
            // MenuHelp
            // 
            MenuHelp.Name = "MenuHelp";
            MenuHelp.Size = new Size(81, 24);
            MenuHelp.Tag = "MenuHelp";
            MenuHelp.Text = "Справка";
            // 
            // MenuSettings
            // 
            MenuSettings.Name = "MenuSettings";
            MenuSettings.Size = new Size(98, 24);
            MenuSettings.Tag = "MenuSettings";
            MenuSettings.Text = "Настройки";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Location = new Point(0, 426);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(936, 24);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(936, 450);
            Controls.Add(statusStrip1);
            Controls.Add(OutputTextBox);
            Controls.Add(InputTextBox);
            Controls.Add(InstrumentMenu);
            Controls.Add(MainMenu);
            MainMenuStrip = InstrumentMenu;
            Name = "MainForm";
            Text = "Compiler";
            InstrumentMenu.ResumeLayout(false);
            InstrumentMenu.PerformLayout();
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip InstrumentMenu;
        private RichTextBox InputTextBox;
        private RichTextBox OutputTextBox;
        private MenuStrip MainMenu;
        private ToolStripMenuItem Создать;
        private ToolStripMenuItem Открыть;
        private ToolStripMenuItem Сохранить;
        private ToolStripMenuItem Отменить;
        private ToolStripMenuItem Повторить;
        private ToolStripMenuItem Копировать;
        private ToolStripMenuItem Вырезать;
        private ToolStripMenuItem Вставить;
        private ToolStripMenuItem MenuFile;
        private ToolStripMenuItem MenuChange;
        private ToolStripMenuItem MenuText;
        private ToolStripMenuItem MenuStart;
        private ToolStripMenuItem MenuHelp;
        private ToolStripMenuItem копироватьToolStripMenuItem1;
        private ToolStripMenuItem вырезатьToolStripMenuItem1;
        private ToolStripMenuItem вставитьToolStripMenuItem1;
        private ToolStripMenuItem методАнализаToolStripMenuItem;
        private ToolStripMenuItem тестовыйПримерToolStripMenuItem;
        private ToolStripMenuItem списокЛитературыToolStripMenuItem;
        private ToolStripMenuItem исходныйКодПрограммыToolStripMenuItem;
        private ToolStripMenuItem Пуск;
        private ToolStripMenuItem Справка;
        private ToolStripMenuItem ОПрограмме;
        private ToolStripMenuItem MenuSettings;
        private StatusStrip statusStrip1;
    }
}
