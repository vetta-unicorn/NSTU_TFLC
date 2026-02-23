using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace TFLC_sem6_lab1.Handlers
{
    public class LangHandler
    {
        public void ApplyResourcesToMenu(ToolStripMenuItem item, ResourceManager resourceManager)
        {
            if (item.Tag != null && item.Tag is string resourceKey)
            {
                item.Text = resourceManager.GetString(resourceKey);
            }
            foreach (ToolStripMenuItem childItem in item.DropDownItems)
            {
                ApplyResourcesToMenu(childItem, resourceManager);
            }
        }

        private void ApplyResourcesToMenu(MenuStrip MainMenu, ResourceManager resourceManager)
        {
            foreach (ToolStripMenuItem topLevelItem in MainMenu.Items)
            {
                ApplyResourcesToMenu(topLevelItem, resourceManager);
            }
        }

        private void ApplyResourcesToInstrumentMenu(ToolStripMenuItem item, ResourceManager resourceManager)
        {
            if (item.Tag != null && item.Tag is string resourceKey)
            {
                item.ToolTipText = resourceManager.GetString(resourceKey);
            }
            foreach (ToolStripMenuItem childItem in item.DropDownItems)
            {
                ApplyResourcesToInstrumentMenu(childItem, resourceManager);
            }
        }

        private void ApplyResourcesToInstrumentMenu(MenuStrip InstrumentMenu, ResourceManager resourceManager)
        {
            foreach (ToolStripMenuItem topLevelItem in InstrumentMenu.Items)
            {
                ApplyResourcesToInstrumentMenu(topLevelItem, resourceManager);
            }
        }

        public void ChangeLanguage(string cultureName, MenuStrip MainMenu, MenuStrip InstrumentMenu, ResourceManager resourceManager)
        {
            CultureInfo newCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;

            ApplyResourcesToMenu(MainMenu, resourceManager);
            ApplyResourcesToInstrumentMenu(InstrumentMenu, resourceManager);
        }
    }
}
