using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SourceSwitch.Core.Display;
using SourceSwitch.Core.Vcp;

namespace SourceSwitch.Extension.Palette.Helpers;

public sealed partial class SwitchInputSource(uint inputSource) : InvokableCommand
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
