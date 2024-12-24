using Godot;

public static class NodeExtensions
{
    public static void InitializeSignals(this Node node)
    {
        SignalManager.Instance.ConnectSignals(node);
    }

    public static void CleanupSignals(this Node node)
    {
        SignalManager.Instance.DisconnectSignals(node);
    }
}
