using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace NewCompetitionAnalizer
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            string path = Path.GetFullPath("Images/image.jpg");
            CloseImage.Source = new BitmapImage(new Uri(@"D:\programming\classstuff\oop\Tesztverseny\NewCompetitionAnalizer\Images\close_button.png"));

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
                        var openFileDialog1 = new OpenFileDialog {
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
        }
    }
}