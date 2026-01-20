using Microsoft.CommandPalette.Extensions.Toolkit;

namespace SourceSwitch.Extension.Palette.Helpers;

public interface ISettingsInterface
{
    public InputSourceModel InputSourceOrder { get; }
    public Settings Settings { get; }
}
