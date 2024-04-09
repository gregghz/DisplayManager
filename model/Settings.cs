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
  public string DeviceId { get; set; } = DeviceId;
  public bool IsPrimary { get; set; } = IsPrimary;
  public Position Position { get; set; } = Position;
  public Resolution Resolution { get; set; } = Resolution;
  public Orientation Orientation { get; set; } = Orientation;
  public uint Frequency { get; set; } = Frequency;

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

  public void Deconstruct(out string DeviceId, out bool IsPrimary, out Position Position, out Resolution Resolution,
    out Orientation Orientation, out uint Frequency)
  {
    DeviceId = this.DeviceId;
    IsPrimary = this.IsPrimary;
    Position = this.Position;
    Resolution = this.Resolution;
    Orientation = this.Orientation;
    Frequency = this.Frequency;
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