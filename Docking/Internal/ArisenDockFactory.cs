using System;
using System.Collections.Generic;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using Dock.Model.Mvvm.Core;
using Dock.Model.Mvvm;
using Avalonia.Collections;

namespace ArisenEditorFramework.Docking.Internal;

/// <summary>
/// Internal factory used by Ava.Dock to build the layout tree.
/// </summary>
internal class ArisenDockFactory : Factory
{
    private IRootDock? _rootDock;
    private IDocumentDock? _documentDock;
    private readonly LayoutManager _layoutManager;

    public ArisenDockFactory(LayoutManager layoutManager)
    {
        _layoutManager = layoutManager;
    }

    public override IRootDock CreateLayout()
    {
        var viewport = new DocumentDocument { Id = "Viewport", Title = "Viewport" };
        var hierarchy = new ToolDocument { Id = "Hierarchy", Title = "Hierarchy" };
        var inspector = new ToolDocument { Id = "Inspector", Title = "Inspector" };
        var console = new ToolDocument { Id = "Console", Title = "Console" };

        var mainLayout = new ProportionalDock
        {
            Id = "MainLayout",
            Orientation = Orientation.Horizontal,
            ActiveDockable = viewport,
            VisibleDockables = CreateList<IDockable>
            (
                new ProportionalDock
                {
                    Id = "LeftPane",
                    Orientation = Orientation.Vertical,
                    ActiveDockable = hierarchy,
                    VisibleDockables = CreateList<IDockable>(hierarchy),
                    Proportion = 0.2
                },
                new ProportionalDockSplitter(),
                _documentDock = new DocumentDock
                {
                    Id = "DocumentsPane",
                    ActiveDockable = viewport,
                    VisibleDockables = CreateList<IDockable>(viewport),
                    Proportion = 0.6
                },
                new ProportionalDockSplitter(),
                new ProportionalDock
                {
                    Id = "RightPane",
                    Orientation = Orientation.Vertical,
                    ActiveDockable = inspector,
                    VisibleDockables = CreateList<IDockable>(inspector),
                    Proportion = 0.2
                }
            )
        };

        var rootDock = CreateRootDock();

        rootDock.Id = "RootDock";
        rootDock.Title = "RootDock";
        rootDock.ActiveDockable = mainLayout;
        rootDock.DefaultDockable = mainLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(mainLayout);

        var bottomPane = new ProportionalDock
        {
            Id = "BottomPane",
            Orientation = Orientation.Horizontal,
            ActiveDockable = console,
            VisibleDockables = CreateList<IDockable>(console),
            Proportion = 0.25
        };

        var windowLayout = new ProportionalDock
        {
            Id = "WindowLayout",
            Orientation = Orientation.Vertical,
            ActiveDockable = mainLayout,
            VisibleDockables = CreateList<IDockable>
            (
                mainLayout,
                new ProportionalDockSplitter(),
                bottomPane
            )
        };

        rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
        rootDock.ActiveDockable = windowLayout;
        rootDock.DefaultDockable = windowLayout;

        _rootDock = rootDock;
        
        return rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>
        {
            // Context locator maps window IDs to actual contents.
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }
}

internal class DocumentDocument : Document
{
}

internal class ToolDocument : Tool
{
}
