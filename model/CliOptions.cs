namespace Gregghz.DisplayManager.Model;

using CommandLine;

public class CliOptions
{
  [Option('l', "load", Required = false, HelpText = "Load a configuation with the given name.")]
  public string? ConfigToLoad { get; set; } = null;

  [Option('i', "info", Required = false, HelpText = "Display information.")]
  public bool Info { get; set; } = false;

  [Option('s', "save", Required = false, HelpText = "Save the current display layout with the given name.")]
  public string? ConfigToSave { get; set; } = null;

  [Option('l', "list", Required = false, HelpText = "List all current saved layouts.")]
  public bool ListLayouts { get; set; } = false;
}