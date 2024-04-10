using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.Windows.Native;

namespace Gregghz.DisplayManager.Windows.Extensions;

public static class SettingsExtensions
{
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

  public static DEVMODE UpdateDevMode(this Settings settings, ref DEVMODE mode)
  {
    if (settings.IsPrimary)
    {
      mode.dmPositionX = 0;
      mode.dmPositionY = 0;
    }
    else
    {
      mode.dmPositionX = settings.Position.X;
      mode.dmPositionY = settings.Position.Y;
    }

    mode.dmPelsWidth = settings.Resolution.Width;
    mode.dmPelsHeight = settings.Resolution.Height;

    mode.dmDisplayOrientation = (int)settings.Orientation;
    mode.dmDisplayFrequency = (int)settings.Frequency;

    return mode;
  }
}