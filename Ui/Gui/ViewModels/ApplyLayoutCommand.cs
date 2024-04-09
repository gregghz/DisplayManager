using System.Windows.Input;

namespace Gregghz.DisplayManager.UI.Gui.ViewModels;

public class ApplyLayoutCommand(DisplayManager dm) : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public async void Execute(object? parameter)
  {
    if (parameter is string layout) await dm.ApplyConfig(layout);
  }

  public event EventHandler? CanExecuteChanged;
}