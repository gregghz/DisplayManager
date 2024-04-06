using Gregghz.DisplayManager.Native;

namespace Gregghz.DisplayManager.Model;

public record Settings(
  string DeviceId,
  bool IsPrimary,
  Position Position,
  Resolution Resolution,
  Orientation Orientation,
  uint Frequency
)
{
  public int RightX => Position.X + Resolution.Width;
  public int BottomY => Position.Y + Resolution.Height;

  public static Settings FromDevMode(string deviceId, in DEVMODE mode)
  {
    return new Settings(
      deviceId,
      mode.dmPositionX == 0 && mode.dmPositionY == 0,
      new Position(mode.dmPositionX, mode.dmPositionY),
      new Resolution(mode.dmPelsWidth, mode.dmPelsHeight),
      (Orientation)mode.dmDisplayOrientation,
      (uint)mode.dmDisplayFrequency
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
}

public record Position(int X, int Y);

public record Resolution(int Width, int Height);

public enum Orientation
{
  Landscape = 1,
  Portrait,
  LandscapeFlipped,
  PortraitFlipped
}