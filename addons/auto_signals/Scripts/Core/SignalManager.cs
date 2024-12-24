using System.Collections.Generic;
using System.Reflection;
using Godot;

public class SignalManager
{
    private static SignalManager _instance;
    public static SignalManager Instance => _instance ??= new SignalManager();

    private readonly Dictionary<Node, List<SignalConnection>> _connections;

    private SignalManager()
    {
        _connections = new Dictionary<Node, List<SignalConnection>>();
    }

    public void ConnectSignals(Node node)
    {
        var methods = node.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<AutoSignalAttribute>();
            if (attribute == null)
                continue;

            Node source = string.IsNullOrEmpty(attribute.NodePath)
                ? node
                : node.GetNode(attribute.NodePath);

            if (source == null)
            {
                GD.PrintErr($"Auto Signal: Node not found: {attribute.NodePath} on {node.Name}");
                break;
            }

            var connection = new SignalConnection(
                source,
                node,
                attribute.SignalName,
                method.Name,
                attribute.ConnectionType
            );

            ConnectSignal(connection);
            TrackConnection(node, connection);
        }
    }

    private void ConnectSignal(SignalConnection connection)
    {
        uint connectFlags = connection.ConnectionType switch
        {
            SignalConnectionType.Deferred => (uint)GodotObject.ConnectFlags.Deferred,
            SignalConnectionType.OneShot => (uint)GodotObject.ConnectFlags.OneShot,
            _ => 0,
        };

        connection.Source.Connect(
            connection.SignalName,
            new Callable(connection.Target, connection.MethodName),
            connectFlags
        );
    }

    private void TrackConnection(Node node, SignalConnection connection)
    {
        if (!_connections.ContainsKey(node))
        {
            _connections[node] = new List<SignalConnection>();
        }
        _connections[node].Add(connection);
    }

    public void DisconnectSignals(Node node)
    {
        if (!_connections.ContainsKey(node))
            return;

        foreach (var connection in _connections[node])
        {
            connection.Source.Disconnect(
                connection.SignalName,
                new Callable(connection.Target, connection.MethodName)
            );
        }

        _connections.Remove(node);
    }
}
