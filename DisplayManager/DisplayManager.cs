using System.Runtime.InteropServices;
using System.Text.Json;
using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.Native;

namespace Gregghz.DisplayManager;

public class DisplayManager(string path)
{
  public event EventHandler<IList<string>>? LayoutsUpdated;
  public event EventHandler<IList<Settings>>? LayoutLoaded;

  public async void FireLayoutUpdated()
  {
    if (LayoutsUpdated is null) return;
    var layouts = await GetSavedLayouts();
    LayoutsUpdated.Invoke(this, layouts);
  }

  public void GetInfo()
  {
    var monitorInfo = "";

    bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
      var mi = new MONITORINFOEX();
      User32.GetMonitorInfo(hMonitor, mi);
      var deviceName = new string(mi.szDevice).TrimEnd((char)0);

      var mode = new DEVMODE();
      User32.EnumDisplaySettings(deviceName, Constants.ENUM_CURRENT_SETTINGS, ref mode);

      monitorInfo +=
        $"Monitor: {deviceName} ({hMonitor}) - Bounds: {mi.rcMonitor.left}, {mi.rcMonitor.top}, {mi.rcMonitor.right}, {mi.rcMonitor.bottom}\n";
      monitorInfo += $"{mode}\r\n\r\n";
      return true; // Continue enumeration
    }

    User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);

    Console.WriteLine(monitorInfo);
  }

  public async Task<int> UpdateSettings(string deviceId, Settings settings)
  {
    var deviceMap = GetDeviceMap();
    var deviceName = deviceMap[deviceId]; // @TODO: handle missing ID

    var mode = new DEVMODE();
    await Task.Run(() => User32.EnumDisplaySettings(deviceName, Constants.ENUM_CURRENT_SETTINGS, ref mode));

    settings.UpdateDevMode(ref mode);

    uint dwFlags = Constants.CDS_UPDATEREGISTRY | Constants.CDS_NORESET;
    if (settings.IsPrimary) dwFlags |= Constants.CDS_SET_PRIMARY;

    var result = await Task.Run(() =>
      User32.ChangeDisplaySettingsEx(deviceName, ref mode, IntPtr.Zero, dwFlags, IntPtr.Zero));
    return result;
  }

  public Layout GetCurrentLayout()
  {
    List<Settings> foundSettings = [];
    var deviceMap = GetDeviceMap();

    bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
      var mi = new MONITORINFOEX();
      User32.GetMonitorInfo(hMonitor, mi);
      var deviceName = new string(mi.szDevice).TrimEnd((char)0);

      var mode = new DEVMODE();
      User32.EnumDisplaySettings(deviceName, Constants.ENUM_CURRENT_SETTINGS, ref mode);

      var id = deviceMap[deviceName];
      foundSettings.Add(Settings.FromDevMode(id, mode));

      return true; // Continue enumeration
    }

    User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);
    return new Layout(foundSettings);
  }

  private IEnumerable<DISPLAY_DEVICE> GetDisplayDevices()
  {
    var d = new DISPLAY_DEVICE();
    d.cb = Marshal.SizeOf(d);

    List<DISPLAY_DEVICE> result = new();

    for (uint id = 0; User32.EnumDisplayDevices(null, id, ref d, 0); id++)
    {
      result.Add(d);

      d = new DISPLAY_DEVICE();
      d.cb = Marshal.SizeOf(d);
    }

    return result;
  }

  public Dictionary<string, string> GetDeviceMap()
  {
    var result = new Dictionary<string, string>();

    foreach (var device in GetDisplayDevices())
    {
      result.Add(device.DeviceName, device.DeviceKey);
      result.Add(device.DeviceKey, device.DeviceName);
    }

    return result;
  }

  public string GetDeviceName(IntPtr hMonitor)
  {
    var mi = new MONITORINFOEX();
    User32.GetMonitorInfo(hMonitor, mi);
    return new string(mi.szDevice).TrimEnd((char)0);
  }

  private static int ApplySettings(string? deviceName = null)
  {
    return User32.ChangeDisplaySettingsEx(
      deviceName,
      IntPtr.Zero,
      (IntPtr)null,
      Constants.CDS_NONE,
      (IntPtr)null);
  }

  public Task<List<string>> GetSavedLayouts()
  {
    return Task.Run(() =>
    {
      return Directory.GetFiles(path, "*.json")
        .Select(Path.GetFileNameWithoutExtension)
        .Where(s => s != null)
        .Select(s => s!)
        .OrderBy(filename => filename)
        .ToList();
    });
  }

  public async Task ApplyConfig(string name)
  {
    try
    {
      var configPath = Path.Combine(path, $"{name}.json");
      var json = await File.ReadAllTextAsync(configPath);
      var settings = JsonSerializer.Deserialize<List<Settings>>(json);
      if (settings is null)
      {
        await Console.Error.WriteLineAsync("Failed to parse settings. Try saving your layout again.");
        return;
      }

      foreach (var s in settings) await UpdateSettings(s.DeviceId, s);

      ApplySettings();
    }
    catch (FileNotFoundException)
    {
      await Console.Error.WriteLineAsync($"There is no saved layout with the name {name}");
    }
  }

  public async Task<IList<Settings>> LoadLayoutSettings(string name)
  {
    var configPath = Path.Combine(path, $"{name}.json");
    try
    {
      var json = await Task.Run(() => File.ReadAllText(configPath));
      var settings = JsonSerializer.Deserialize<List<Settings>>(json);

      if (settings is null)
      {
        Console.Error.WriteLine("Failed to parse settings. Try saving your layout again.");
        settings = [];
      }

      LayoutLoaded?.Invoke(this, settings);

      return settings;
    }
    catch (FileNotFoundException)
    {
      FireLayoutUpdated();
      return [];
    }
  }

  public async Task SaveLayout(string name)
  {
    var configPath = Path.Combine(path, $"{name}.json");

    try
    {
      var currentSettings = GetCurrentLayout();

      var data = JsonSerializer.Serialize(currentSettings.Settings);
      await File.WriteAllTextAsync(configPath, data);

      FireLayoutUpdated();

      Console.WriteLine($"Current layout saved as {name}.");
    }
    catch (Exception e)
    {
      await Console.Error.WriteLineAsync($"error: {e}");
    }
  }
}