using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Tesztverszeny;
using VersenyInfo.MVVM.ViewModel;

namespace VersenyInfo.MVVM.View
{
    public partial class SettingsView : UserControl
    {
        private Window parent;
        public SettingsView()
        {
            InitializeComponent();
            parent = Window.GetWindow(this);
            ReloadSelectableFiles();
        }
        
        private void ComboBox_DropDownClosed(object sender, EventArgs e) {
            
            if(FileList.SelectedItem == null) return;
            
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "VersenyInfo/Versenyek");
            Directory.CreateDirectory(specificFolder);

            // parent.Location = specificFolder + "/" + FileList.SelectedValue + ".txt";
            //
            // MessageBox.Show(
            //     $"Path: {parent.Location}",
            //     "Figyelem",
            //     MessageBoxButton.OK,
            //     MessageBoxImage.Asterisk);
        }
    
        
        /* Method: Reloads the menu list of the selectable competition list
         * List name: SelectFileName 
         */
        private void ReloadSelectableFiles()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo/Versenyek");
            Directory.CreateDirectory(specificFolder);
            
            DirectoryInfo dirInfo = new DirectoryInfo(specificFolder);
            
            var info = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            FileList.Items.Clear();
            
            var temp = new ComboBoxItem(){Content = "Válasszon fájlt", IsSelected = true};
            FileList.Items.Add(temp);

            foreach (var file in info)
            {
                temp = new ComboBoxItem(){Content = Path.ChangeExtension(file.Name, null)};
                FileList.Items.Add(temp);
            }
        }
        
        /* MenuItem EventHandler: If there isn't a valaszok file user can download it from here
         * Also calls the ReloadSelectableFiles() method
         */
        private void DownloadFile(object sender, RoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");
            Directory.CreateDirectory(specificFolder);

            var dialog = new SaveFileDialog
            {
                Filter = "TXT (*.txt)|*.txt", InitialDirectory = specificFolder, RestoreDirectory = true
            };
            var result = dialog.ShowDialog(); //shows save file dialog
            if(result == true)
            {
                var wClient = new WebClient();
                wClient.DownloadFile("https://kreastol.club/valaszok.txt", dialog.FileName);
                ReloadSelectableFiles();
            }
        }
        
        /* MenuItem EventHandler: Use to import a file the the user's appdata folder*/
        private void ImportFile(object sender, RoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");
            Directory.CreateDirectory(specificFolder);

            //Set up the dialog for file selection
            OpenFileDialog openFileDialog1 = new OpenFileDialog {
                DefaultExt = ".txt", Filter = "TXT Files (*.txt)|*.txt"
            };

            
            // Display OpenFileDialog by calling ShowDialog method 
            var result = openFileDialog1.ShowDialog();
            var filePath = openFileDialog1.FileName;
            var filename = openFileDialog1.SafeFileName;

            // Get the selected file name and display in a TextBox 
            if (result != true) return;

            if (Path.GetDirectoryName(filePath) == specificFolder)
            {
                MessageBox.Show(
                    $"Nem tud az adat mappabol importalni!",
                    "Figyelem",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                return; 
            }
            
            if (File.Exists(specificFolder + "/" + filename))
            {
                //Massage box for file overwrite
                var resultAnswer = MessageBox.Show(
                    $"A(z) \"{filename}\" nevu file már létezik, felül szeretné e írni?",
                    "Figyelem",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                    
                switch (resultAnswer)
                {
                    case MessageBoxResult.Yes:
                        File.Copy(filePath, Path.Combine(specificFolder, filename), true);
                        break;
                    // More cases so the program won't crash :) 
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.None:
                        break;
                    case MessageBoxResult.OK:
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
                File.Copy(filePath, Path.Combine(specificFolder, filename));
                
            ReloadSelectableFiles();
        }

        public class FileListItem
        {
            private string _name;

            public FileListItem(string name)
            {
                _name = name;
            }
        }
    }
}