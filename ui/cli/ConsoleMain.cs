using CommandLine;
using Gregghz.DisplayManager.UI.Cli.Model;

namespace Gregghz.DisplayManager.UI.Cli;

public class ConsoleMain(string path)
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
      DisplayManager.GetInfo();
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
        await DisplayManager.ApplyConfig(opts.ConfigToLoad, path);
        break;
      case (null, string):
        await DisplayManager.SaveLayout(opts.ConfigToSave, path);
        break;
      default:
        await Console.Error.WriteLineAsync("Invalid arguments.");
        break;
    }
  }


  private async Task ListLayouts()
  {
    var files = await DisplayManager.GetSavedLayouts(path);

    foreach (var file in files) Console.WriteLine(file);
  }
}