using System.Reflection;

namespace TFLC_sem6_lab1
{
    internal static class Program
    {
        private static string _dllPath;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ExtractDllFromResources();
            ApplicationConfiguration.Initialize();

            Application.Run(new MainForm());
        }
        private static void ExtractDllFromResources()
        {
            try
            {
                string architecture = Environment.Is64BitOperatingSystem ? "64" : "32";

                string resourceName = $"TFLC_sem6_lab1.Grammar.Grammar_{architecture}.dll";

                var executingAssembly = Assembly.GetExecutingAssembly();

                string[] availableResources = executingAssembly.GetManifestResourceNames();

                if (!availableResources.Contains(resourceName))
                {
                    resourceName = "TFLC_sem6_lab1.Grammar.Grammar.dll";
                }

                using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        string resourceList = string.Join("\n", availableResources);
                        MessageBox.Show(
                            $"Ресурс '{resourceName}' не найден!\n\n" +
                            $"Доступные ресурсы:\n{resourceList}",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    string tempPath = Path.GetTempPath();
                    _dllPath = Path.Combine(tempPath, "Grammar.dll");

                    if (File.Exists(_dllPath))
                    {
                        try { File.Delete(_dllPath); }
                        catch
                        {
                            _dllPath = Path.Combine(tempPath, $"Grammar_{Guid.NewGuid()}.dll");
                        }
                    }

                    using (FileStream fileStream = new FileStream(_dllPath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при извлечении DLL: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private static void ExtractDllFromResources()
        //{
        //    try
        //    {
        //        string resourceName = "TFLC_sem6_lab1.Grammar.Grammar.dll";
        //        var executingAssembly = Assembly.GetExecutingAssembly();

        //        using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
        //        {
        //            if (stream == null)
        //            {
        //                string[] resources = executingAssembly.GetManifestResourceNames();
        //                string resourceList = string.Join("\n", resources);

        //                MessageBox.Show(
        //                    $"Ресурс '{resourceName}' не найден!\n\n" +
        //                    $"Доступные ресурсы:\n{resourceList}",
        //                    "Ошибка",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Error);
        //                return;
        //            }

        //            // Создаем временный файл для DLL
        //            string tempPath = Path.GetTempPath();
        //            _dllPath = Path.Combine(tempPath, "Grammar.dll");

        //            // Если файл уже существует, удаляем его (на всякий случай)
        //            if (File.Exists(_dllPath))
        //            {
        //                File.Delete(_dllPath);
        //            }

        //            // Копируем DLL из ресурсов во временный файл
        //            using (FileStream fileStream = new FileStream(_dllPath, FileMode.Create, FileAccess.Write))
        //            {
        //                stream.CopyTo(fileStream);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка при извлечении DLL: {ex.Message}", "Ошибка",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        // Свойство для доступа к пути к DLL из других классов
        public static string DllPath => _dllPath;
    }
}

