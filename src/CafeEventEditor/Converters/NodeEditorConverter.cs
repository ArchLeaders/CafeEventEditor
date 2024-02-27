using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.NodeEditor.Core;
using System.Globalization;

namespace CafeEventEditor.Converters;

public class NodeEditorConverter : IValueConverter
{
    public static NodeEditorConverter Shared { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is INode node && node.Content is UserControl control) {
            return control.DataTemplates[0];
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
