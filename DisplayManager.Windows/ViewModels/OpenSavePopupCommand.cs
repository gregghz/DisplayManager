using System.Windows.Input;
using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.Services;

namespace Gregghz.DisplayManager.Windows.ViewModels;

public class OpenSavePopupCommand(IDisplayService displayService) : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public void Execute(object? parameter)
  {
    var currentLayout = displayService.GetDisplayLayout();
    SaveDialogRequested?.Invoke(this, currentLayout);
  }

  public event EventHandler? CanExecuteChanged;

  public void SubscribeToSaveDialogRequest(EventHandler<Layout> f)
  {
    SaveDialogRequested += f;
  }

  public event EventHandler<Layout>? SaveDialogRequested;
}