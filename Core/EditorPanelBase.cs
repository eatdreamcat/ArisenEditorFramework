using System;
using Avalonia.Controls;

namespace ArisenEditorFramework.Core;

/// <summary>
/// A base class for editor panels that provides standard windowing behavior.
/// </summary>
public abstract class EditorPanelBase : IEditorPanel
{
    public abstract string Title { get; }
    public abstract string Id { get; }
    public abstract object Content { get; }

    protected void OnClosing()
    {
        // Cleanup logic if needed
    }
}
