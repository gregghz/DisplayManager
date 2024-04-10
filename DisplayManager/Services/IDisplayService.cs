using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.Services;

public interface IDisplayService
{
  Layout GetDisplayLayout();
  string GetMonitorInfo();
  Dictionary<string, string> GetDeviceMap();
  Task ApplyLayout(Layout layout);
}