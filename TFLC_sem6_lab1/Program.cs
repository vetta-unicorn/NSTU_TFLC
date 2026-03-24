using System.Reflection;

namespace TFLC_sem6_lab1
{
    internal static class Program
    {
        private static string _dllPath; 
        private static string _antlrDllPath;      
        private static Assembly _antlrAssembly;   

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ExtractDllFromResources();
            ExtractAntlrDllFromResources();

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static void ExtractDllFromResources()
        {
            try
            {
                string architecture = Environment.Is64BitOperatingSystem ? "64" : "32";

                string resourceName = $"TFLC_sem6_lab1.Grammar.FlexBisonGrammar_{architecture}.dll";

                var executingAssembly = Assembly.GetExecutingAssembly();

                string[] availableResources = executingAssembly.GetManifestResourceNames();

                if (!availableResources.Contains(resourceName))
                {
                    resourceName = "TFLC_sem6_lab1.Grammar.FlexBisonGrammar.dll";
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
                    _dllPath = Path.Combine(tempPath, "FlexBisonGrammar.dll");

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
                MessageBox.Show($"Ошибка при извлечении FlexBisonGrammar DLL: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ExtractAntlrDllFromResources()
        {
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();

                string[] availableResources = executingAssembly.GetManifestResourceNames();
                string antlrResourceName = null;

                foreach (var resource in availableResources)
                {
                    if (resource.Contains("ANTLR_DLL") && resource.EndsWith(".dll"))
                    {
                        antlrResourceName = resource;
                        break;
                    }
                }

                if (antlrResourceName == null)
                {
                    string resourceList = string.Join("\n", availableResources);
                    MessageBox.Show(
                        $"Ресурс ANTLR_DLL.dll не найден!\n\n" +
                        $"Доступные ресурсы:\n{resourceList}",
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                using (Stream stream = executingAssembly.GetManifestResourceStream(antlrResourceName))
                {
                    if (stream == null)
                    {
                        MessageBox.Show(
                            $"Не удалось загрузить ресурс '{antlrResourceName}'",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);

                    try
                    {
                        _antlrAssembly = Assembly.Load(assemblyData);
                        _antlrDllPath = null; 

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Не удалось загрузить ANTLR DLL из памяти: {ex.Message}\n" +
                            $"Пробуем сохранить на диск...",
                            "Информация",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        string tempPath = Path.GetTempPath();
                        _antlrDllPath = Path.Combine(tempPath, $"ANTLR_DLL_{Guid.NewGuid()}.dll");

                        stream.Seek(0, SeekOrigin.Begin);

                        using (FileStream fileStream = new FileStream(_antlrDllPath, FileMode.Create, FileAccess.Write))
                        {
                            stream.CopyTo(fileStream);
                        }

                        _antlrAssembly = Assembly.LoadFrom(_antlrDllPath);
                    }
                }

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при извлечении ANTLR DLL: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("ANTLR_DLL"))
            {
                return _antlrAssembly;
            }

            if (args.Name.Contains("Antlr4.Runtime"))
            {
                try
                {
                    var executingAssembly = Assembly.GetExecutingAssembly();
                    string resourceName = "TFLC_sem6_lab1.Antlr4.Runtime.Standard.dll";

                    using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            byte[] assemblyData = new byte[stream.Length];
                            stream.Read(assemblyData, 0, assemblyData.Length);
                            return Assembly.Load(assemblyData);
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        public static Assembly AntlrAssembly => _antlrAssembly;

        public static Type GetAntlrParserType()
        {
            if (_antlrAssembly == null)
                return null;

            foreach (Type type in _antlrAssembly.GetTypes())
            {
                if (type.Name == "DoWhileParser" || type.Name.Contains("DoWhileParser"))
                {
                    return type;
                }
            }

            return null;
        }

        public static dynamic CreateAntlrParser()
        {
            Type parserType = GetAntlrParserType();
            if (parserType == null)
                return null;

            return Activator.CreateInstance(parserType);
        }

        public static string DllPath => _dllPath;

        public static string AntlrDllPath => _antlrDllPath;

        public static void CleanupTempFiles()
        {
            try
            {
                if (_antlrDllPath != null && File.Exists(_antlrDllPath))
                {
                    File.Delete(_antlrDllPath);
                }

                if (_dllPath != null && File.Exists(_dllPath))
                {
                    File.Delete(_dllPath);
                }
            }
            catch
            {
            }
        }
    }
}


