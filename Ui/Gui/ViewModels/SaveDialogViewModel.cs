using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;

namespace Gregghz.DisplayManager.UI.Gui.ViewModels;

public class SaveDialogViewModel : INotifyPropertyChanged
{
  private Canvas _canvas;
  private TextBox _textBox;

  public Canvas Canvas
  {
    get => _canvas;
    set
    {
      _canvas = value;
      OnPropertyChanged();
    }
  }

  public TextBox TextBox
  {
    get => _textBox;
    set
    {
      _textBox = value;
      OnPropertyChanged();
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  public GetLayoutInput GetPopupTextValue()
  {
    return () => _textBox.Text;
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