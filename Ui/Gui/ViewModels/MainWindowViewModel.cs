using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.UI.Gui.ViewModels;

public class MainWindowViewModel(DisplayManager dm) : INotifyPropertyChanged
{
  public readonly ICommand ApplyLayoutCommand = new ApplyLayoutCommand(dm);
  public readonly OpenSavePopupCommand OpenSavePopupCommand = new(dm);
  public readonly ICommand SaveLayout = new SaveLayoutCommand(dm);
  private bool _applyEnabled;
  private string? _selectedLayout;

  public bool ApplyEnabled
  {
    get => _applyEnabled;
    set
    {
      if (_applyEnabled != value)
      {
        _applyEnabled = value;
        OnPropertyChanged();
      }
    }
  }

  public string? SelectedLayout
  {
    get => _selectedLayout;
    set
    {
      if (_selectedLayout != value)
      {
        _selectedLayout = value;
        ApplyEnabled = value is not null;
        if (value is not null)
        {
          var _ = dm.LoadLayoutSettings(value);
        }

        OnPropertyChanged();
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  public void RefreshLayouts()
  {
    var _ = dm.GetSavedLayouts();
  }

  public void SubscribeToSaveDialogRequested(EventHandler<Layout> handler)
  {
    OpenSavePopupCommand.SubscribeToSaveDialogRequest(handler);
  }

  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
  {
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    OnPropertyChanged(propertyName);
    return true;
  }
}