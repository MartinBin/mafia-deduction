using System.Windows;
using System.Windows.Controls;

namespace Mafia_client.Prototype;

public class TextBlockPrototype : ICloneable<TextBlock>
{
    private readonly TextBlock _textBlock;

    public TextBlockPrototype(string text, double fontSize, TextAlignment textAlignment, double width)
    {
        _textBlock = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            TextAlignment = textAlignment,
            Width = width
        };
    }

    public TextBlock Clone()
    {
        return new TextBlock
        {
            Text = _textBlock.Text,
            FontSize = _textBlock.FontSize,
            TextAlignment = _textBlock.TextAlignment,
            Width = _textBlock.Width
        };
    }
}