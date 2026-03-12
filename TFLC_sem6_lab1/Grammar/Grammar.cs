using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.Grammar
{
    public class GrammarHandle
    {
        [DllImport("Grammar.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ParseString(string input);

        [DllImport("Grammar.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetLastParseError();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]

        private static extern bool FreeLibrary(IntPtr hModule);
        private IntPtr dllHandle;

        public void LoadDll()
        {
            string dllPath = Path.Combine(Application.StartupPath, "Grammar.dll");

            if (!File.Exists(dllPath))
            {
                MessageBox.Show($"DLL не найдена: {dllPath}\n\n");
                return;
            }

            try
            {
                dllHandle = LoadLibrary(dllPath);
                if (dllHandle == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Ошибка в загрузке DLL. Код ошибки: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public string ConvertTxbToStr(RichTextBox txb)
        {
            string[] lines = txb.Lines;
            var sb = new System.Text.StringBuilder();

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    sb.Append(line + " ");
                }
            }

            string result = sb.ToString().Trim();
            return System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
        }

        public void ParseProgram(RichTextBox inputTxb, TextBox txtOutput)
        {
            string input = ConvertTxbToStr(inputTxb);
            if (string.IsNullOrEmpty(input))
            {
                txtOutput.Text = "Введите код для проверки грамматики";
                return;
            }

            txtOutput.Clear();
            Application.DoEvents();

            try
            {
                DateTime start = DateTime.Now;
                int result = ParseString(input);
                string error = "";
                try
                {
                    IntPtr errorPtr = GetLastParseError();
                    if (errorPtr != IntPtr.Zero)
                    {
                        error = Marshal.PtrToStringAnsi(errorPtr) ?? "";
                    }
                }
                catch {  }

                TimeSpan elapsed = DateTime.Now - start;
                StringBuilder output = new StringBuilder();
                output.AppendLine($"Время: {elapsed.TotalMilliseconds:F2} ms");
                output.AppendLine();

                if (result == 0)
                {
                    output.AppendLine("Успех!");
                    output.AppendLine("Код синтаксически верный");
                }
                else
                {
                    output.AppendLine("Ошибка!");
                    if (!string.IsNullOrEmpty(error))
                    {
                        output.AppendLine($"Ошибка: {error}");
                    }
                    else
                    {
                        output.AppendLine("Неизвестная ошибка");
                    }
                }

                txtOutput.Text = output.ToString();
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"Исключение: {ex.Message}";
            }
        }
    }
}
