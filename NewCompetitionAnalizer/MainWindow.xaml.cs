using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NewCompetitionAnalizer
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            string path = Path.GetFullPath("Images/image.jpg");
            CloseImage.Source = new BitmapImage(new Uri(@"D:\学校\Jetbrains\C#\Tesztverseny\NewCompetitionAnalizer\Images\close_button.png"));

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
    }
}