using Godot;

public partial class Demo : Node2D
{
    [Signal]
    public delegate void MyEventEventHandler();

    public override void _Ready()
    {
        // Initialize all signal connections
        this.InitializeSignals();
    }

    public override void _ExitTree()
    {
        // Cleanup signal connections when node exits
        this.CleanupSignals();
    }

    // Simple signal handler from same node
    [AutoSignal("ready")]
    private void OnReady()
    {
        GD.Print("Node is ready!");
    }

    // Connect to child node with deferred connection
    [AutoSignal("pressed", "Button", SignalConnectionType.Deferred)]
    private void OnButtonPressed()
    {
        GD.Print("Button pressed!");
    }

    [AutoSignal("pressed", "Button", SignalConnectionType.Deferred)]
    private void OnButtonPressed2()
    {
        GD.Print("Button pressed! by the second handler");
        EmitSignal(SignalName.MyEvent);
    }

    // One-shot connection to relative path node
    [AutoSignal("text_submitted", "../LineEdit")]
    private void OnTextSubmitted(string text)
    {
        GD.Print($"Text submitted: {text}");
    }

    [AutoSignal(nameof(MyEvent))]
    private void OnMyEvent()
    {
        GD.Print("My event was emitted!");
    }
}
