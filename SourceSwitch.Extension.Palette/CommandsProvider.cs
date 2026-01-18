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
    private readonly ICommandItem[] _commands;
    private static readonly SettingsManager SettingsManager = new();

    public CommandsProvider()
    {
        DisplayName = Resources.CommandsProvider_CommandsProvider_Switch_input_source;
        Id = Resources.CommandsProvider_CommandsProvider_SourceSwitch;
        Icon = IconHelpers.FromRelativePath("Assets\\Logo.svg");
        _commands =
        [
            new CommandItem(new MainPage(SettingsManager))
            {
                Title = Resources.CommandsProvider_CommandsProvider_Switch_input_source,
                Subtitle =
                    Resources.CommandsProvider_CommandsProvider_Switch_the_current_monitor_s_input_source_,
            },
        ];

        Settings = SettingsManager.Settings;
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }
}
