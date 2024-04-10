using System.Windows.Input;
using Gregghz.DisplayManager.Services;

namespace Gregghz.DisplayManager.Windows.ViewModels;

public class ApplyLayoutCommand(IDisplayService displayService, ILayoutService layoutService) : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public async void Execute(object? parameter)
  {
    if (parameter is not string layoutName) return;
    var layout = await layoutService.GetLayout(layoutName);
    if (layout is not null) await displayService.ApplyLayout(layout);
    // @TODO: what to do if layout is null?
  }

  public event EventHandler? CanExecuteChanged;
}