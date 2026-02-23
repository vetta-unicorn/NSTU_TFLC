using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.Handlers
{
    //public static class MessageHandler
    //{
    //    public static List<string> messages;
    //    static MessageHandler()
    //    {
    //        messages = new List<string>
    //        {
    //            "SuccessOpen", "FilterString", "ChooseFile", 
    //            "SuccessSave", "FileNotOpened", "SaveTxtFile",
    //            "SaveBeforeExit"
    //        };
    //    }


    //}
    public static class RichTextBoxExtensions
    {
        private static ResourceManager _resourceManager;

        static RichTextBoxExtensions()
        {
            _resourceManager = new ResourceManager("TFLC_sem6_lab1.Resources",
                                                  typeof(MainForm).Assembly);
        }

        public static void RefreshResources()
        {
            _resourceManager = new ResourceManager("TFLC_sem6_lab1.Resources",
                                                  typeof(MainForm).Assembly);
        }

        public static void LogLocalized(this RichTextBox rtb, string messageKey)
        {
            string message = _resourceManager.GetString(messageKey);
            rtb.AppendText((message ?? messageKey) + Environment.NewLine);
        }

        public static void LogLocalized(this RichTextBox rtb, string messageKey, params object[] args)
        {
            string message = _resourceManager.GetString(messageKey);
            if (message != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            rtb.AppendText((message ?? messageKey) + Environment.NewLine);
        }
    }
}
