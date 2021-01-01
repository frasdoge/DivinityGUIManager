using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using LSLib.LS;

namespace DivinityGUIManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GameDataButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = ConfigurationManager.AppSettings.Get("GameDataPath"),
                IsFolderPicker = true,
                Title = "Select the DOS2 Data directory."
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (File.Exists(dialog.FileName + @"\Origins.pak"))
                {
                    GameDataText.Text = dialog.FileName;
                }
                else
                {
                    MessageBox.Show("Could not find Origins.pak inside selected directory.\nPlease check you have selected a DOS2 DefEd data directory.", "Divinity GUI Manager Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ModFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = ConfigurationManager.AppSettings.Get("ModFilePath"),
                Filter = "DOS2 Mod Pak (*.pak)|*.pak"
            };

            if (dialog.ShowDialog() == true)
            {
                ModFileText.Text = dialog.FileName;
            }
        }

        private void GameDataText_Initialized(object sender, EventArgs e)
        {
            GameDataText.Text = ConfigurationManager.AppSettings.Get("GameDataPath");
            if (string.IsNullOrWhiteSpace(GameDataText.Text))
            {
                GameDataText.Text = Path.GetPathRoot(Environment.SystemDirectory);
                App.UpdateSetting("GameDataPath", GameDataText.Text);
            }
        }

        private void ModFileText_Initialized(object sender, EventArgs e)
        {
            ModFileText.Text = ConfigurationManager.AppSettings.Get("ModFilePath");
            if (string.IsNullOrWhiteSpace(ModFileText.Text))
            {
                ModFileText.Text = Path.GetPathRoot(Environment.SystemDirectory);
                App.UpdateSetting("ModFilePath", ModFileText.Text);
            }
        }

        private void EnableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(GameDataText.Text + @"\Origins.pak") || Path.GetExtension(ModFileText.Text) != ".pak")
            {
                MessageBox.Show("Please enter a valid Game Data and Mod File path.", "Divinity GUI Manager Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string tempDirectory = Path.Combine(Path.GetTempPath(), @"DivinityGUIManagerTemp");
            Directory.CreateDirectory(tempDirectory);

            string gameGuiDirectory = Path.Combine(GameDataText.Text, @"Public\Game\GUI");
            Directory.CreateDirectory(gameGuiDirectory);

            string modName = Path.GetFileNameWithoutExtension(ModFileText.Text);
            string extrGuiDirectory = Path.Combine(tempDirectory, $@"Public\{modName}\GUI");
            
            var packager = new Packager();
            packager.UncompressPackage(ModFileText.Text, tempDirectory, null);

            string[] files = Directory.GetFiles(extrGuiDirectory);
            
            foreach (string s in files)
            {
                var fileName = Path.GetFileName(s);
                var destFile = Path.Combine(gameGuiDirectory, fileName);
                File.Copy(s, destFile, true);
            }

            Directory.Delete(tempDirectory, true);

            App.UpdateSetting("GameDataPath", GameDataText.Text);
            App.UpdateSetting("ModFilePath", ModFileText.Text);
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(GameDataText.Text + @"\Origins.pak"))
            {
                string gameGuiDirectory = Path.Combine(GameDataText.Text, @"Public\Game\GUI");
                Directory.Delete(gameGuiDirectory, true);
            }
        }
    }
}
