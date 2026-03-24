using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TFLC_sem6_lab1.Grammar
{
    public class AntlrClass
    {
        public void InitializeAntlrParser(dynamic _antlrParser)
        {
            try
            {
                _antlrParser = Program.CreateAntlrParser();

                if (_antlrParser == null)
                {
                    MessageBox.Show("Не удалось создать ANTLR парсер", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    var testResult = _antlrParser.ParseWithDetails("$x = 5;");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации ANTLR парсера: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
