using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using NewCompetitionAnalizer.MVVM.View;

namespace NewCompetitionAnalizer
{
    public partial class MainWindow
    {
        private string fileLocation;
        private int selectedView;
        
        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            string path = Path.GetFullPath("Images/image.jpg");
            CloseImage.Source = new BitmapImage(new Uri(@"D:\programming\classstuff\oop\Tesztverseny\NewCompetitionAnalizer\Images\close_button.png"));
            HomeView home = new HomeView(fileLocation);
            WindowContent.Children.Add(home);
            selectedView = 0;
            ReloadSelectableFiles();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");
            Directory.CreateDirectory(specificFolder);

            //Set up the dialog for file selection
            var openFileDialog1 = new OpenFileDialog
            {
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

        private void DownloadData
            (object sender, RoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");
            Directory.CreateDirectory(specificFolder);

            var dialog = new SaveFileDialog
            {
                Filter = "TXT (*.txt)|*.txt", InitialDirectory = specificFolder, RestoreDirectory = true
            };
            var result = dialog.ShowDialog(); //shows save file dialog
            if (result == true)
            {
                var wClient = new WebClient();
                wClient.DownloadFile("https://kreastol.club/valaszok.txt", dialog.FileName);
            }
            ReloadSelectableFiles();
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var specificFolder = Path.Combine(folder, "VersenyInfo/Versenyek");
            Directory.CreateDirectory(specificFolder);
            fileLocation = specificFolder + "/" + FileList.SelectedValue + ".txt";

            switch (selectedView)
            {
                case 1:
                    TaskView task = new TaskView(fileLocation);
                    WindowContent.Children.Clear();
                    WindowContent.Children.Add(task);
                    break;
                default:
                    HomeView home = new HomeView(fileLocation);
                    WindowContent.Children.Clear();
                    WindowContent.Children.Add(home);
                    break;
            }
        }

        /* Method to open data folder in file explorer 
         * Called by "Pontok" button
         */
        private void OpenDataFolder(object sender, EventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(folder, "VersenyInfo");
            folderPath += "/data/";
            
            Process.Start(folderPath);
        }
        
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

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            selectedView = 0;
            HomeView home = new HomeView(fileLocation);
            WindowContent.Children.Clear();
            WindowContent.Children.Add(home);
        }

        private void TaskClick(object sender, RoutedEventArgs e)
        {
            selectedView = 1;
            TaskView task = new TaskView(fileLocation);
            WindowContent.Children.Clear();
            WindowContent.Children.Add(task);
        }
    }
}