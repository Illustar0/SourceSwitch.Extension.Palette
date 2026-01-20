using System.ComponentModel.DataAnnotations;
using CsToml;

namespace SourceSwitch.Extension.Palette.Helpers;

[TomlSerializedObject]
public partial class InputSourceModel
{
    [Required]
    [TomlValueOnSerialized]
    public List<InputSourceProfile> Profiles { get; set; } = new();
}

[TomlSerializedObject]
public partial class InputSourceProfile
{
    [TomlValueOnSerialized(NullHandling = TomlNullHandling.Ignore)]
    public string LightIcon { get; set; } = "SegoeFluentIcon://&#xE8A7;";

    [TomlValueOnSerialized(NullHandling = TomlNullHandling.Ignore)]
    public string DarkIcon { get; set; } = "SegoeFluentIcon://&#xE8A7;";

    [Required(ErrorMessage = "The \"title\" field is required.")]
    [StringLength(
        int.MaxValue,
        MinimumLength = 1,
        ErrorMessage = "\"title\" cannot be an empty string."
    )]
    [TomlValueOnSerialized]
    public string Title { get; set; } = "";

    [TomlValueOnSerialized]
    public bool TopLevel { get; set; } = false;

    [Required(ErrorMessage = "The \"vcpValue\" field is required.")]
    [RegularExpression(
        "^[0-9A-Fa-f]{1,2}$",
        ErrorMessage = "\"vcpValue\" must be a 1- or 2-digit hexadecimal string (for example, '0F' or '11')."
    )]
    [TomlValueOnSerialized]
    public string VcpValue { get; set; } = "";
}
