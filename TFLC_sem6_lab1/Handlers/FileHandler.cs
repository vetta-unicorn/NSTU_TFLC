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
                        OutputTextBox.LogLocalized("OpenError");
                    }
                    finally { currentFilePath = openFileDialog.FileName; }
                }
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
                    OutputTextBox.LogLocalized("SaveCompleted");
                }
                catch (Exception ex)
                {
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
                        OutputTextBox.LogLocalized("SaveCompleted");
                    }
                    catch (Exception ex)
                    {
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

        public string LoadEmbeddedResource(string resourceName)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        string availableResources = string.Join("\n",
                            assembly.GetManifestResourceNames());

                        return $@"<html><body>
                    <h1>Ресурс не найден</h1>
                    <p>Искали: {resourceName}</p>
                    <p>Доступные ресурсы:</p>
                    <pre>{availableResources}</pre>
                </body></html>";
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return $@"<html><body>
            <h1>Ошибка загрузки</h1>
            <p>{ex.Message}</p>
        </body></html>";
            }
        }
    }
}
