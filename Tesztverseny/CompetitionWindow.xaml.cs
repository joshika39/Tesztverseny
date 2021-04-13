using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesztverszeny;
using Path = System.IO.Path;

namespace Tesztverseny
{

    public partial class CompetitionWindow
    {
        
        private Importer _importer;
        private readonly bool _isMain;
        
        /* Making the basic configurations like:
         * - Icon setup
         * - Title setup
         * - Main Window check
         * - Calling the selectable file list reload
         */
        public CompetitionWindow(bool isMain)
        {
            InitializeComponent();
            var iconUri = new Uri("fuuka.ico", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri);
            
            _isMain = isMain;
            if (!_isMain)
                OpenNewWindow.Visibility = Visibility.Collapsed;
            
            Title = _isMain ? "Verseny Információ Főablak" : "Verseny Információ Segédablak";
            
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "Competition Inspector", "Versenyek");
            Directory.CreateDirectory(specificFolder);
            
            var wClient = new WebClient();
            wClient.DownloadFile("https://kreastol.club/valaszok.txt", specificFolder + "/valaszok.txt");
            
            _importer= new Importer();
            VersenyContent content = new VersenyContent(_importer);
            WindowContent.Children.Add(content);
            
            ReloadSelectableFiles();
        }
        
        /* Method: Reloads the menu list of the selectable competition list
         * List name: SelectFileName 
         */
        private void ReloadSelectableFiles()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "Competition Inspector/Versenyek");
            Directory.CreateDirectory(specificFolder);
            
            DirectoryInfo dirInfo = new DirectoryInfo(specificFolder);
            
            var info = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            SelectFileName.Items.Clear();
            foreach (var file in info)
            {
                MenuItem temp = new MenuItem {Header = Path.ChangeExtension(file.Name, null)};
                
                temp.Click += SelectFile;
                SelectFileName.Items.Add(temp);
            }
        }
        
        /* MenuItem EventHandler: If there isn't a valaszok file user can download it from here
         * Also calls the ReloadSelectableFiles() method
         */
        private void DownloadFile(object sender, RoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "Competition Inspector", "Versenyek");
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
            string specificFolder = Path.Combine(folder, "Competition Inspector", "Versenyek");
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

        
        /* MenuItem EventHandler: Opens a new window passing a 'false' bool,
         * to separate the secondary and main window
         */
        private void NewWindow(object sender, RoutedEventArgs e)
        {
            Window window = new CompetitionWindow(false);
            window.Show();
        }
        
        /* MenuItem EventHandler:
         *  - Creating a new Importer class
         *  - Passing the importer the the content UserControl
         */
        private void SelectFile(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            if (mi == null) return;
            
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "Competition Inspector/Versenyek");
            Directory.CreateDirectory(specificFolder);
                
            _importer = new Importer();
            _importer.Import(specificFolder + "/" + mi.Header + ".txt");
                
            VersenyContent content = new VersenyContent(_importer);
            if(emptyNotify.Visibility != Visibility.Collapsed)
                emptyNotify.Visibility = Visibility.Collapsed;
            WindowContent.Children.Clear();
            WindowContent.Children.Add(content);
        }
        
        // Obvious :) It quits
        private void AppQuit(object sender, RoutedEventArgs e)
        {
            if(_isMain)
                Application.Current.Shutdown();
            else
                Close();
        }

        
    }
}