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

        var mainLayout = new ProportionalDock
        {
            Id = "MainLayout",
            Orientation = Orientation.Horizontal,
            ActiveDockable = viewport,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    Id = "LeftPane",
                    ActiveDockable = hierarchy,
                    VisibleDockables = CreateList<IDockable>(hierarchy),
                    Proportion = 0.2,
                    IsCollapsable = false
                },
                new ProportionalDockSplitter(),
                new ToolDock
                {
                    Id = "CenterPane",
                    ActiveDockable = viewport,
                    VisibleDockables = CreateList<IDockable>(viewport),
                    Proportion = 0.6,
                    IsCollapsable = false
                },
                new ProportionalDockSplitter(),
                new ToolDock
                {
                    Id = "RightPane",
                    ActiveDockable = inspector,
                    VisibleDockables = CreateList<IDockable>(inspector),
                    Proportion = 0.2,
                    IsCollapsable = false
                }
            )
        };

        var bottomPane = new ToolDock
        {
            Id = "BottomPane",
            ActiveDockable = console,
            VisibleDockables = CreateList<IDockable>(console),
            Proportion = 0.25,
            IsCollapsable = false
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
                    ActiveDockable = toolbar,
                    VisibleDockables = CreateList<IDockable>(toolbar),
                    Proportion = 0.05,
                    IsCollapsable = false
                },
                new ProportionalDockSplitter(),
                mainLayout,
                new ProportionalDockSplitter(),
                bottomPane
            )
        };

        var rootDock = CreateRootDock();
        rootDock.Id = "RootDock";
        rootDock.Title = "RootDock";
        rootDock.ActiveDockable = windowLayout;
        rootDock.DefaultDockable = windowLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);

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
