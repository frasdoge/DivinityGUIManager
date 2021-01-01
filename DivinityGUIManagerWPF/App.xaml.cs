using System.Configuration;
using System.Windows;

namespace DivinityGUIManagerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void UpdateSetting(string name, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings[name].Value = value;
            configFile.Save(ConfigurationSaveMode.Modified);
        }
    }
}
