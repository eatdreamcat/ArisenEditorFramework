using System;
using System.ComponentModel;
using System.Reflection;
using ReactiveUI;

namespace ArisenEditorFramework.Inspector;

/// <summary>
/// Represents a single editable property discovered via reflection.
/// Binds to a specific PropertyInfo of a given Target object.
/// Implements IDisposable to clean up event subscriptions and prevent memory leaks.
/// </summary>
public class PropertyItemViewModel : ReactiveObject, IDisposable
{
    private readonly PropertyInfo _propertyInfo;
    private readonly object _target;
    private readonly PropertyChangedEventHandler? _targetPropertyChangedHandler;
    private bool _disposed;

    public string PropertyName { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public string Category { get; }
    public Type PropertyType { get; }
    
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets or sets the value of the property on the underlying object.
    /// Notifies the UI when changed.
    /// </summary>
    public object? Value
    {
        get => _propertyInfo.GetValue(_target);
        set
        {
            if (!IsReadOnly)
            {
                // Simple attempt to convert if needed, e.g., string from a TextBox to numeric
                object? convertedValue = value;
                if (value != null && PropertyType != value.GetType())
                {
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(PropertyType);
                        if (converter.CanConvertFrom(value.GetType()))
                        {
                            convertedValue = converter.ConvertFrom(value);
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value, PropertyType);
                        }
                    }
                    catch
                    {
                        // Ignore conversion errors and just return (or log in a real system)
                        return;
                    }
                }

                _propertyInfo.SetValue(_target, convertedValue);
                this.RaisePropertyChanged(nameof(Value));
            }
        }
    }

    public PropertyItemViewModel(object target, PropertyInfo propertyInfo)
    {
        _target = target;
        _propertyInfo = propertyInfo;
        
        PropertyName = _propertyInfo.Name;
        PropertyType = _propertyInfo.PropertyType;
        IsReadOnly = !_propertyInfo.CanWrite;

        // Default metadata
        DisplayName = PropertyName;
        Description = string.Empty;
        Category = "Misc";

        // Read attributes for metadata
        var browsableAttributes = _propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), true);
        if (browsableAttributes.Length > 0 && browsableAttributes[0] is BrowsableAttribute browsable)
        {
            if (!browsable.Browsable)
            {
                // This property wouldn't normally be here if browsable is false, handled by the parent
            }
        }

        var displayAttributes = _propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
        if (displayAttributes.Length > 0 && displayAttributes[0] is DisplayNameAttribute display)
        {
            DisplayName = display.DisplayName;
        }

        var descriptionAttributes = _propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
        if (descriptionAttributes.Length > 0 && descriptionAttributes[0] is DescriptionAttribute desc)
        {
            Description = desc.Description;
        }

        var categoryAttributes = _propertyInfo.GetCustomAttributes(typeof(CategoryAttribute), true);
        if (categoryAttributes.Length > 0 && categoryAttributes[0] is CategoryAttribute cat)
        {
            Category = cat.Category;
        }
        
        // Subscribe to target's PropertyChanged using a stored handler so we can unsubscribe later.
        if (_target is INotifyPropertyChanged npc)
        {
             _targetPropertyChangedHandler = (s, e) => {
                 if (e.PropertyName == PropertyName)
                 {
                     this.RaisePropertyChanged(nameof(Value));
                 }
             };
             npc.PropertyChanged += _targetPropertyChangedHandler;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_targetPropertyChangedHandler != null && _target is INotifyPropertyChanged npc)
        {
            npc.PropertyChanged -= _targetPropertyChangedHandler;
        }
    }
}

