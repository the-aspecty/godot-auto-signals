using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Aspecty.AutoSignals.Core
{
    /// <summary>
    /// Automatic signal processor that handles signal connections for nodes with auto signal attributes.
    /// This processor automatically connects and disconnects signals when nodes enter and exit the tree.
    /// </summary>
    public partial class AutoSignalProcessor : Node
    {
        private static AutoSignalProcessor _instance;
        public static AutoSignalProcessor Instance => _instance;

        private readonly Dictionary<Node, List<SignalConnection>> _trackedNodes;
        private readonly Dictionary<Node, List<DynamicSignalConnection>> _dynamicConnections;
        private readonly Dictionary<Node, List<SignalConnection>> _dynamicTrackedConnections;
        private readonly HashSet<Type> _processedTypes;

        public AutoSignalProcessor()
        {
            _trackedNodes = new Dictionary<Node, List<SignalConnection>>();
            _dynamicConnections = new Dictionary<Node, List<DynamicSignalConnection>>();
            _dynamicTrackedConnections = new Dictionary<Node, List<SignalConnection>>();
            _processedTypes = new HashSet<Type>();
            Name = "AutoSignalProcessor";
            ProcessMode = ProcessModeEnum.Always;
        }

        public override void _EnterTree()
        {
            if (_instance == null)
            {
                _instance = this;
                GD.Print("[AutoSignals] AutoSignalProcessor autoload initialized successfully");
            }
            else if (_instance != this)
            {
                GD.Print(
                    "[AutoSignals] Multiple AutoSignalProcessor instances detected, removing duplicate"
                );
                QueueFree();
                return;
            }

            // Connect to the scene tree to monitor node additions
            GetTree().NodeAdded += OnNodeAdded;
            GetTree().NodeRemoved += OnNodeRemoved;
        }

        public override void _ExitTree()
        {
            if (_instance == this)
            {
                GetTree().NodeAdded -= OnNodeAdded;
                GetTree().NodeRemoved -= OnNodeRemoved;

                // Cleanup all tracked connections
                foreach (var connections in _trackedNodes.Values)
                {
                    foreach (var connection in connections)
                    {
                        DisconnectSignalSafe(connection);
                    }
                }

                // Cleanup dynamic tracked connections
                foreach (var connections in _dynamicTrackedConnections.Values)
                {
                    foreach (var connection in connections)
                    {
                        DisconnectSignalSafe(connection);
                    }
                }

                _trackedNodes.Clear();
                _dynamicConnections.Clear();
                _dynamicTrackedConnections.Clear();
                _processedTypes.Clear();
                _instance = null;
                GD.Print("[AutoSignals] AutoSignalProcessor autoload cleaned up");
            }
        }

        /// <summary>
        /// Called when any node is added to the scene tree
        /// </summary>
        private void OnNodeAdded(Node node)
        {
            // Skip if node is null or already being processed
            if (node == null || _trackedNodes.ContainsKey(node))
                return;

            // Check if this node type has AutoSignal attributes
            var nodeType = node.GetType();
            if (HasAutoSignalMethods(nodeType))
            {
                ProcessNodeSignals(node);
            }
        }

        /// <summary>
        /// Called when any node is removed from the scene tree
        /// </summary>
        private void OnNodeRemoved(Node node)
        {
            if (node != null)
            {
                if (_trackedNodes.ContainsKey(node))
                {
                    DisconnectNodeSignals(node);
                }

                if (_dynamicConnections.ContainsKey(node))
                {
                    CleanupDynamicConnections(node);
                }
            }
        }

        /// <summary>
        /// Checks if a node type has methods with AutoSignal attributes
        /// </summary>
        private bool HasAutoSignalMethods(Type nodeType)
        {
            if (_processedTypes.Contains(nodeType))
                return true;

            var methods = nodeType.GetMethods(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<AutoSignalAttribute>() != null)
                {
                    _processedTypes.Add(nodeType);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Process all AutoSignal attributes for a node and establish connections
        /// </summary>
        private void ProcessNodeSignals(Node node)
        {
            try
            {
                var methods = node.GetType()
                    .GetMethods(
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                    );

                var connections = new List<SignalConnection>();
                var dynamicConnections = new List<DynamicSignalConnection>();

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute<AutoSignalAttribute>();
                    if (attribute == null)
                        continue;

                    if (attribute.Dynamic)
                    {
                        // Create dynamic connection for monitoring child nodes
                        var dynamicConnection = new DynamicSignalConnection(
                            node,
                            attribute.NodePath,
                            attribute.SignalName,
                            method,
                            attribute.ConnectionType
                        );
                        dynamicConnections.Add(dynamicConnection);
                        SetupDynamicConnection(dynamicConnection);
                    }
                    else
                    {
                        // Regular static connection
                        var connection = CreateSignalConnection(node, method, attribute);
                        if (connection.HasValue)
                        {
                            connections.Add(connection.Value);
                            ConnectSignal(connection.Value);
                        }
                    }
                }

                if (connections.Count > 0)
                {
                    _trackedNodes[node] = connections;
                }

                if (dynamicConnections.Count > 0)
                {
                    _dynamicConnections[node] = dynamicConnections;
                }

                var totalConnections = connections.Count + dynamicConnections.Count;
                if (totalConnections > 0)
                {
                    GD.Print(
                        $"[AutoSignals] Connected {connections.Count} static + {dynamicConnections.Count} dynamic signals for {node.Name} ({node.GetType().Name})"
                    );
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr(
                    $"[AutoSignals] Error processing signals for node {node.Name}: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Create a signal connection from method and attribute information
        /// </summary>
        private SignalConnection? CreateSignalConnection(
            Node node,
            MethodInfo method,
            AutoSignalAttribute attribute
        )
        {
            try
            {
                Node source = string.IsNullOrEmpty(attribute.NodePath)
                    ? node
                    : node.GetNode(attribute.NodePath);

                if (source == null)
                {
                    GD.PrintErr(
                        $"[AutoSignals] Node not found: {attribute.NodePath} on {node.Name}"
                    );
                    return null;
                }

                return new SignalConnection(
                    source,
                    node,
                    attribute.SignalName,
                    method.Name,
                    attribute.ConnectionType
                );
            }
            catch (Exception ex)
            {
                GD.PrintErr(
                    $"[AutoSignals] Error creating connection for {method.Name}: {ex.Message}"
                );
                return null;
            }
        }

        /// <summary>
        /// Establish a signal connection
        /// </summary>
        private void ConnectSignal(SignalConnection connection)
        {
            try
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
            catch (Exception ex)
            {
                GD.PrintErr(
                    $"[AutoSignals] Error connecting signal {connection.SignalName}: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Disconnect all signals for a node
        /// </summary>
        private void DisconnectNodeSignals(Node node)
        {
            var disconnectedCount = 0;

            // Disconnect static connections
            if (_trackedNodes.TryGetValue(node, out var connections))
            {
                foreach (var connection in connections)
                {
                    if (DisconnectSignalSafe(connection))
                        disconnectedCount++;
                }
                _trackedNodes.Remove(node);
            }

            // Cleanup dynamic connections
            if (_dynamicConnections.ContainsKey(node))
            {
                CleanupDynamicConnections(node);
            }

            if (disconnectedCount > 0)
            {
                GD.Print($"[AutoSignals] Disconnected {disconnectedCount} signals for {node.Name}");
            }
        }

        /// <summary>
        /// Safely disconnect a signal connection
        /// </summary>
        private bool DisconnectSignalSafe(SignalConnection connection)
        {
            try
            {
                if (IsInstanceValid(connection.Source) && IsInstanceValid(connection.Target))
                {
                    connection.Source.Disconnect(
                        connection.SignalName,
                        new Callable(connection.Target, connection.MethodName)
                    );
                    return true;
                }
            }
            catch (Exception ex)
            {
                GD.Print(
                    $"[AutoSignals] Error disconnecting signal {connection.SignalName}: {ex.Message}"
                );
            }
            return false;
        }

        /// <summary>
        /// Manually process a specific node (for backward compatibility)
        /// </summary>
        public static void ProcessNode(Node node)
        {
            Instance?.ProcessNodeSignals(node);
        }

        /// <summary>
        /// Manually disconnect a specific node (for backward compatibility)
        /// </summary>
        public static void DisconnectNode(Node node)
        {
            Instance?.DisconnectNodeSignals(node);
        }

        /// <summary>
        /// Get the count of tracked nodes
        /// </summary>
        public static int GetTrackedNodeCount()
        {
            return Instance?._trackedNodes.Count ?? 0;
        }

        /// <summary>
        /// Check if a specific node is being tracked
        /// </summary>
        public static bool IsNodeTracked(Node node)
        {
            return Instance?._trackedNodes.ContainsKey(node) ?? false;
        }

        /// <summary>
        /// Setup a dynamic connection that monitors child nodes
        /// </summary>
        private void SetupDynamicConnection(DynamicSignalConnection dynamicConnection)
        {
            // Connect to child_entered_tree and child_exited_tree to monitor dynamic children
            if (
                !dynamicConnection.Owner.IsConnected(
                    SignalName.ChildEnteredTree,
                    new Callable(this, MethodName.OnChildEnteredTree)
                )
            )
            {
                dynamicConnection.Owner.Connect(
                    SignalName.ChildEnteredTree,
                    new Callable(this, MethodName.OnChildEnteredTree)
                );
            }

            if (
                !dynamicConnection.Owner.IsConnected(
                    SignalName.ChildExitingTree,
                    new Callable(this, MethodName.OnChildExitedTree)
                )
            )
            {
                dynamicConnection.Owner.Connect(
                    SignalName.ChildExitingTree,
                    new Callable(this, MethodName.OnChildExitedTree)
                );
            }

            // Try to connect to existing children that match the path
            TryConnectExistingChildren(dynamicConnection);
        }

        /// <summary>
        /// Called when a child node is added to any tracked node
        /// </summary>
        private void OnChildEnteredTree(Node child)
        {
            var parent = child.GetParent();
            if (parent == null || !_dynamicConnections.ContainsKey(parent))
                return;

            foreach (var dynamicConnection in _dynamicConnections[parent])
            {
                TryConnectToChild(child, dynamicConnection);
            }
        }

        /// <summary>
        /// Called when a child node is removed from any tracked node
        /// </summary>
        private void OnChildExitedTree(Node child)
        {
            // Remove any tracked connections for this child
            if (_dynamicTrackedConnections.ContainsKey(child))
            {
                var connections = _dynamicTrackedConnections[child];
                foreach (var connection in connections)
                {
                    DisconnectSignalSafe(connection);
                }
                _dynamicTrackedConnections.Remove(child);
            }
        }

        /// <summary>
        /// Try to connect to existing children that match the dynamic connection path
        /// </summary>
        private void TryConnectExistingChildren(DynamicSignalConnection dynamicConnection)
        {
            foreach (Node child in dynamicConnection.Owner.GetChildren())
            {
                TryConnectToChild(child, dynamicConnection);
            }
        }

        /// <summary>
        /// Try to connect a specific child to a dynamic connection
        /// </summary>
        private void TryConnectToChild(Node child, DynamicSignalConnection dynamicConnection)
        {
            try
            {
                Node targetNode = null;

                if (string.IsNullOrEmpty(dynamicConnection.NodePath))
                {
                    // Connect to the child itself
                    targetNode = child;
                }
                else
                {
                    // Try to find a child node matching the path
                    if (child.HasNode(dynamicConnection.NodePath))
                    {
                        targetNode = child.GetNode(dynamicConnection.NodePath);
                    }
                    else if (child.Name == dynamicConnection.NodePath.Split('/')[0])
                    {
                        // Check if this child matches the first part of the path
                        var remainingPath = string.Join(
                            "/",
                            dynamicConnection.NodePath.Split('/').Skip(1)
                        );
                        if (string.IsNullOrEmpty(remainingPath))
                        {
                            targetNode = child;
                        }
                        else if (child.HasNode(remainingPath))
                        {
                            targetNode = child.GetNode(remainingPath);
                        }
                    }
                }

                if (targetNode != null && targetNode.HasSignal(dynamicConnection.SignalName))
                {
                    var connection = new SignalConnection(
                        targetNode,
                        dynamicConnection.Owner,
                        dynamicConnection.SignalName,
                        dynamicConnection.Method.Name,
                        dynamicConnection.ConnectionType
                    );

                    ConnectSignal(connection);

                    // Track this connection for cleanup
                    if (!_dynamicTrackedConnections.ContainsKey(child))
                    {
                        _dynamicTrackedConnections[child] = new List<SignalConnection>();
                    }
                    _dynamicTrackedConnections[child].Add(connection);

                    GD.Print(
                        $"[AutoSignals] Dynamic connection established: {child.Name}.{dynamicConnection.SignalName} -> {dynamicConnection.Owner.Name}.{dynamicConnection.Method.Name}"
                    );
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr(
                    $"[AutoSignals] Error connecting to dynamic child {child.Name}: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Cleanup dynamic connections for a node
        /// </summary>
        private void CleanupDynamicConnections(Node node)
        {
            if (_dynamicConnections.ContainsKey(node))
            {
                // Disconnect child monitoring signals
                if (
                    node.IsConnected(
                        SignalName.ChildEnteredTree,
                        new Callable(this, MethodName.OnChildEnteredTree)
                    )
                )
                {
                    node.Disconnect(
                        SignalName.ChildEnteredTree,
                        new Callable(this, MethodName.OnChildEnteredTree)
                    );
                }

                if (
                    node.IsConnected(
                        SignalName.ChildExitingTree,
                        new Callable(this, MethodName.OnChildExitedTree)
                    )
                )
                {
                    node.Disconnect(
                        SignalName.ChildExitingTree,
                        new Callable(this, MethodName.OnChildExitedTree)
                    );
                }

                _dynamicConnections.Remove(node);
            }
        }
    }
}
