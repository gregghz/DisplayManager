using CommandLine;
using Gregghz.DisplayManager.UI.Cli.Model;

namespace Gregghz.DisplayManager.UI.Cli;

public class ConsoleMain(DisplayManager displayManager)
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
      displayManager.GetInfo();
      return;
    }

    if (opts.ListLayouts)
    {
      await ListLayouts();
      return;
    }

    switch (opts.ConfigToLoad, opts.ConfigToSave)
    {
      case (string, null):
        await displayManager.ApplyConfig(opts.ConfigToLoad);
        break;
      case (null, string):
        await displayManager.SaveLayout(opts.ConfigToSave);
        break;
      default:
        await Console.Error.WriteLineAsync("Invalid arguments.");
        break;
    }
  }


  private async Task ListLayouts()
  {
    var files = await displayManager.GetSavedLayouts();

    foreach (var file in files) Console.WriteLine(file);
  }
}