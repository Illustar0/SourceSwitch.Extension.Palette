using System.Text;
using CsToml;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SourceSwitch.Core.Capabilities;
using SourceSwitch.Core.Display;
using SourceSwitch.Extension.Palette.Properties;

namespace SourceSwitch.Extension.Palette.Helpers;

public sealed class SettingsManager : JsonSettingsManager, ISettingsInterface
{
    private static readonly CsTomlSerializerOptions TomlOptions =
        CsTomlSerializerOptions.Default with
        {
            SerializeOptions = new SerializeOptions { ArrayStyle = TomlArrayStyle.Header },
        };

    // VCP 60 输入源值映射表 (十六进制值 -> 显示名称)
    private static readonly Dictionary<string, string> InputMappings = new()
    {
        { "01", "VGA-1" },
        { "02", "VGA-2" },
        { "03", "DVI-1" },
        { "04", "DVI-2" },
        { "05", "Composite video 1" },
        { "06", "Composite video 2" },
        { "07", "S-Video-1" },
        { "08", "S-Video-2" },
        { "09", "Tuner-1" },
        { "0A", "Tuner-2" },
        { "0B", "Tuner-3" },
        { "0C", "Component video (YPrPb/YCrCb) 1" },
        { "0D", "Component video (YPrPb/YCrCb) 2" },
        { "0E", "Component video (YPrPb/YCrCb) 3" },
        { "0F", "DisplayPort-1" },
        { "10", "DisplayPort-2" },
        { "11", "HDMI-1" },
        { "12", "HDMI-2" },
        { "1B", "USB-C" },
    };

    private const string Namespace = "SourceSwitch";

    private static string Namespaced(string propertyName) => $"{Namespace}.{propertyName}";

    private readonly TextSetting _inputSourceOrder = new(
        Namespaced(nameof(InputSourceOrder)),
        Resources.SettingsManager__inputSourceOrder_Input_source_configuration,
        Resources.SettingsManager__inputSourceOrder_Adjust_the_input_source_configuration__Use_TOML_format_,
        BuildValue()
    )
    {
        Multiline = true,
    };

    public InputSourceModel InputSourceOrder
    {
        get
        {
            var normalizedValue = (_inputSourceOrder.Value ?? "")
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            if (string.IsNullOrEmpty(normalizedValue))
            {
                return new InputSourceModel();
            }

            var utf8Bytes = Encoding.UTF8.GetBytes(normalizedValue);
            return CsTomlSerializer.Deserialize<InputSourceModel>(utf8Bytes, TomlOptions);
        }
    }

    private static InputSourceProfile CreateProfile(string title, string vcpValue) =>
        new() { Title = title, VcpValue = vcpValue };

    private static string BuildValue()
    {
        var model = new InputSourceModel();

        try
        {
            MonitorCapabilities caps = CapabilityParser.Parse(
                DisplayMonitors
                    .GetCurrentMonitor()
                    .WithPhysicalMonitor(0, monitor => monitor.GetCapabilitiesString())
            );

            if (caps.VcpFeatures.TryGetValue("60", out MonitorVcpFeature? inputSource))
            {
                foreach (var val in inputSource.SupportedValues)
                {
                    // 尝试获取显示名称，如果找不到就使用十六进制值
                    var displayName = InputMappings.TryGetValue(val, out string? name)
                        ? name
                        : $"Unknown ({val})";

                    model.Profiles.Add(CreateProfile(displayName, val));
                }

                using var result = CsTomlSerializer.Serialize(model, TomlOptions);
                return Encoding.UTF8.GetString(result.ByteSpan);
            }
        }
        catch
        {
            // 忽略错误，使用默认值
        }

        // 如果读取失败或没有 VCP 60 功能，返回所有 InputMappings 作为默认值
        foreach (var (vcpCode, displayName) in InputMappings)
        {
            model.Profiles.Add(CreateProfile(displayName, vcpCode));
        }

        using var defaultResult = CsTomlSerializer.Serialize(model, TomlOptions);
        return Encoding.UTF8.GetString(defaultResult.ByteSpan);
    }

    private static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "SourceSwitch.json");
    }

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(_inputSourceOrder);

        // Load settings from the file upon initialization
        LoadSettings();
        SaveSettings();

        Settings.SettingsChanged += (_, _) => this.SaveSettings();
    }
}
