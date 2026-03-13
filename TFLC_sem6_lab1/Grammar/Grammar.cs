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
        // Импортируем функции из kernel32 для загрузки библиотеки
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // Делегаты для функций из нашей DLL
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int ParseStringDelegate(string input);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetLastParseErrorDelegate();

        private IntPtr _dllHandle;
        private ParseStringDelegate _parseString;
        private GetLastParseErrorDelegate _getLastParseError;

        public GrammarHandle()
        {
            LoadDllFromPath();
        }

        private void LoadDllFromPath()
        {
            string dllPath = Program.DllPath;

            if (string.IsNullOrEmpty(dllPath) || !System.IO.File.Exists(dllPath))
            {
                throw new Exception("DLL не найдена. Убедитесь, что Program.ExtractDllFromResources() был вызван.");
            }

            // Загружаем DLL
            _dllHandle = LoadLibrary(dllPath);
            if (_dllHandle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Exception($"Не удалось загрузить DLL. Код ошибки: {error}");
            }

            // Получаем адреса функций
            IntPtr parseStringAddr = GetProcAddress(_dllHandle, "ParseString");
            IntPtr getLastErrorAddr = GetProcAddress(_dllHandle, "GetLastParseError");

            if (parseStringAddr == IntPtr.Zero || getLastErrorAddr == IntPtr.Zero)
            {
                FreeLibrary(_dllHandle);
                throw new Exception("Не удалось найти функции в DLL");
            }

            // Создаем делегаты
            _parseString = (ParseStringDelegate)Marshal.GetDelegateForFunctionPointer(
                parseStringAddr, typeof(ParseStringDelegate));

            _getLastParseError = (GetLastParseErrorDelegate)Marshal.GetDelegateForFunctionPointer(
                getLastErrorAddr, typeof(GetLastParseErrorDelegate));
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

                // Используем делегаты для вызова функций
                int result = _parseString(input);

                string error = "";
                try
                {
                    IntPtr errorPtr = _getLastParseError();
                    if (errorPtr != IntPtr.Zero)
                    {
                        error = Marshal.PtrToStringAnsi(errorPtr) ?? "";
                    }
                }
                catch { }

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

        // Освобождаем ресурсы
        public void Dispose()
        {
            if (_dllHandle != IntPtr.Zero)
            {
                FreeLibrary(_dllHandle);
                _dllHandle = IntPtr.Zero;
            }
        }

        //public void ParseProgram(RichTextBox inputTxb, TextBox txtOutput)
        //{
        //    string input = ConvertTxbToStr(inputTxb);
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        txtOutput.Text = "Введите код для проверки грамматики";
        //        return;
        //    }

        //    txtOutput.Clear();
        //    Application.DoEvents();

        //    try
        //    {
        //        DateTime start = DateTime.Now;
        //        int result = ParseString(input);
        //        string error = "";
        //        try
        //        {
        //            IntPtr errorPtr = GetLastParseError();
        //            if (errorPtr != IntPtr.Zero)
        //            {
        //                error = Marshal.PtrToStringAnsi(errorPtr) ?? "";
        //            }
        //        }
        //        catch {  }

        //        TimeSpan elapsed = DateTime.Now - start;
        //        StringBuilder output = new StringBuilder();
        //        output.AppendLine($"Время: {elapsed.TotalMilliseconds:F2} ms");
        //        output.AppendLine();

        //        if (result == 0)
        //        {
        //            output.AppendLine("Успех!");
        //            output.AppendLine("Код синтаксически верный");
        //        }
        //        else
        //        {
        //            output.AppendLine("Ошибка!");
        //            if (!string.IsNullOrEmpty(error))
        //            {
        //                output.AppendLine($"Ошибка: {error}");
        //            }
        //            else
        //            {
        //                output.AppendLine("Неизвестная ошибка");
        //            }
        //        }

        //        txtOutput.Text = output.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        txtOutput.Text = $"Исключение: {ex.Message}";
        //    }
        //}
    }
}
