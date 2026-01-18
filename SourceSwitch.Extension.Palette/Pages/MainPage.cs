using System.Globalization;
using System.Text;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SourceSwitch.Core.Display;
using SourceSwitch.Core.Vcp;
using SourceSwitch.Extension.Palette.Helpers;
using SourceSwitch.Extension.Palette.Properties;

namespace SourceSwitch.Extension.Palette.Pages;

internal sealed partial class MainPage : ListPage
{
    private readonly ISettingsInterface _settingsManager;

    private static readonly CompositeFormat SwitchFormat = CompositeFormat.Parse(
        Resources.MainPage_GetItems_Switch_to__0_
    );

    public MainPage(ISettingsInterface settingManager)
    {
        Name = Resources.MainPage_MainPage_Switch_input_source;
        _settingsManager = settingManager;
        Icon = IconHelpers.FromRelativePath("Assets\\Logo.svg");
    }

    private sealed partial class SwitchInputSource(uint inputSource) : InvokableCommand
    {
        public override CommandResult Invoke()
        {
            try
            {
                DisplayMonitors
                    .GetCurrentMonitor()
                    .WithPhysicalMonitor(
                        0,
                        monitor => monitor.SetVcpFeature(VcpFeature.InputSource, inputSource)
                    );
            }
            catch (Exception ex)
            {
                ExtensionHost.LogMessage(
                    new LogMessage()
                    {
                        Message =
                            $"Error in switch input source(SetVcpFeature): {ex.GetType().Name}: {ex.Message}\nStackTrace: {ex.StackTrace}",
                        State = MessageState.Error,
                    }
                );
                new ToastStatusMessage(
                    new StatusMessage()
                    {
                        Message = $"Failed to switch input source: {ex.Message}",
                        State = MessageState.Error,
                    }
                )
                {
                    Duration = 3000,
                }.Show();
                return CommandResult.KeepOpen();
            }
            return CommandResult.Dismiss();
        }
    }

    public override IListItem[] GetItems()
    {
        try
        {
            var inputSourceOrder = _settingsManager.InputSourceOrder;

            return (
                from profile in inputSourceOrder.Profiles
                let lightIcon = profile.LightIcon
                let darkIcon = profile.DarkIcon
                let title = profile.Title
                let vcpValue = profile.VcpValue
                select new ListItem(
                    new SwitchInputSource(Convert.ToUInt32(vcpValue, 16))
                    {
                        Name = string.Format(CultureInfo.CurrentCulture, SwitchFormat, title),
                        Icon = new IconInfo(
                            IconFactory.CreateIcon(lightIcon),
                            IconFactory.CreateIcon(darkIcon)
                        ),
                    }
                )
                {
                    Title = title,
                }
            )
                .Cast<IListItem>()
                .ToArray();
        }
        catch (Exception ex)
        {
            ExtensionHost.LogMessage(
                new LogMessage()
                {
                    Message =
                        $"Error in GetItems: {ex.GetType().Name}: {ex.Message}\nStackTrace: {ex.StackTrace}",
                    State = MessageState.Error,
                }
            );
            throw;
        }
    }
}
