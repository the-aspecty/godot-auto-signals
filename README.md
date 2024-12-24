# Auto Signals for Godot C#

> ⚠ WIP 

A lightweight library that simplifies signal connections in Godot C# through attributes.

## Features

- Automatic signal connections using attributes
- Reduce boilerplate code in your node classes
- Supports custom signals
- AutoSignal can get Nodes (just send the relative node path in the second argument)

## Installation

1. Copy the `auto_signals` folder into your project's `addons` directory
<!-- 2. Add the following using statement to your scripts:
```csharp
using AutoSignals;
``` -->

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

1. **Naming Convention**: Right now for default godot signals you need to use the signal name as a string like the gdscript counterpart in snake_case. 
> because attributes doesn't support SignalName 

2. **Connect Early**: Call `this.ConnectAutoSignals()` in the `_Ready()` method to ensure signals are connected when the node enters the scene

3. **Disconnect Signals**: Call `this.CleanupSignals()` in the `_ExitTree()` method to disconnect all signals when the node exits the scene

4. **Function implementations**: They need to be same as the expected signal function signature. (this is how godot handle signals)

- The function must have the same parameters as the signal
- The function must have the same return type as the signal

### Demo 
You can verify more at the demo file:
[Demo.cs](/demo/Demo.cs)



### Performance Considerations

The signal connection process uses reflection and occurs once during node initialization. This has minimal impact on runtime performance.


## License

This library is released under the MIT License.
