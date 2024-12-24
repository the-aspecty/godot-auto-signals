using System;

[AttributeUsage(AttributeTargets.Method)]
public class AutoSignalAttribute : Attribute
{
    public string SignalName { get; }
    public string NodePath { get; }
    public SignalConnectionType ConnectionType { get; }

    public AutoSignalAttribute(
        string signalName,
        string nodePath = "",
        SignalConnectionType connectionType = SignalConnectionType.Normal
    )
    {
        SignalName = signalName;
        NodePath = nodePath;
        ConnectionType = connectionType;
    }
}
