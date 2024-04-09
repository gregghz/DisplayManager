using System.Collections.ObjectModel;
using Windows.Graphics;
using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.UI.Gui.ViewModels;
using Gregghz.DisplayManager.UI.Gui.Views.Dialogs;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using WinRT.Interop;
using Layout = Gregghz.DisplayManager.Model.Layout;
using Size = Windows.Foundation.Size;

namespace Gregghz.DisplayManager.Windows.Views;

/// <summary>
///   Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private readonly Dictionary<string, string> _deviceMap;
  private readonly DisplayManager _displayManager;

  private bool _firstLoad = true;

  public MainWindow(DisplayManager displayManager)
  {
    InitializeComponent();

    ViewModel = new MainWindowViewModel(displayManager);
    _displayManager = displayManager;
    _deviceMap = _displayManager.GetDeviceMap();
    var appWindow = GetAppWindowForCurrentWindow();
    appWindow.Resize(new SizeInt32(825, 600));

    _displayManager.LayoutsUpdated += (_, layouts) => { RefreshLayouts(layouts); };
    _displayManager.LayoutLoaded += OnLayoutLoad;

    ViewModel.SubscribeToSaveDialogRequested(DrawSaveDialog);
  }

  public MainWindowViewModel ViewModel { get; set; }

  public ObservableCollection<string> Layouts { get; } = new();

  private AppWindow GetAppWindowForCurrentWindow()
  {
    var hWnd = WindowNative.GetWindowHandle(this);
    var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
    return AppWindow.GetFromWindowId(windowId);
  }

  private async void Window_Activated(object sender, WindowActivatedEventArgs e)
  {
    if (_firstLoad)
    {
      _firstLoad = false;

      _displayManager.FireLayoutUpdated();
    }
  }

  private void RefreshLayouts(IList<string> layouts)
  {
    Layouts.Clear();

    if (layouts.Count > 0)
    {
      LayoutsListBox.IsEnabled = true;
      LayoutsTextBlock.Visibility = Visibility.Collapsed;
      foreach (var layout in layouts)
        Layouts.Add(layout);
    }
    else
    {
      LayoutsListBox.IsEnabled = false;
      LayoutsTextBlock.Visibility = Visibility.Visible;
    }
  }

  private void OnLayoutLoad(object? sender, IList<Settings> data)
  {
    DrawLayout(data, LayoutCanvas);
  }

  private void DrawLayout(IList<Settings> layout, Canvas canvas, double? scale = null)
  {
    canvas.Children.Clear();

    var canvasHeight = canvas.ActualHeight;
    var canvasWidth = canvas.ActualWidth;
    if (canvasHeight == 0 || canvasWidth == 0)
    {
      canvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      canvasHeight = canvas.DesiredSize.Height;
      canvasWidth = canvas.DesiredSize.Width;
    }

    var scalingFactor = 96.0 / 2;
    if (scale is not null) scalingFactor = scale.Value;

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
    var strokeColor = settings.IsPrimary ? Colors.Indigo : Colors.Black;
    var rect = new Rectangle
    {
      Stroke = new SolidColorBrush(strokeColor),
      Fill = new SolidColorBrush(Colors.Transparent),
      StrokeThickness = 2,
      Width = settings.Resolution.Width / scalingFactor,
      Height = settings.Resolution.Height / scalingFactor
    };
    var toolTip = new ToolTip
    {
      Content = _deviceMap[settings.DeviceId]
    };
    ToolTipService.SetToolTip(rect, toolTip);

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

  private async void DrawSaveDialog(object? sender, Layout currentLayout)
  {
    var content = new SaveDialogContent();
    var cd = new ContentDialog
    {
      XamlRoot = RootGrid.XamlRoot,
      Width = 300,
      Height = 500,
      Content = content,
      CloseButtonText = "Cancel",
      PrimaryButtonText = "Save",
      PrimaryButtonCommand = ViewModel.SaveLayout,
      PrimaryButtonCommandParameter = content.ViewModel.GetPopupTextValue()
    };
    DrawLayout(currentLayout.Settings, content.ViewModel.Canvas, 96.0 / 1.5);
    await cd.ShowAsync();
  }
}