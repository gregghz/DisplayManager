using Gregghz.DisplayManager.Services.Implementations;
using Gregghz.DisplayManager.UI.Cli;
using Gregghz.DisplayManager.Windows.Services.Implementations;
using Gregghz.DisplayManager.Windows.Views;
using Microsoft.UI.Xaml;

namespace Gregghz.DisplayManager.Windows;

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

    // @TODO: DI
    var layoutService = new FileSystemLayoutService(path);
    var displayService = new WindowsDisplayService();

    var cliArgs = Environment.GetCommandLineArgs();

    if (cliArgs.Length == 0 || cliArgs[0].EndsWith(".dll"))
    {
      var mainWindow = new MainWindow(displayService, layoutService);
      mainWindow.Activate();
    }
    else
    {
      // cli stuff
      var console = new ConsoleMain(displayService, layoutService);
      console.Run(cliArgs);
      // Current.Shutdown();
    }
  }
}