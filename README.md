# Auto Signals for Godot C#

A lightweight library that simplifies signal connections in Godot C# through attributes.

## Features

- Automatic signal connections using attributes
- Reduce boilerplate code in your node classes
- Type-safe signal connections
- Support for both public and private methods

## Installation

1. Copy the `auto_signals` folder into your project's `addons` directory
2. Add the following using statement to your scripts:
```csharp
using AutoSignals;
```

## Usage

### Basic Example

```csharp
public partial class Player : Node
{
    // Mark your method with the AutoSignal attribute
    [AutoSignal("ready")]
    private void OnReady()
    {
        GD.Print("Node is ready!");
    }

    public override void _Ready()
    {
        // Call ConnectAutoSignals() to set up all signal connections
        this.ConnectAutoSignals();
    }

    public override void _ExitTree()
    {
        // Cleanup signal connections when node exits
        this.CleanupSignals();
    }

}
```

### Custom Signals Example

```csharp
public partial class Player : Node
{
    
    public delegate void PlayerSpawnedHandler();

    // Mark your method with the AutoSignal attribute using nameof to reference the signal name
    [AutoSignal(nameof(PlayerSpawned))]
    private void OnPlayerSpawned()
    {
        GD.Print("Player has Spawned!");
    }

    public override void _Ready()
    {
        this.ConnectAutoSignals();
        // Emit the signal when the player is spawned using SignalName to match godot's signal implementation
        EmitSignal(SignalName.PlayerSpawned);
    }
}
```

### Best Practices

1. **Naming Convention**: Use the "On" prefix for signal handler methods to make them easily identifiable
2. **Connect Early**: Call `this.ConnectAutoSignals()` in the `_Ready()` method to ensure signals are connected when the node enters the scene
3. **Method Visibility**: You can use either public or private methods with AutoSignal

### Performance Considerations

The signal connection process uses reflection and occurs once during node initialization. This has minimal impact on runtime performance.


## License

This library is released under the MIT License.
