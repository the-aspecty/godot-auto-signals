using System.Reflection;
using Godot;

namespace Aspecty.AutoSignals
{
    /// <summary>
    /// Represents a dynamic signal connection that monitors child nodes for connections
    /// </summary>
    public readonly struct DynamicSignalConnection
    {
        /// <summary>
        /// The node that owns this dynamic connection
        /// </summary>
        public readonly Node Owner { get; }

        /// <summary>
        /// The path to search for child nodes
        /// </summary>
        public readonly string NodePath { get; }

        /// <summary>
        /// The name of the signal to connect to
        /// </summary>
        public readonly string SignalName { get; }

        /// <summary>
        /// The method to call when the signal is emitted
        /// </summary>
        public readonly MethodInfo Method { get; }

        /// <summary>
        /// The type of connection
        /// </summary>
        public readonly SignalConnectionType ConnectionType { get; }

        /// <summary>
        /// Creates a new dynamic signal connection
        /// </summary>
        public DynamicSignalConnection(
            Node owner,
            string nodePath,
            string signalName,
            MethodInfo method,
            SignalConnectionType connectionType
        )
        {
            Owner = owner;
            NodePath = nodePath;
            SignalName = signalName;
            Method = method;
            ConnectionType = connectionType;
        }
    }
}
