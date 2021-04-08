using Tesztverszeny;

namespace Tesztverseny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Importer importer = new Importer();
            importer.Import("valaszok.txt");
        }
    }
}