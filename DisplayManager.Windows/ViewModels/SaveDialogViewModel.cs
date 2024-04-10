using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;

namespace Gregghz.DisplayManager.Windows.ViewModels;

public class SaveDialogViewModel(Canvas canvas) : INotifyPropertyChanged
{
  private string? _layoutName;

  public string? LayoutName
  {
    get => _layoutName;
    set
    {
      if (_layoutName == value) return;
      _layoutName = value;
      OnPropertyChanged();
    }
  }

  public Canvas Canvas
  {
    get => canvas;
    set
    {
      canvas = value;
      OnPropertyChanged();
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

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