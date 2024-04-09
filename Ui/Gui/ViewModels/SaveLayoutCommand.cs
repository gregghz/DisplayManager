using System.Windows.Input;

namespace Gregghz.DisplayManager.UI.Gui.ViewModels;

public delegate string GetLayoutInput();

public class SaveLayoutCommand(DisplayManager displayManager) : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public async void Execute(object? parameter)
  {
    if (parameter is GetLayoutInput getter)
    {
      var value = getter();
      await displayManager.SaveLayout(value);
    }
  }

  public event EventHandler? CanExecuteChanged;
}