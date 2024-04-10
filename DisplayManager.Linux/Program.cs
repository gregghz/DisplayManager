using Gregghz.DisplayManager.Services;
using Gregghz.DisplayManager.Services.Implementations;
using Gregghz.DisplayManager.UI.Cli;
using Gtk;

namespace Gregghz.DisplayManager;

public class Program
{
  private static void Main(string[] args)
  {
    var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var path = Path.Combine(appDataPath, "DisplayManager");
    Directory.CreateDirectory(path);

    var layoutService = new FileSystemLayoutService(path);
    IDisplayService displayService =
      Environment.GetEnvironmentVariable("XDG_SESSION_TYPE") switch
      {
        "x11" => new XrandrDisplayService(),
        "wayland" => throw new NotImplementedException(),
        _ => throw new NotImplementedException()
      };

    if (args.Length == 0)
    {
      Application.Init();

      var window = new Window("GTK#")
      {
        DefaultWidth = 800,
        DefaultHeight = 600
      };

      window.DeleteEvent += (sender, e) => Application.Quit();

      window.ShowAll();

      Application.Run();
    }
    else
    {
      var console = new ConsoleMain(displayService, layoutService);
      console.Run(args);
    }
  }
}