using System.Runtime.InteropServices;

namespace Gregghz.DisplayManager.Windows.Native;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct DISPLAY_DEVICE
{
  public int cb;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
  public string DeviceName;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
  public string DeviceString;
  public uint StateFlags;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
  public string DeviceID;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
  public string DeviceKey;
}