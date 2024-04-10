using CommandLine;
using Gregghz.DisplayManager.Services;
using Gregghz.DisplayManager.UI.Cli.Model;

namespace Gregghz.DisplayManager.UI.Cli;

public class ConsoleMain(IDisplayService displayService, ILayoutService layoutService)
{
  public void Run(string[] args)
  {
    Parser.Default.ParseArguments<CliOptions>(args)
      .WithParsed(Run)
      .WithNotParsed(errs =>
      {
        foreach (var err in errs) Console.WriteLine(err);
      });
  }

  private async void Run(CliOptions opts)
  {
    if (opts.Info)
    {
      var result = displayService.GetMonitorInfo();
      Console.WriteLine(result);
      return;
    }

    if (opts.ListLayouts)
    {
      await ListLayouts();
      return;
    }

    switch (opts.ConfigToLoad, opts.ConfigToSave)
    {
      case (not null, null):
        var layout = await layoutService.GetLayout(opts.ConfigToLoad);
        if (layout is not null) await displayService.ApplyLayout(layout);
        else
          // @TODO: handle this
          await Console.Error.WriteLineAsync($"{layout} does not exist.");

        break;
      case (null, not null):
        var currentLayout = displayService.GetDisplayLayout();
        await layoutService.SaveLayout(opts.ConfigToSave, currentLayout);
        break;
      default:
        await Console.Error.WriteLineAsync("Invalid arguments.");
        break;
    }
  }


  private async Task ListLayouts()
  {
    var layoutNames = await layoutService.GetSavedLayouts();

    foreach (var layoutName in layoutNames) Console.WriteLine(layoutName);
  }
}