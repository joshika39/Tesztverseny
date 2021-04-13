using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Tesztverszeny;
using Path = System.IO.Path;

namespace Tesztverseny
{

    public partial class ExtraWindow
    {
        private Importer importer = new Importer();
        public ExtraWindow()
        {
            InitializeComponent();
            Uri iconUri = new Uri("fuuka.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            this.Title = "Verseny Információ";

            importer= new Importer();
            
            VersenyContent content = new VersenyContent(importer);
            
            WindowContent.Children.Add(content);

            reloadSelectableFiles();
        }

                private void reloadSelectableFiles()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo/Versenyek");
            Directory.CreateDirectory(specificFolder);
            
            DirectoryInfo dirInfo = new DirectoryInfo(specificFolder);
            
            var info = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            SelectFileName.Items.Clear();
            foreach (var file in info)
            {
                MenuItem temp = new MenuItem();
                temp.Header = Path.ChangeExtension(file.Name, null);
                temp.Click += SelectFile;
                SelectFileName.Items.Add(temp);
            }
        }

        private void DownloadFile(object sender, RoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo", "Versenyek");
            Directory.CreateDirectory(specificFolder);
            
            var dialog = new SaveFileDialog();
            dialog.Filter = "TXT (*.txt)|*.txt";
            dialog.InitialDirectory = specificFolder;
            dialog.RestoreDirectory = true;
            var result = dialog.ShowDialog(); //shows save file dialog
            if(result == true)
            {
                var wClient = new WebClient();
                wClient.DownloadFile("https://kreastol.club/valaszok.txt", dialog.FileName);
                reloadSelectableFiles();
            }
        }
        
        private void ImportFile(object sender, RoutedEventArgs e)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VersenyInfo/Versenyek");
            Directory.CreateDirectory(specificFolder);
            
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            
            openFileDialog1.DefaultExt = ".txt";
            openFileDialog1.Filter = "TXT Files (*.txt)|*.txt"; 
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = openFileDialog1.ShowDialog();
            
            string filename = openFileDialog1.SafeFileName;

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                if (File.Exists(specificFolder + "/" + filename))
                {
                    string messageBoxText = "A file már létezik, felül szeretné e írni?";
                    string caption = "Figyelem";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult resultAnswer = MessageBox.Show(messageBoxText, caption, button, icon);
                    
                    switch (resultAnswer)
                    {
                        case MessageBoxResult.Yes:
                            for (var index = 0; index < openFileDialog1.FileNames.Length; index++)
                            {
                                string _file = openFileDialog1.FileNames[index];
                                FileInfo fi = new FileInfo(_file);
                                
                                File.Copy(_file, Path.Combine(specificFolder, fi.Name),true);
                            }
                            // Process.Start(specificFolder);
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                else
                {
                    for (var index = 0; index < openFileDialog1.FileNames.Length; index++)
                    {
                        string _file = openFileDialog1.FileNames[index];
                        FileInfo fi = new FileInfo(_file);
                                
                        File.Copy(_file, Path.Combine(specificFolder, fi.Name));
                    }
                    // Process.Start(specificFolder);
                }
                reloadSelectableFiles();
            }
        }

        private void NewWindow(object sender, RoutedEventArgs e)
        {
            Window window = new ExtraWindow();
            window.Show();
        }

        private void SelectFile(object sender, RoutedEventArgs e)
        {
            MenuItem MI = (MenuItem)sender;
            if (MI != null)
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string specificFolder = Path.Combine(folder, "VersenyInfo/Versenyek");
                Directory.CreateDirectory(specificFolder);
                
                importer = new Importer();
                importer.Import(specificFolder + "/" + MI.Header + ".txt");
                
                VersenyContent content = new VersenyContent(importer);
                if(emptyNotify.Visibility != Visibility.Collapsed)
                    emptyNotify.Visibility = Visibility.Collapsed;
                WindowContent.Children.Clear();
                WindowContent.Children.Add(content);
            }
        }
        private void WindowExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}