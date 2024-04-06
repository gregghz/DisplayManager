using System.IO;
using System.Windows;
using Gregghz.DisplayManager.UI.Cli;
using Gregghz.DisplayManager.UI.Gui;

namespace Gregghz.DisplayManager;

public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    string path = Path.Combine(appDataPath, "DisplayManager");
    Directory.CreateDirectory(path);

    if (e.Args.Length == 0)
    {
      var mainWindow = new MainWindow(path);
      mainWindow.Show();
    }
    else
    {
      // cli stuff
      var console = new ConsoleMain(path);
      console.Run(e.Args);
      Current.Shutdown();
    }
  }
}