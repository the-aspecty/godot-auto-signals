using Aspecty.AutoSignals;
using Aspecty.AutoSignals.Core;
using Godot;

/// <summary>
/// Demo showcasing AutoSignals functionality with popup notifications
/// This demonstrates both string literals and ASignalName constants in attributes
/// </summary>
public partial class Demo : Node2D
{
    [Signal]
    public delegate void MyEventEventHandler();

    // UI References
    private Label _statusLabel;
    private Label _connectionCountLabel;
    private Button _testButton;
    private LineEdit _textInput;
    private RichTextLabel _logOutput;
    private VBoxContainer _notificationContainer;

    // Notification management
    private int _notificationCounter = 0;

    public override void _Ready()
    {
        // With AutoSignalProcessor autoload, signals are connected automatically!
        GD.Print("[Demo] Node ready - AutoSignals autoload will handle connections automatically");

        SetupUI();
        UpdateConnectionCount();
    }

    public override void _ExitTree()
    {
        // With AutoSignalProcessor autoload, signals are disconnected automatically!
        // No need to manually call CleanupSignals()
        GD.Print("[Demo] Node exiting - AutoSignals autoload will handle cleanup automatically");
    }

    private void SetupUI()
    {
        // Get UI references
        _statusLabel = GetNode<Label>("UI/VBox/StatusLabel");
        _connectionCountLabel = GetNode<Label>("UI/VBox/ConnectionCountLabel");
        _testButton = GetNode<Button>("UI/VBox/HBox/TestButton");
        _textInput = GetNode<LineEdit>("UI/VBox/HBox/TextInput");
        _logOutput = GetNode<RichTextLabel>("UI/VBox/LogOutput");
        _notificationContainer = GetNode<VBoxContainer>("UI/NotificationContainer");

        // Set initial UI state
        _statusLabel.Text = "Auto Signals Demo - Ready!";
        _logOutput.Text = "[color=green]Demo initialized successfully![/color]\n";

        LogMessage("AutoSignals will automatically connect all signal handlers.");
        LogMessage("Try pressing the button or typing in the text field!");
    }

    private void UpdateConnectionCount()
    {
        // Use static method to get tracked count
        var count = AutoSignalProcessor.GetTrackedNodeCount();
        _connectionCountLabel.Text = $"Tracked Nodes: {count}";
    }

    private void LogMessage(string message)
    {
        var timestamp = (long)Time.GetUnixTimeFromSystem();
        var timeStr = Time.GetDatetimeStringFromUnixTime(timestamp, false);
        _logOutput.AppendText($"[color=gray]{timeStr}[/color] {message}\n");

        // Auto-scroll to bottom
        _logOutput.ScrollToLine(_logOutput.GetLineCount() - 1);
    }

    private void ShowNotification(string message, Color color = default)
    {
        if (color == default)
            color = Colors.White;

        // Create notification using our custom popup
        var notification = new NotificationPopup();
        _notificationContainer.AddChild(notification);
        notification.ShowNotification(message, color, color);

        _notificationCounter++;
    } // Simple signal handler from same node

    [AutoSignal(ASignalName.Ready)]
    private void OnReady()
    {
        GD.Print("Node is ready!");
    }

    // Connect to child node with deferred connection - using constants
    [AutoSignal(ASignalName.Pressed, "UI/VBox/HBox/TestButton", SignalConnectionType.Deferred)]
    private void OnButtonPressed()
    {
        GD.Print("Button pressed!");
        LogMessage("[color=yellow]Button pressed! (Handler 1)[/color]");
        ShowNotification("Button pressed!", Colors.LightGreen);
    }

    [AutoSignal(ASignalName.Pressed, "UI/VBox/HBox/TestButton", SignalConnectionType.Deferred)]
    private void OnButtonPressed2()
    {
        GD.Print("Button pressed! by the second handler");
        LogMessage("[color=yellow]Button pressed! (Handler 2)[/color]");
        EmitSignal(SignalName.MyEvent);
    }

    // Connect to text input - using constants
    [AutoSignal(ASignalName.TextSubmitted, "UI/VBox/HBox/TextInput")]
    private void OnTextSubmitted(string text)
    {
        GD.Print($"Text submitted: {text}");
        LogMessage($"[color=cyan]Text submitted: '{text}'[/color]");
        ShowNotification($"Text submitted: '{text}'", Colors.LightBlue);

        // Clear the input
        _textInput.Text = "";
    }

    [AutoSignal(ASignalName.TextChanged, "UI/VBox/HBox/TextInput")]
    private void OnTextChanged(string newText)
    {
        if (!string.IsNullOrEmpty(newText))
        {
            LogMessage($"[color=gray]Text changed: '{newText}'[/color]");
        }
    }

    [AutoSignal(nameof(MyEvent))]
    private void OnMyEvent()
    {
        GD.Print("My event was emitted!");
        LogMessage("[color=magenta]Custom event 'MyEvent' was emitted![/color]");
        ShowNotification("Custom event triggered!", Colors.Magenta);
        UpdateConnectionCount();
    } // Timer-based updates - using constants

    [AutoSignal(ASignalName.Timeout, "UI/UpdateTimer")]
    private void OnUpdateTimer()
    {
        UpdateConnectionCount();
    }

    // Demonstrate oneshot connection - using constants
    [AutoSignal(ASignalName.Pressed, "UI/VBox/HBox/OneTimeButton", SignalConnectionType.OneShot)]
    private void OnOneTimeButtonPressed()
    {
        LogMessage(
            "[color=orange]One-time button pressed! This handler will now disconnect.[/color]"
        );
        ShowNotification("One-time event! Handler disconnected.", Colors.Orange);

        // Disable the button to show it was a one-time thing
        var button = GetNode<Button>("UI/VBox/HBox/OneTimeButton");
        button.Text = "Used (One-Shot)";
        //button.Disabled = true;
    }
}
