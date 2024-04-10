using System.Text.Json;
using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.Services.Implementations;

public class FileSystemLayoutService(string basePath) : ILayoutService
{
  public event EventHandler<Layout>? LayoutLoaded;
  public event EventHandler<IList<string>>? LayoutsChanged;

  public Task<List<string>> GetSavedLayouts()
  {
    return Task.Run(() =>
    {
      return Directory.GetFiles(basePath, "*.json")
        .Select(Path.GetFileNameWithoutExtension)
        .Where(s => s != null)
        .Select(s => s!)
        .OrderBy(filename => filename)
        .ToList();
    });
  }

  public async Task<Layout?> GetLayout(string name)
  {
    var configPath = Path.Combine(basePath, $"{name}.json");
    try
    {
      var json = await Task.Run(() => File.ReadAllText(configPath));
      var settings = JsonSerializer.Deserialize<List<Settings>>(json);

      if (settings is null)
      {
        Console.Error.WriteLine("Failed to parse settings. Try saving your layout again.");
        settings = [];
      }

      // LayoutLoaded?.Invoke(this, settings);

      return new Layout(settings);
    }
    catch (FileNotFoundException)
    {
      // FireLayoutUpdated();
      // @TODO: handle errors
      return null;
    }
  }

  public async Task SaveLayout(string name, Layout layout)
  {
    var configPath = Path.Combine(basePath, $"{name}.json");

    try
    {
      var data = JsonSerializer.Serialize(layout.Settings);
      await File.WriteAllTextAsync(configPath, data);

      // FireLayoutUpdated();

      Console.WriteLine($"Current layout saved as {name}.");
    }
    catch (Exception e)
    {
      await Console.Error.WriteLineAsync($"error: {e}");
    }
  }
}