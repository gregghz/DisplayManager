using System.Runtime.InteropServices;
using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.Services.Implementations;

public class MacDisplayService : IDisplayService
{
  public Layout GetDisplayLayout()
  {
    throw new NotImplementedException();
  }

  public string GetMonitorInfo()
  {
    var onlineDisplaysPtr = Marshal.AllocHGlobal(10 * sizeof(uint)); // Allocate memory for the display IDs

    var error = CGGetOnlineDisplayList(10, onlineDisplaysPtr, out var count);
    if (error == 0) // 0 typically indicates success
      for (var i = 0; i < count; i++)
      {
        // Read the display ID from the unmanaged array
        var displayId = (uint)Marshal.ReadInt32(onlineDisplaysPtr, i * sizeof(uint));
        Console.WriteLine($"Display ID {i}: {displayId}");
      }
    else
      Console.WriteLine($"Error retrieving display list. Error code: {error}");

    Marshal.FreeHGlobal(onlineDisplaysPtr); // Free the allocated memory

    return count.ToString();
  }

  public Dictionary<string, string> GetDeviceMap()
  {
    throw new NotImplementedException();
  }

  public Task ApplyLayout(Layout layout)
  {
    throw new NotImplementedException();
  }

  [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
  public static extern int CGGetOnlineDisplayList(uint maxDisplays, IntPtr onlineDisplays, out uint displayCount);
}