using System.Windows.Input;
using Gregghz.DisplayManager.Services;

namespace Gregghz.DisplayManager.Windows.ViewModels;

public class SaveLayoutCommand(IDisplayService displayService, ILayoutService layoutService) : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public async void Execute(object? parameter)
  {
    if (parameter is not Func<string> getter) return;

    var value = getter();
    var currentLayout = displayService.GetDisplayLayout();
    await layoutService.SaveLayout(value, currentLayout);
    LayoutSaved?.Invoke(this, EventArgs.Empty);
  }

  public event EventHandler? CanExecuteChanged;
  public event EventHandler? LayoutSaved;
}