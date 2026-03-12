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
                MessageBox.Show($"DLL not found at: {dllPath}\n\n");
                return;
            }

            try
            {
                // Просто проверяем что DLL можно загрузить
                dllHandle = LoadLibrary(dllPath);
                if (dllHandle == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Failed to load DLL. Error code: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
                txtOutput.Text = "Please enter some code to parse.";
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
                output.AppendLine($"Time: {elapsed.TotalMilliseconds:F2} ms");
                output.AppendLine();

                if (result == 0)
                {
                    output.AppendLine("SUCCESS");
                    output.AppendLine("The code is syntactically correct.");
                }
                else
                {
                    output.AppendLine("FAILED");
                    if (!string.IsNullOrEmpty(error))
                    {
                        output.AppendLine($"Error: {error}");
                    }
                    else
                    {
                        output.AppendLine("Unknown error");
                    }
                }

                //output.AppendLine();
                //output.AppendLine("=== INPUT ===");
                //output.AppendLine(input);

                txtOutput.Text = output.ToString();
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"Exception: {ex.Message}";
            }
        }
    }
}
