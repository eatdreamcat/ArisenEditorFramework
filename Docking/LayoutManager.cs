using System.Collections.Generic;
using System.Linq;
using ArisenEditorFramework.Docking.Internal;
using Avalonia.Collections;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using Dock.Serializer;

namespace ArisenEditorFramework.Docking;

public class LayoutManager : IEditorLayoutService
{
    private readonly ArisenDockFactory _factory;
    private IRootDock? _layout;
    private readonly Dictionary<string, IEditorWindow> _customWindows = new();

    public IFactory Factory => _factory;
    public IRootDock? Layout => _layout;

    public LayoutManager()
    {
        _factory = new ArisenDockFactory(this);
    }

    public void Initialize()
    {
        _layout = _factory.CreateLayout();
        if (_layout != null)
        {
            _factory.InitLayout(_layout);
        }
    }

    public void OpenWindow(IEditorWindow window)
    {
        _customWindows[window.Id] = window;
        
        // Find documents pane or root to add
        var docPane = _layout != null ? _factory.FindDockable(_layout, v => v is IDocumentDock) as IDocumentDock : null;
        if (docPane != null)
        {
            var document = new EditorWindowDocument(window)
            {
                Id = window.Id,
                Title = window.Title
            };
            
            _factory.AddDockable(docPane, document);
            _factory.SetActiveDockable(document);
            _factory.SetFocusedDockable(docPane, document);
        }
    }

    public void CloseWindow(IEditorWindow window)
    {
        if (_customWindows.Remove(window.Id))
        {
            var dockable = _layout != null ? _factory.FindDockable(_layout, v => v.Id == window.Id) : null;
            if (dockable != null && dockable.Owner is IDock parentDock)
            {
                _factory.RemoveDockable(dockable, true);
            }
        }
    }

    public string SaveLayout()
    {
        if (_layout == null) return string.Empty;
        var serializer = new DockSerializer(typeof(AvaloniaList<IDockable>));
        return serializer.Serialize(_layout);
    }

    public void LoadLayout(string layoutData)
    {
        if (string.IsNullOrEmpty(layoutData)) return;
        
        var serializer = new DockSerializer(typeof(AvaloniaList<IDockable>));
        var newLayout = serializer.Deserialize<IRootDock>(layoutData);
        if (newLayout != null)
        {
            _layout = newLayout;
            _factory.InitLayout(_layout);
        }
    }
    
    public Dictionary<string, string> SerializeCustomWindows()
    {
        return _customWindows.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.SerializeState());
    }

    public void RestoreCustomWindows(IEnumerable<IEditorWindow> newInstances, Dictionary<string, string> stateData)
    {
        _customWindows.Clear();
        foreach (var window in newInstances)
        {
            if (stateData.TryGetValue(window.Id, out var state))
            {
                window.DeserializeState(state);
            }
            _customWindows[window.Id] = window;
            
            // Re-bind to existing Ava.Dock view models built from loaded JSON
            var existingView = _layout != null ? _factory.FindDockable(_layout, v => v is EditorWindowDocument d && d.Id == window.Id) as EditorWindowDocument : null;
            if (existingView != null)
            {
                existingView.SetWindow(window);
            }
        }
    }
}

internal class EditorWindowDocument : Document
{
    private IEditorWindow? _window;
    public object? WindowContent => _window?.GetContent();

    public EditorWindowDocument(IEditorWindow window)
    {
        _window = window;
    }
    
    // Parameterless constructor needed for deserialization
    public EditorWindowDocument() { }
    
    public void SetWindow(IEditorWindow window)
    {
        _window = window;
        // Raise PropertyChanged when content changes if we implement INotifyPropertyChanged
        // but for now simple binding works for static content wrappers or reactive wrappers.
    }
}
