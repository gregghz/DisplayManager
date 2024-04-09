using System.Runtime.InteropServices;

namespace Gregghz.DisplayManager.Native;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class MONITORINFOEX
{
  public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
  public RECT rcMonitor = new RECT();
  public RECT rcWork = new RECT();
  public int dwFlags = 0;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
  public char[] szDevice = new char[32];
}