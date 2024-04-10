using System.Runtime.InteropServices;
using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.Services;
using Gregghz.DisplayManager.Windows.Extensions;
using Gregghz.DisplayManager.Windows.Native;

namespace Gregghz.DisplayManager.Windows.Services.Implementations;

public class WindowsDisplayService : IDisplayService
{
  public Layout GetDisplayLayout()
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
      foundSettings.Add(SettingsExtensions.FromDevMode(id, mode));

      return true; // Continue enumeration
    }

    User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);
    return new Layout(foundSettings);
  }

  public string GetMonitorInfo()
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

    return monitorInfo;
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

  public async Task ApplyLayout(Layout layout)
  {
    var settings = layout.Settings;

    foreach (var s in settings) await UpdateSettings(s.DeviceId, s);

    ApplySettings();
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

  private async Task<int> UpdateSettings(string deviceId, Settings settings)
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

  private static int ApplySettings(string? deviceName = null)
  {
    return User32.ChangeDisplaySettingsEx(
      deviceName,
      IntPtr.Zero,
      (IntPtr)null,
      Constants.CDS_NONE,
      (IntPtr)null);
  }
}