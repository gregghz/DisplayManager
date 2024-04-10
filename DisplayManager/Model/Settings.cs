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