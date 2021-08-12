using System.Windows;

namespace PasteDownload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            PasteDownload.Properties.Settings.Default.Save();
        }
    }
}
