using System.Windows.Input;
using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.UI.Gui.ViewModels;

public class OpenSavePopupCommand(DisplayManager dm) : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public void Execute(object? parameter)
  {
    var currentLayout = dm.GetCurrentLayout();
    SaveDialogRequested?.Invoke(this, currentLayout);
  }

  public event EventHandler? CanExecuteChanged;

  public void SubscribeToSaveDialogRequest(EventHandler<Layout> f)
  {
    SaveDialogRequested += f;
  }

  public event EventHandler<Layout>? SaveDialogRequested;
}