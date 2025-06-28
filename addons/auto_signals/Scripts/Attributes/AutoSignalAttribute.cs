using System;

namespace Aspecty.AutoSignals
{
    /// <summary>
    /// Attribute to automatically connect methods to signals.
    /// Apply this attribute to methods that should be automatically connected to signals.
    /// </summary>
    /// <example>
    /// <code>
    /// [AutoSignal("ready")]
    /// private void OnReady() { }
    ///
    /// [AutoSignal("pressed", "Button")]
    /// private void OnButtonPressed() { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoSignalAttribute : Attribute
    {
        /// <summary>
        /// The name of the signal to connect to
        /// </summary>
        public string SignalName { get; }

        /// <summary>
        /// The node path to the signal source (empty string means self)
        /// </summary>
        public string NodePath { get; }

        /// <summary>
        /// The type of connection to establish
        /// </summary>
        public SignalConnectionType ConnectionType { get; }

        /// <summary>
        /// Whether to monitor dynamically added children for this connection
        /// When true, will track child_entered_tree and child_exited_tree events
        /// </summary>
        public bool Dynamic { get; }

        /// <summary>
        /// Creates a new AutoSignal attribute
        /// </summary>
        /// <param name="signalName">Name of the signal to connect to</param>
        /// <param name="nodePath">Path to the node emitting the signal (empty for self)</param>
        /// <param name="connectionType">Type of connection to establish</param>
        /// <param name="dynamic">Whether to monitor dynamically added children for this connection</param>
        /// <exception cref="ArgumentNullException">Thrown when signalName is null</exception>
        /// <exception cref="ArgumentException">Thrown when signalName is empty or whitespace</exception>
        public AutoSignalAttribute(
            string signalName,
            string nodePath = "",
            SignalConnectionType connectionType = SignalConnectionType.Normal,
            bool dynamic = false
        )
        {
            if (signalName == null)
                throw new ArgumentNullException(nameof(signalName), "Signal name cannot be null");

            if (string.IsNullOrEmpty(signalName) || string.IsNullOrWhiteSpace(signalName))
                throw new ArgumentException(
                    "Signal name cannot be empty or whitespace",
                    nameof(signalName)
                );

            SignalName = signalName;
            NodePath = nodePath ?? "";
            ConnectionType = connectionType;
            Dynamic = dynamic;
        }
    }
}
