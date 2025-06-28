using Godot;

namespace Aspecty.AutoSignals
{
    /// <summary>
    /// Represents a signal connection between a source node and target method
    /// </summary>
    public readonly struct SignalConnection
    {
        /// <summary>
        /// The node that emits the signal
        /// </summary>
        public readonly Node Source { get; }

        /// <summary>
        /// The node that receives the signal
        /// </summary>
        public readonly Node Target { get; }

        /// <summary>
        /// The name of the signal
        /// </summary>
        public readonly string SignalName { get; }

        /// <summary>
        /// The name of the method to call
        /// </summary>
        public readonly string MethodName { get; }

        /// <summary>
        /// The type of connection
        /// </summary>
        public readonly SignalConnectionType ConnectionType { get; }

        /// <summary>
        /// Creates a new signal connection
        /// </summary>
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
}
