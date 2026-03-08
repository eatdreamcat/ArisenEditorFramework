using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using ArisenEditorFramework.Inspector;

namespace ArisenEditorFramework.Inspector;

/// <summary>
/// Dynamically selects an Avalonia Control template based on the underlying PropertyType
/// of a PropertyItemViewModel instance.
/// </summary>
public class PropertyDataTemplateSelector : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is not PropertyItemViewModel prop)
            return new TextBlock { Text = "Unsupported Type" };

        var type = prop.PropertyType;

        // Boolean
        if (type == typeof(bool))
        {
            var checkBox = new CheckBox { Margin = new Avalonia.Thickness(0) };
            checkBox.Bind(Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty, 
                          new Binding(nameof(PropertyItemViewModel.Value)) { Mode = BindingMode.TwoWay });
            var wrapper = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            wrapper.Children.Add(checkBox);
            return wrapper;
        }

        // Numeric Inputs (int, float, double)
        if (type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(long))
        {
            var numericUpDown = new NumericUpDown 
            { 
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FormatString = type == typeof(int) || type == typeof(long) ? "0" : "0.00",
                ShowButtonSpinner = true
            };
            
            // To properly bind diff types to a decimal NumericUpDown we usually need a converter.
            // For simplicity in this generic one, we bind Value, and let the string conv do work.
            if (type == typeof(int) || type == typeof(long))
            {
               numericUpDown.Increment = 1m;
            }
            else
            {
               numericUpDown.Increment = 0.1m;
            }

            // Using string binding for loose typing
            numericUpDown.Bind(NumericUpDown.ValueProperty, 
                               new Binding(nameof(PropertyItemViewModel.Value)) 
                               { 
                                   Mode = BindingMode.TwoWay, 
                                   Converter = new DecimalObjectConverter(type) 
                               });
                               
            return numericUpDown;
        }

        // Enum
        if (type.IsEnum)
        {
            var comboBox = new ComboBox 
            { 
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ItemsSource = Enum.GetValues(type)
            };
            comboBox.Bind(ComboBox.SelectedItemProperty, 
                          new Binding(nameof(PropertyItemViewModel.Value)) { Mode = BindingMode.TwoWay });
            return comboBox;
        }

        // Default to a TextBox string representation for everything else (string, Vector3 struct text representation, etc.)
        var textBox = new TextBox 
        { 
            HorizontalAlignment = HorizontalAlignment.Stretch,
            AcceptsReturn = false
        };
        textBox.Bind(TextBox.TextProperty, 
                     new Binding(nameof(PropertyItemViewModel.Value)) { Mode = BindingMode.TwoWay });
        return textBox;
    }

    public bool Match(object? data)
    {
        return data is PropertyItemViewModel;
    }
}

public class DecimalObjectConverter : IValueConverter
{
    private Type _targetBaseType;
    public DecimalObjectConverter(Type targetBaseType) { _targetBaseType = targetBaseType; }

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) return null;
        try { return System.Convert.ToDecimal(value); } catch { return 0m; }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) return null;
        try { return System.Convert.ChangeType(value, _targetBaseType); } catch { return null; }
    }
}
