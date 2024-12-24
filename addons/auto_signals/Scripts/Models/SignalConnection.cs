using Godot;

public readonly struct SignalConnection
{
    public readonly Node Source { get; }
    public readonly Node Target { get; }
    public readonly string SignalName { get; }
    public readonly string MethodName { get; }
    public readonly SignalConnectionType ConnectionType { get; }

    public SignalConnection(
        Node source,
        Node target,
        string signalName,
        string methodName,
        SignalConnectionType connectionType
    )
    {
        Source = source;
        Target = target;
        SignalName = signalName;
        MethodName = methodName;
        ConnectionType = connectionType;
    }
}
