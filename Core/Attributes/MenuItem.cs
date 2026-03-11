using System;

namespace ArisenEditorFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MenuItem : System.Attribute
{
    public static readonly string kMenuItemSeparators = "/";

    public string menuItem;
    public bool separator;
    public MenuItem(string itemName, bool separator = false)
    {
        this.menuItem = itemName;
        this.separator = separator;
    }
}
