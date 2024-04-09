using Gregghz.DisplayManager.UI.Cli;
using Gregghz.DisplayManager.Ui.Gui.Views;
using Microsoft.UI.Xaml;

namespace Gregghz.DisplayManager;

public partial class App
{
  public App()
  {
    InitializeComponent();
  }


  protected override void OnLaunched(LaunchActivatedEventArgs args)
  {
    var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var path = Path.Combine(appDataPath, "DisplayManager");
    Directory.CreateDirectory(path);
    var displayManager = new DisplayManager(path);

    if (args.Arguments.Length == 0)
    {
      var mainWindow = new MainWindow(displayManager);
      mainWindow.Activate();
    }
    else
    {
      // cli stuff
      var console = new ConsoleMain(displayManager);
      console.Run([args.Arguments]);
      // Current.Shutdown();
    }
  }
}