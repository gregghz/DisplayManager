using System.Text.Json;
using CommandLine;
using Gregghz.DisplayManager;
using Gregghz.DisplayManager.Model;

string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
string path = Path.Combine(appDataPath, "DisplayManager");
Directory.CreateDirectory(path);

Parser.Default.ParseArguments<CliOptions>(args)
  .WithParsed(Run)
  .WithNotParsed(errs =>
  {
    foreach (var err in errs)
    {
      Console.WriteLine(err);
    }
  });

void Run(CliOptions opts)
{
  if (opts.Info)
  {
    DisplayManager.GetInfo();
    return;
  }

  if (opts.ListLayouts)
  {
    ListLayouts();
    return;
  }

  switch (opts.ConfigToLoad, opts.ConfigToSave)
  {
    case (string, null):
      ApplyConfig(opts.ConfigToLoad);
      break;
    case (null, string):
      SaveConfig(opts.ConfigToSave);
      break;
    default:
      Console.Error.WriteLine("Invalid arguments.");
      break;
  }
}

void ApplyConfig(string name)
{
  try
  {
    string configPath = Path.Combine(path, $"{name}.json");
    string json = File.ReadAllText(configPath);
    var settings = JsonSerializer.Deserialize<List<Settings>>(json);
    if (settings is null)
    {
      Console.Error.WriteLine("Failed to parse settings. Try saving your layout again.");
      return;
    }

    foreach (var s in settings)
    {
      DisplayManager.UpdateSettings(s.Name, s);
    }

    DisplayManager.ApplySettings();
  }
  catch (FileNotFoundException)
  {
    Console.Error.WriteLine($"There is no saved layout with the name {name}");
  }
}

void SaveConfig(string name)
{
  string configPath = Path.Combine(path, $"{name}.json");

  var currentSettings = DisplayManager.GetCurrentLayout();

  var data = JsonSerializer.Serialize(currentSettings);
  File.WriteAllText(configPath, data);

  Console.WriteLine($"Current layout saved as {name}.");
}

void ListLayouts()
{
  var files = Directory.GetFiles(path, "*.json")
    .Select(Path.GetFileNameWithoutExtension)
    .OrderBy(filename => filename);

  foreach (var file in files)
  {
    Console.WriteLine(file);
  }
}