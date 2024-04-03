using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.Native;

namespace Gregghz.DisplayManager;

static class DisplayManager
{
  public static uint[] GetInfo()
  {
    string MonitorInfo = "";
    uint[] monitors = new uint[3]; // @TODO: make this not static
    int i = 0;

    bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
      MONITORINFOEX mi = new MONITORINFOEX();
      User32.GetMonitorInfo(hMonitor, mi);
      string deviceName = new string(mi.szDevice).TrimEnd((char)0);

      DEVMODE mode = new DEVMODE();
      User32.EnumDisplaySettings(deviceName, Constants.ENUM_CURRENT_SETTINGS, ref mode);

      MonitorInfo += $"Monitor: {deviceName} ({hMonitor}) - Bounds: {mi.rcMonitor.left}, {mi.rcMonitor.top}, {mi.rcMonitor.right}, {mi.rcMonitor.bottom}\n";
      MonitorInfo += $"\t{mode}\n\n";
      monitors[i++] = (uint)hMonitor;
      return true; // Continue enumeration
    }

    User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);

    Console.WriteLine(MonitorInfo);
    return monitors;
  }

  public static int UpdateSettings(string deviceName, Settings settings)
  {
    DEVMODE mode = new DEVMODE();
    User32.EnumDisplaySettings(deviceName, Constants.ENUM_CURRENT_SETTINGS, ref mode);

    settings.UpdateDevMode(ref mode);

    uint dwFlags = Constants.CDS_UPDATEREGISTRY | Constants.CDS_NORESET;
    if (settings.IsPrimary)
    {
      dwFlags |= Constants.CDS_SET_PRIMARY;
    }

    int result = User32.ChangeDisplaySettingsEx(deviceName, ref mode, IntPtr.Zero, dwFlags, IntPtr.Zero);
    return result;
  }

  public static IList<Settings> GetCurrentLayout()
  {
    List<Settings> foundSettings = [];
    bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
      MONITORINFOEX mi = new MONITORINFOEX();
      User32.GetMonitorInfo(hMonitor, mi);
      string deviceName = new string(mi.szDevice).TrimEnd((char)0);

      DEVMODE mode = new DEVMODE();
      User32.EnumDisplaySettings(deviceName, Constants.ENUM_CURRENT_SETTINGS, ref mode);

      foundSettings.Add(Settings.FromDevMode(deviceName, mode));

      return true; // Continue enumeration
    }

    User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);
    return foundSettings;
  }

  public static string GetDeviceName(IntPtr hMonitor)
  {
    MONITORINFOEX mi = new MONITORINFOEX();
    User32.GetMonitorInfo(hMonitor, mi);
    return new string(mi.szDevice).TrimEnd((char)0);
  }

  public static int ApplySettings(string? deviceName = null)
  {
    return User32.ChangeDisplaySettingsEx(
      deviceName,
      IntPtr.Zero,
      (IntPtr)null,
      Constants.CDS_NONE,
      (IntPtr)null);
  }
}