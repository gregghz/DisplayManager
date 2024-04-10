using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gregghz.DisplayManager.Model;
using Gregghz.DisplayManager.Services;

namespace Gregghz.DisplayManager.Windows.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
  private readonly ILayoutService _layoutService;
  public readonly ICommand ApplyLayoutCommand;
  public readonly OpenSavePopupCommand OpenSavePopupCommand;
  public readonly SaveLayoutCommand SaveLayoutCommand;
  private bool _applyEnabled;
  private string? _selectedLayout;

  public MainWindowViewModel(IDisplayService displayService, ILayoutService layoutService)
  {
    _layoutService = layoutService;
    ApplyLayoutCommand = new ApplyLayoutCommand(displayService, layoutService);
    OpenSavePopupCommand = new OpenSavePopupCommand(displayService);
    SaveLayoutCommand = new SaveLayoutCommand(displayService, layoutService);

    Layouts.CollectionChanged += (_, args) =>
    {
      var newCount = args.NewItems?.Count ?? 0;
      var oldCount = args.OldItems?.Count ?? 0;
      if (newCount != oldCount) LayoutCountChanged?.Invoke(this, newCount);
    };

    PopulateLayouts();
  }

  public ObservableCollection<string> Layouts { get; } = new();

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
        TriggerLayoutSelected();

        OnPropertyChanged();
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private async void PopulateLayouts()
  {
    var layouts = await _layoutService.GetSavedLayouts();
    foreach (var layout in layouts) Layouts.Add(layout);
  }

  private async void TriggerLayoutSelected()
  {
    if (_selectedLayout is null) return;
    var layout = await _layoutService.GetLayout(_selectedLayout);
    if (layout is not null) LayoutSelected?.Invoke(this, layout);
  }

  public void SubscribeToLayoutCountChanged(Action<int> onChanged)
  {
    LayoutCountChanged += (_, count) => onChanged(count);
    onChanged(Layouts.Count);
  }

  public void SubscribeToLayoutSelected(Action<Layout> onSelected)
  {
    LayoutSelected += (_, layout) => onSelected(layout);
  }

  public void SubscribeToLayoutSaved(Action<IList<string>> onSaved)
  {
    SaveLayoutCommand.LayoutSaved += async (_, _) =>
    {
      var layouts = await _layoutService.GetSavedLayouts();
      onSaved(layouts);
    };
  }

  public event EventHandler<int>? LayoutCountChanged;
  public event EventHandler<Layout>? LayoutSelected;

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