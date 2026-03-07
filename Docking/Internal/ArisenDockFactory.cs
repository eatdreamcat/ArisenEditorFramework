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
    private readonly LayoutManager _layoutManager;

    public ArisenDockFactory(LayoutManager layoutManager)
    {
        _layoutManager = layoutManager;
    }

    public override IRootDock CreateLayout()
    {
        var toolbar = new ToolDocument { Id = "Toolbar", Title = "Toolbar" };
        var viewport = new ToolDocument { Id = "Viewport", Title = "Viewport" };
        var hierarchy = new ToolDocument { Id = "Hierarchy", Title = "Hierarchy" };
        var inspector = new ToolDocument { Id = "Inspector", Title = "Inspector" };
        var console = new ToolDocument { Id = "Console", Title = "Console" };

        // Unified layout using ProportionalDocks and ToolDocks.
        // We allow collapse (default) to support space fulfillment like Unity.
        
        var mainLayout = new ProportionalDock
        {
            Id = "MainLayout",
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    Id = "LeftPane",
                    Proportion = 0.2,
                    ActiveDockable = hierarchy,
                    VisibleDockables = CreateList<IDockable>(hierarchy)
                },
                new ProportionalDockSplitter(),
                new ToolDock
                {
                    Id = "CenterPane",
                    Proportion = 0.6,
                    ActiveDockable = viewport,
                    VisibleDockables = CreateList<IDockable>(viewport)
                },
                new ProportionalDockSplitter(),
                new ToolDock
                {
                    Id = "RightPane",
                    Proportion = 0.2,
                    ActiveDockable = inspector,
                    VisibleDockables = CreateList<IDockable>(inspector)
                }
            )
        };

        var windowLayout = new ProportionalDock
        {
            Id = "WindowLayout",
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    Id = "ToolbarPane",
                    Proportion = 0.05,
                    ActiveDockable = toolbar,
                    VisibleDockables = CreateList<IDockable>(toolbar)
                },
                new ProportionalDockSplitter(),
                mainLayout,
                new ProportionalDockSplitter(),
                new ToolDock
                {
                    Id = "BottomPane",
                    Proportion = 0.25,
                    ActiveDockable = console,
                    VisibleDockables = CreateList<IDockable>(console)
                }
            )
        };

        var rootDock = CreateRootDock();
        rootDock.Id = "RootDock";
        rootDock.ActiveDockable = windowLayout;
        rootDock.DefaultDockable = windowLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
        
        // The RootDock itself must NOT collapse so the window remains a valid drop target.
        rootDock.IsCollapsable = false;

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
    public ToolDocument()
    {
        CanFloat = true;
        CanClose = true;
        CanPin = true;
    }
}
