using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        OutputTextBox.Text = ex.Message;
                    }
                    finally { currentFilePath = openFileDialog.FileName; }
                }
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
                    OutputTextBox.Text = "Файл успешно сохранен!";
                }
                catch (Exception ex)
                {
                    OutputTextBox.Text = ex.Message;
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
                        OutputTextBox.Text = "Файл успешно сохранен!";
                    }
                    catch (Exception ex)
                    {
                        OutputTextBox.Text = ex.Message;
                    }
                }
            }
        }

        public void ExitFile(RichTextBox InputTextBox)
        {
            InputTextBox.Text = "";
            InputTextBox.Enabled = false;
        }
    }
}
