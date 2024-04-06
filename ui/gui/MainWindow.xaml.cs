using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.UI.Gui;

/// <summary>
///   Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
  private readonly Dictionary<string, string> DeviceMap = new();
  public readonly string Path;

  public MainWindow(string path)
  {
    InitializeComponent();

    Path = path;
    DataContext = this;
    DeviceMap = DisplayManager.GetDeviceMap();
  }

  public ObservableCollection<string> Layouts { get; } = new();
  public ObservableCollection<Settings> SelectedLayout { get; } = new();

  private string SelectedLayoutName { get; set; } = "";

  private async void Window_Loaded(object sender, RoutedEventArgs e)
  {
    await RefreshLayouts();
  }

  private async Task RefreshLayouts()
  {
    Layouts.Clear();
    var enumerable = await DisplayManager.GetSavedLayouts(Path);
    var layouts = enumerable.ToList();

    if (layouts.Count > 0)
    {
      LayoutsListBox.IsEnabled = true;
      LayoutsTextBlock.IsEnabled = false;
      foreach (var layout in layouts)
        Layouts.Add(layout);
    }
    else
    {
      LayoutsListBox.IsEnabled = false;
      LayoutsTextBlock.IsEnabled = true;
    }
  }

  private async void LayoutsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    var selectedItem = LayoutsListBox.SelectedItem;

    if (selectedItem is string layout)
    {
      ApplyButton.IsEnabled = false;
      SelectedLayoutName = layout;
      SelectedLayout.Clear();
      var data = await DisplayManager.LoadLayoutSettings(layout, Path);
      foreach (var setting in data) SelectedLayout.Add(setting);
      DrawLayout(data, LayoutCanvas);
      ApplyButton.IsEnabled = true;
    }
  }

  private void DrawLayout(IList<Settings> layout, Canvas canvas)
  {
    canvas.Children.Clear();

    var canvasHeight = canvas.ActualHeight;
    var canvasWidth = canvas.ActualWidth;

    var scalingFactor = 96.0 / 2;

    var leftMost = layout.MinBy(s => s.Position.X);
    var rightMost = layout.MaxBy(s => s.RightX);

    var topMost = layout.MinBy(s => s.Position.Y);
    var bottomMost = layout.MaxBy(s => s.BottomY);

    var layoutWidth = 0.0;
    var layoutHeight = 0.0;
    if (leftMost is not null && rightMost is not null && topMost is not null && bottomMost is not null)
    {
      layoutWidth = (rightMost.RightX - leftMost.Position.X) / scalingFactor;
      layoutHeight = (bottomMost.BottomY - topMost.Position.Y) / scalingFactor;
    }

    foreach (var settings in layout)
    {
      var offsetX = canvasWidth / 2 - layoutWidth / 2;
      var offsetY = canvasHeight / 2 - layoutHeight / 2;
      var monitorShapes = DrawMonitor(settings, offsetX, offsetY, scalingFactor);
      foreach (var shape in monitorShapes) canvas.Children.Add(shape);
    }
  }

  private FrameworkElement[] DrawMonitor(Settings settings, double offsetX, double offsetY, double scalingFactor)
  {
    var strokeColor = settings.IsPrimary ? Brushes.Indigo : Brushes.Black;
    var rect = new Rectangle
    {
      Stroke = strokeColor,
      Fill = Brushes.Transparent,
      StrokeThickness = 2,
      Width = settings.Resolution.Width / scalingFactor,
      Height = settings.Resolution.Height / scalingFactor,
      ToolTip = DeviceMap[settings.DeviceId]
    };

    var rectLeft = settings.Position.X / scalingFactor + offsetX;
    var rectTop = settings.Position.Y / scalingFactor + offsetY;

    Canvas.SetLeft(rect, rectLeft);
    Canvas.SetTop(rect, rectTop);

    var resolutionTextBlock = DrawResolutionTextBlock(settings, scalingFactor);
    var resolutionTextBlockSize = resolutionTextBlock.DesiredSize;

    // Get the center position of the rectangle.
    var rectCenterX = rectLeft + rect.Width / 2;
    var rectCenterY = rectTop + rect.Height / 2;

    // Calculate the top-left position for the TextBox to make it centered within the Rectangle.
    var textBoxLeft = rectCenterX - resolutionTextBlockSize.Width / 2;
    var textBoxTop = rectCenterY - resolutionTextBlockSize.Height / 2;

    // Apply the calculated position to the TextBox.
    Canvas.SetLeft(resolutionTextBlock, textBoxLeft);
    Canvas.SetTop(resolutionTextBlock, textBoxTop);

    return [rect, resolutionTextBlock];
  }

  private static TextBlock DrawResolutionTextBlock(Settings settings, double scalingFactor, double fontSize = 9)
  {
    var resolutionTextBlock = new TextBlock
    {
      Text = $"{settings.Resolution.Width}x{settings.Resolution.Height}",
      FontSize = fontSize
    };
    resolutionTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    var resolutionTextBlockSize = resolutionTextBlock.DesiredSize;
    if (fontSize > 1 && resolutionTextBlockSize.Width >= settings.Resolution.Width / scalingFactor - 5)
      return DrawResolutionTextBlock(settings, scalingFactor, fontSize - 1);

    return resolutionTextBlock;
  }

  private async void ApplyButton_Click(object sender, RoutedEventArgs e)
  {
    await DisplayManager.ApplyConfig(SelectedLayoutName, Path);
  }

  private void SaveButton_Click(object sender, RoutedEventArgs e)
  {
    SavePopup.IsOpen = true;
    var currentLayout = DisplayManager.GetCurrentLayout();
    DrawLayout(currentLayout.Settings, PreviewCanvas);
    PopupTextBox.Focus();
  }

  private async void PopupSaveButton_Click(object sender, RoutedEventArgs e)
  {
    await DisplayManager.SaveLayout(PopupTextBox.Text, Path);
    await RefreshLayouts();
    SavePopup.IsOpen = false;
  }

  private void PopupCancelButton_Click(object sender, RoutedEventArgs e)
  {
    SavePopup.IsOpen = false;
  }

  private void PopupTextBox_TextChanged(object sender, RoutedEventArgs e)
  {
    PopupSaveButton.IsEnabled = sender is TextBox tb && tb.Text.Length > 0;
  }

  private void PopupTextBox_KeyUp(object sender, KeyEventArgs e)
  {
    if (e.Key == Key.Enter) PopupSaveButton_Click(sender, e);
  }
}