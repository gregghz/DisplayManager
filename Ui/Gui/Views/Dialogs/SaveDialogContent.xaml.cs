using Windows.System;
using Gregghz.DisplayManager.UI.Gui.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Gregghz.DisplayManager.UI.Gui.Views.Dialogs;

public sealed partial class SaveDialogContent
{
  public SaveDialogContent()
  {
    InitializeComponent();
    ViewModel = new SaveDialogViewModel
    {
      Canvas = PreviewCanvas,
      TextBox = PopupTextBox
    };
    DataContext = ViewModel;
  }

  public SaveDialogViewModel ViewModel { get; set; }

  private void PopupTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
  {
    if (e.Key == VirtualKey.Enter)
    {
    }
  }

  private void PopupTextBox_TextChanged(object sender, TextChangedEventArgs e)
  {
  }
}