using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Gregghz.DisplayManager.Native;

namespace Gregghz.DisplayManager.Model;

public record Settings(
  string Name,
  bool IsPrimary,
  Position Position,
  Resolution Resolution,
  Orientation Orientation,
  uint Frequency
)
{
  public static Settings FromDevMode(string deviceName, in DEVMODE mode)
  {
    return new Settings(
      Name: deviceName,
      IsPrimary: mode.dmPositionX == 0 && mode.dmPositionY == 0,
      Position: new Position(mode.dmPositionX, mode.dmPositionY),
      Resolution: new Resolution(mode.dmPelsWidth, mode.dmPelsHeight),
      Orientation: (Orientation)mode.dmDisplayOrientation,
      Frequency: (uint)mode.dmDisplayFrequency
    );
  }

  public DEVMODE UpdateDevMode(ref DEVMODE mode)
  {
    if (IsPrimary)
    {
      mode.dmPositionX = 0;
      mode.dmPositionY = 0;
    }
    else
    {
      mode.dmPositionX = Position.X;
      mode.dmPositionY = Position.Y;
    }

    mode.dmPelsWidth = Resolution.Width;
    mode.dmPelsHeight = Resolution.Height;

    mode.dmDisplayOrientation = (int)Orientation;
    mode.dmDisplayFrequency = (int)Frequency;

    return mode;
  }
};

public record Position(int X, int Y);

public record Resolution(int Width, int Height);

public enum Orientation
{
  Landscape = 1,
  Portrait,
  LandscapeFlipped,
  PortraitFlipped
}
