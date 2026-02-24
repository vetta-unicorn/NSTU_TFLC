using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFLC_sem6_lab1.Handlers;

namespace TFLC_sem6_lab1.ButtonHandlers
{
    public class ProcessFile
    {
        public string OpenTxtFile(RichTextBox InputTextBox, RichTextBox OutputTextBox, string currentFilePath)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Выберите текстовый файл";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(openFileDialog.FileName);
                        InputTextBox.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        //OutputTextBox.Text = ex.Message;
                        OutputTextBox.LogLocalized("OpenError");
                    }
                    finally { currentFilePath = openFileDialog.FileName; }
                }
                //OutputTextBox.Text = "Файл успешно открыт!";
                OutputTextBox.LogLocalized("OpenCompleted");
            }
            return currentFilePath;
        }

        public void SaveTxtFile(RichTextBox InputTextBox, RichTextBox OutputTextBox, string currentFilePath, bool isOpened)
        {
            if (isOpened && !string.IsNullOrEmpty(currentFilePath))
            {
                try
                {
                    File.WriteAllText(currentFilePath, InputTextBox.Text);
                    //OutputTextBox.Text = "Файл успешно сохранен!";
                    OutputTextBox.LogLocalized("SaveCompleted");
                }
                catch (Exception ex)
                {
                    //OutputTextBox.Text = ex.Message;
                    OutputTextBox.LogLocalized("SaveError");
                }
            }
            else
            {
                SaveTxtFileAs(InputTextBox, OutputTextBox, currentFilePath, isOpened);
            }
        }

        public void SaveTxtFileAs(RichTextBox InputTextBox, RichTextBox OutputTextBox, string currentFilePath, bool isOpened)
        {
            if (!isOpened) { OutputTextBox.Text = "Файл не открыт!"; return; }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Сохранить текстовый файл";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string textToSave = InputTextBox.Text;

                        File.WriteAllText(saveFileDialog.FileName, textToSave);
                        //OutputTextBox.Text = "Файл успешно сохранен!";
                        OutputTextBox.LogLocalized("SaveCompleted");
                    }
                    catch (Exception ex)
                    {
                        //OutputTextBox.Text = ex.Message;
                        OutputTextBox.LogLocalized("SaveError");
                    }
                }
            }
        }

        public void ExitFile(RichTextBox InputTextBox)
        {
            InputTextBox.Text = "";
            InputTextBox.Enabled = false;
        }

        public void OpenDropFile(string filePath, RichTextBox OutputTextBox, 
            RichTextBox InputTextBox, string currentFilePath)
        {
            OutputTextBox.Text = "";
            try
            {
                string content = System.IO.File.ReadAllText(filePath, Encoding.Default);
                InputTextBox.Text = content;
                currentFilePath = filePath;
            }
            catch (Exception ex)
            {
                OutputTextBox.LogLocalized("OpenError");
            }
        }
    }
}
