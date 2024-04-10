using Gregghz.DisplayManager.Services.Implementations;
using Gregghz.DisplayManager.UI.Cli;

namespace Gregghz.DisplayManager;

public class Program
{
   private static void Main(string[] args)
   {
     var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); 
     var path = Path.Combine(appDataPath, "DisplayManager");
     Directory.CreateDirectory(path);

     var layoutService = new FileSystemLayoutService(path);
     var displayService = new MacDisplayService();
     var console = new ConsoleMain(displayService, layoutService);
      
      console.Run(args);
   } 
}