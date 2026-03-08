using System;
using Avalonia.Controls;
using Avalonia.Data;

namespace ArisenEditorFramework.Inspector;

public class BooleanPropertyEditor : IPropertyEditor
{
    public bool CanHandle(PropertyItemViewModel property) => property.PropertyType == typeof(bool);

    public Control CreateControl(PropertyItemViewModel property)
    {
        var checkBox = new CheckBox { Margin = new Avalonia.Thickness(0) };
        checkBox.Bind(Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty, 
                      new Binding(nameof(PropertyItemViewModel.Value)) { Mode = BindingMode.TwoWay });
        
        var wrapper = new StackPanel { 
            Orientation = Avalonia.Layout.Orientation.Horizontal, 
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center 
        };
        wrapper.Children.Add(checkBox);
        return wrapper;
    }
}

public class NumericPropertyEditor : IPropertyEditor
{
    public bool CanHandle(PropertyItemViewModel property)
    {
        var type = property.PropertyType;
        return type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(long) || type == typeof(decimal);
    }

    public Control CreateControl(PropertyItemViewModel property)
    {
        var type = property.PropertyType;
        var numericUpDown = new NumericUpDown 
        { 
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            FormatString = (type == typeof(int) || type == typeof(long)) ? "0" : "0.00",
            ShowButtonSpinner = true
        };
        
        if (type == typeof(int) || type == typeof(long))
            numericUpDown.Increment = 1m;
        else
            numericUpDown.Increment = 0.1m;

        numericUpDown.Bind(NumericUpDown.ValueProperty, 
                           new Binding(nameof(PropertyItemViewModel.Value)) 
                           { 
                               Mode = BindingMode.TwoWay, 
                               Converter = new DecimalObjectConverter(type) 
                           });
                           
        return numericUpDown;
    }
}

public class EnumPropertyEditor : IPropertyEditor
{
    public bool CanHandle(PropertyItemViewModel property) => property.PropertyType.IsEnum && !Attribute.IsDefined(property.PropertyType, typeof(FlagsAttribute));

    public Control CreateControl(PropertyItemViewModel property)
    {
        var comboBox = new ComboBox 
        { 
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = Enum.GetValues(property.PropertyType)
        };
        comboBox.Bind(ComboBox.SelectedItemProperty, 
                      new Binding(nameof(PropertyItemViewModel.Value)) { Mode = BindingMode.TwoWay });
        return comboBox;
    }
}
