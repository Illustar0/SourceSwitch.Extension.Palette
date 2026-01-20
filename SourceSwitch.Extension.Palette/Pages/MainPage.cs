using System.Globalization;
using System.Text;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SourceSwitch.Extension.Palette.Helpers;
using SourceSwitch.Extension.Palette.Properties;

namespace SourceSwitch.Extension.Palette.Pages;

internal sealed partial class MainPage : ListPage
{
    private readonly ISettingsInterface _settingsManager;
    private IListItem[] _commands;

    private static readonly CompositeFormat SwitchFormat = CompositeFormat.Parse(
        Resources.MainPage_GetItems_Switch_to__0_
    );

    public MainPage(ISettingsInterface settingManager)
    {
        Name = Resources.MainPage_MainPage_Switch_input_source;
        _settingsManager = settingManager;
        Icon = IconHelpers.FromRelativePath("Assets\\Logo.svg");
        ShowDetails = true;
        EmptyContent = new CommandItem(new NoOpCommand())
        {
            Title = Resources.MainPage_MainPage_Switch_input_source,
            Subtitle =
                Resources.CommandsProvider_CommandsProvider_Switch_the_current_monitor_s_input_source_,
            Icon = Icon,
        };
        _commands = BuildCommands();
        _settingsManager.Settings.SettingsChanged += (_, _) => RefreshCommands();
    }

    private void RefreshCommands()
    {
        _commands = BuildCommands();
        RaiseItemsChanged(_commands.Length);
    }

    private IListItem[] BuildCommands()
    {
        try
        {
            var inputSourceOrder = _settingsManager.InputSourceOrder;

            return inputSourceOrder
                .Profiles.Where(profile => !profile.TopLevel)
                .Select(
                    IListItem (profile) =>
                        new ListItem(
                            new SwitchInputSource(Convert.ToUInt32(profile.VcpValue, 16))
                            {
                                Name = string.Format(
                                    CultureInfo.CurrentCulture,
                                    SwitchFormat,
                                    profile.Title
                                ),
                                Icon = new IconInfo(
                                    IconFactory.CreateIcon(profile.LightIcon),
                                    IconFactory.CreateIcon(profile.DarkIcon)
                                ),
                            }
                        )
                        {
                            Title = profile.Title,
                        }
                )
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

    public override IListItem[] GetItems()
    {
        return _commands;
    }
}
