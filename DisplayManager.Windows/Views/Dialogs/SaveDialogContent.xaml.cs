using Gregghz.DisplayManager.Windows.ViewModels;

namespace Gregghz.DisplayManager.Windows.Views.Dialogs;

public sealed partial class SaveDialogContent
{
  public SaveDialogContent()
  {
    InitializeComponent();
    ViewModel = new SaveDialogViewModel(PreviewCanvas);
    DataContext = ViewModel;
  }

  public SaveDialogViewModel ViewModel { get; set; }
}