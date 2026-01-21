// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SourceSwitch.Extension.Palette.Helpers;
using SourceSwitch.Extension.Palette.Pages;
using SourceSwitch.Extension.Palette.Properties;

namespace SourceSwitch.Extension.Palette;

public sealed partial class CommandsProvider : CommandProvider
{
    private ICommandItem[] _commands;
    private static readonly SettingsManager SettingsManager = new();

    public CommandsProvider()
    {
        DisplayName = Resources.CommandsProvider_CommandsProvider_Switch_input_source;
        Id = "SourceSwitch";
        Icon = IconHelpers.FromRelativePath("Assets\\Logo.svg");
        Settings = SettingsManager.Settings;
        SettingsManager.Settings.SettingsChanged += (_, _) => RefreshCommands();

        _commands =
        [
            new CommandItem(new MainPage(SettingsManager))
            {
                Title = Resources.CommandsProvider_CommandsProvider_Switch_input_source,
                Subtitle =
                    Resources.CommandsProvider_CommandsProvider_Switch_the_current_monitor_s_input_source_,
            },
            .. SettingsManager
                .InputSourceOrder.Profiles.Where(profile => profile.TopLevel)
                .Select(profile => new CommandItem(
                    new SwitchInputSource(Convert.ToUInt32(profile.VcpValue, 16))
                    {
                        Name = profile.Title,
                        Icon = new IconInfo(
                            IconFactory.CreateIcon(profile.LightIcon),
                            IconFactory.CreateIcon(profile.DarkIcon)
                        ),
                    }
                )
                {
                    Title = profile.Title,
                }),
        ];
    }

    private void RefreshCommands()
    {
        _commands = BuildCommands();
        RaiseItemsChanged(_commands.Length);
    }

    private static ICommandItem[] BuildCommands()
    {
        return
        [
            new CommandItem(new MainPage(SettingsManager))
            {
                Title = Resources.CommandsProvider_CommandsProvider_Switch_input_source,
                Subtitle =
                    Resources.CommandsProvider_CommandsProvider_Switch_the_current_monitor_s_input_source_,
            },
            .. SettingsManager
                .InputSourceOrder.Profiles.Where(profile => profile.TopLevel)
                .Select(profile => new CommandItem(
                    new SwitchInputSource(Convert.ToUInt32(profile.VcpValue, 16))
                    {
                        Name = profile.Title,
                        Icon = new IconInfo(
                            IconFactory.CreateIcon(profile.LightIcon),
                            IconFactory.CreateIcon(profile.DarkIcon)
                        ),
                    }
                )
                {
                    Title = profile.Title,
                }),
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }
}
