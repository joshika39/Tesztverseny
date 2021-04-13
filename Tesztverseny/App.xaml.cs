using System.Windows;

namespace Tesztverseny
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CompetitionWindow mainWindow = new CompetitionWindow(true);
            mainWindow.Show();
        }
    }
}