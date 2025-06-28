#if TOOLS
using Godot;

[Tool]
public partial class AutoSignalsPlugin : EditorPlugin
{
    private const string AutoloadName = "AutoSignalProcessor";

    public override void _EnablePlugin()
    {
        GD.Print("[AutoSignals] Plugin enabling...");

        // Register the AutoSignalProcessor as an autoload singleton
        AddAutoloadSingleton(
            AutoloadName,
            "res://addons/auto_signals/Scripts/Core/AutoSignalProcessor.cs"
        );

        GD.Print(
            "[AutoSignals] Plugin enabled successfully - AutoSignalProcessor registered as autoload"
        );
    }

    public override void _DisablePlugin()
    {
        GD.Print("[AutoSignals] Plugin disabling...");

        // Remove the autoload singleton
        RemoveAutoloadSingleton(AutoloadName);

        GD.Print("[AutoSignals] Plugin disabled - AutoSignalProcessor autoload removed");
    }
}
#endif
