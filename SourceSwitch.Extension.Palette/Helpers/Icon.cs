using Microsoft.CommandPalette.Extensions.Toolkit;

namespace SourceSwitch.Extension.Palette.Helpers;

public static class IconFactory
{
    private const string FallbackGlyph = "\uE8A7";

    private static readonly Dictionary<string, string> ExtendIconMap = new()
    {
        { "Apple-Dark", "Assets/icons/Apple.svg" },
    };

    public static IconData CreateIcon(string? iconString)
    {
        if (string.IsNullOrEmpty(iconString))
        {
            return new IconData(FallbackGlyph);
        }

        if (!Uri.TryCreate(iconString, UriKind.Absolute, out var iconUri))
        {
            if (File.Exists(iconString))
            {
                return new IconData(iconString);
            }
            return new IconData(FallbackGlyph);
        }

        switch (iconUri.Scheme.ToLowerInvariant())
        {
            case "segoefluenticon":
                string glyph = iconUri.Host.ToLowerInvariant();
                return new IconData(glyph);

            case "extendicon":
                string key = iconUri.Host.ToLowerInvariant();
                if (ExtendIconMap.TryGetValue(key, out var relativePath))
                {
                    return new IconData(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath)
                    );
                }
                break;

            case "file":
                return new IconData(iconUri.ToString());

            case "http":
            case "https":
                return new IconData(iconUri.ToString());
        }

        return new IconData(FallbackGlyph);
    }
}
