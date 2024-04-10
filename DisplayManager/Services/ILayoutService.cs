using Gregghz.DisplayManager.Model;

namespace Gregghz.DisplayManager.Services;

public interface ILayoutService
{
  event EventHandler<Layout>? LayoutLoaded;
  event EventHandler<IList<string>>? LayoutsChanged;

  Task<List<string>> GetSavedLayouts();
  Task<Layout?> GetLayout(string name);
  Task SaveLayout(string name, Layout layout);
}