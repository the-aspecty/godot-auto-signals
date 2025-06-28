# Auto Signals for Godot C#

A powerful and lightweight system that eliminates signal connection boilerplate in Godot C# through attributes and automatic processing.

## 📋 Documentation Overview

This README covers everything you need to know about Auto Signals:

- [**✨ Features**](#-features) - Core capabilities and benefits of the system
- [**🚀 Quick Start**](#-quick-start) - Installation steps and basic usage examples
- [**📚 Comprehensive Examples**](#-comprehensive-examples) - Detailed code examples for different connection scenarios
- [**🔧 Using Signal Name Constants**](#-using-signal-name-constants) - Type-safe signal names with IntelliSense support
- [**🎯 API Reference**](#-api-reference) - Complete attribute syntax and parameter documentation
- [**⚡ Performance Notes**](#-performance-notes) - Runtime efficiency and optimization details
- [**🔧 Troubleshooting**](#-troubleshooting) - Common issues and debugging information
- [**📝 Best Practices**](#-best-practices) - Recommended patterns and conventions
- [**🤖 For LLM Agents & AI Assistants**](#-for-llm-agents--ai-assistants) - Special guidance for AI development tools
- [**🔗 Examples and Demo**](#-examples-and-demo) - Working demo scene and implementation reference
- [**📄 License**](#-license) - MIT license information

## ✨ Features

- **Automatic Signal Management**: Signals are connected/disconnected automatically when nodes enter/exit the scene tree via autoload singleton
- **Zero Boilerplate**: No need for manual `Connect()` and `Disconnect()` calls
- **Attribute-Based**: Simple `[AutoSignal]` attribute to mark signal handlers
- **Multiple Connection Types**: Support for Normal, Deferred, and OneShot connections
- **Path-Based Connections**: Connect to signals from child nodes, parent nodes, or any node via path
- **Performance Optimized**: Uses reflection once during connection, minimal runtime overhead
- **Autoload Integration**: Properly registered as Godot autoload singleton for optimal performance

## 🚀 Quick Start

### Installation

1. Copy the `auto_signals` folder into your project's `addons` directory
2. Enable the plugin in Project Settings → Plugins 
3. Add the using statement to your scripts:
```csharp
using Aspecty.AutoSignals;
```

### Basic Usage

Signals are connected automatically when nodes enter the scene tree:

```csharp
using Godot;
using Aspecty.AutoSignals;

public partial class Player : Node2D
{
    // Signals are automatically connected when the node enters the tree!
    
    [AutoSignal(ASinalName.Ready)]
    private void OnReady()
    {
        GD.Print("Player is ready!");
    }

    [AutoSignal(ASinalName.Pressed, "AttackButton")]
    private void OnAttackButtonPressed()
    {
        GD.Print("Attack button pressed!");
    }
      // No need for _Ready() or _ExitTree() methods!
}
```

> Use the ASignalName class to use const values from common godot signals

## 📚 Comprehensive Examples

### 1. Self Signal Connection
```csharp
public partial class GameManager : Node
{
    [Signal]
    public delegate void GameStartedEventHandler();
    
    // Connect to own custom signal
    [AutoSignal(nameof(GameStarted))]
    private void OnGameStarted()
    {
        GD.Print("Game has started!");
    }
    
    public void StartGame()
    {
        EmitSignal(SignalName.GameStarted);
    }
}
```

### 2. Child Node Connections
```csharp
public partial class UI : Control
{
    // Connect to direct child
    [AutoSignal(ASinalName.Pressed, "StartButton")]
    private void OnStartPressed()
    {
        GD.Print("Start button pressed!");
    }
    
    // Connect to nested child
    [AutoSignal(ASinalName.ValueChanged, "Settings/VolumeSlider")]
    private void OnVolumeChanged(float value)
    {
        GD.Print($"Volume changed to: {value}");
    }
}
```

### 3. Connection Types
```csharp
public partial class Combat : Node
{
    // Normal connection (immediate)
    [AutoSignal(nameof(HealthChanged))]
    private void OnHealthChanged(int health)
    {
        UpdateHealthBar(health);
    }
    
    // Deferred connection (end of frame)
    [AutoSignal(nameof(EnemyDied), "Enemy", SignalConnectionType.Deferred)]
    private void OnEnemyDied()
    {
        CheckWinCondition(); // Safe to modify scene tree
    }
    
    // One-shot connection (disconnects after first call)
    [AutoSignal(nameof(GameOver), "../GameManager", SignalConnectionType.OneShot)]
    private void OnGameOver()
    {
        ShowGameOverScreen();
    }
}
```

### 4. Complex Scenarios
```csharp
public partial class Inventory : Control
{
    // Multiple handlers for the same signal
    [AutoSignal(ASignalName.Selected, "ItemList")]
    private void OnItemSelected(int index)
    {
        DisplayItemDetails(index);
    }
    
    [AutoSignal(ASignalName.Selected, "ItemList")]
    private void OnItemSelectedAudio(int index)
    {
        PlaySelectSound();
    }
    
    // Parent node connection
    [AutoSignal(nameof(InventoryOpened), "..")]
    private void OnInventoryOpened()
    {
        Show();
        AnimateIn();
    }
    
    // Absolute path connection
    [AutoSignal(nameof(PlayerDied), "/root/GameManager")]
    private void OnPlayerDied()
    {
        Hide();
    }
}
```


## 🔧 Using Signal Name Constants

Instead of string literals, you can use the `ASignalName` class which provides compile-time constants for common Godot signals:

```csharp
using Aspecty.AutoSignals;

public partial class Player : Node2D
{
    // Using constants instead of string literals - provides IntelliSense and prevents typos
    [AutoSignal(ASignalName.Ready)]
    private void OnReady()
    {
        GD.Print("Player is ready!");
    }

    [AutoSignal(ASignalName.Pressed, "UI/StartButton")]
    private void OnStartButtonPressed()
    {
        GD.Print("Start button pressed!");
    }

    [AutoSignal(ASignalName.TextSubmitted, "UI/NameInput")]
    private void OnNameSubmitted(string text)
    {
        GD.Print($"Name entered: {text}");
    }

    [AutoSignal(ASignalName.Timeout, "Timers/GameTimer")]
    private void OnGameTimerTimeout()
    {
        GD.Print("Game timer finished!");
    }
}
```

### Available Signal Constants

The `ASignalName` class includes constants for:
- **Node signals**: `Ready`, `TreeEntered`, `TreeExited`, etc.
- **Control signals**: `Draw`, `FocusEntered`, `MouseEntered`, `Resized`, etc.
- **Button signals**: `Pressed`, `ButtonDown`, `ButtonUp`, `Toggled`
- **Input signals**: `TextChanged`, `TextSubmitted`, `TextChangeRejected`
- **Timer signals**: `Timeout`
- **Animation signals**: `AnimationFinished`, `AnimationStarted`, etc.
- **Physics signals**: `BodyEntered`, `BodyExited`, `AreaEntered`, etc.
- **And many more...**

### Benefits of Using Constants

1. **IntelliSense Support**: Get autocomplete suggestions
2. **Compile-time Safety**: Typos become compile errors
3. **Refactoring Safety**: Changes propagate automatically
4. **Better Performance**: No string allocations at runtime

**Note**: You cannot use `SignalName.MySignal` in attributes because C# attributes require compile-time constants. `SignalName` properties are runtime-generated and not compile-time constants.

## 🎯 API Reference

### AutoSignalAttribute

```csharp
[AutoSignal(signalName, nodePath, connectionType, dynamic)]

// Parameters:
// signalName: string - Name of the signal to connect to
// nodePath: string - Path to the signal source (empty = self)
// connectionType: SignalConnectionType - Type of connection (default: Normal)
// dynamic: bool - Monitor dynamically added children (default: false)
```

**Path Examples:**
- `""` or omit - Connect to self
- `"Button"` - Direct child named Button
- `"UI/StartButton"` - Nested child
- `".."` - Parent node
- `"../SomeNode"` - Sibling node
- `"/root/GameManager"` - Absolute path

### Connection Types

```csharp
public enum SignalConnectionType
{
    Normal,   // Immediate execution
    Deferred, // Execute at end of frame
    OneShot   // Disconnect after first emission
}
```

### Dynamic Connections

For nodes that add children dynamically at runtime, use the `dynamic` parameter:

```csharp
public partial class DynamicSpawner : Node
{
    // This will automatically connect to Timer nodes added as children
    [AutoSignal(ASignalName.Timeout, "Timer", dynamic: true)]
    private void OnTimerTimeout()
    {
        GD.Print("A dynamically added timer finished!");
    }
    
    private void SpawnTimedObject()
    {
        var obj = new SomeObject();
        var timer = new Timer();
        timer.Name = "Timer";
        timer.WaitTime = 2.0f;
        timer.OneShot = true;
        
        obj.AddChild(timer); // AutoSignal will automatically connect!
        AddChild(obj);
        timer.Start();
    }
}
```

**How Dynamic Connections Work:**
- When `dynamic: true` is set, AutoSignals monitors `child_entered_tree` and `child_exiting_tree` events
- Automatically connects to matching child nodes when they're added (Match by node Name)
- Automatically disconnects when child nodes are removed
- Perfect for procedurally generated content and runtime node creation

## ⚡ Performance Notes

- **Reflection Cost**: Occurs only once during node initialization
- **Runtime Overhead**: Minimal - signals function at native Godot speed
- **Memory Usage**: Tracks connections for proper cleanup
- **Scene Tree Impact**: Automatically monitors node additions/removals

##  Troubleshooting

### Common Issues

1. **Signals not connecting**: Ensure the plugin is enabled in Project Settings
2. **Node not found errors**: Check your node paths are correct
3. **Method signature mismatch**: Ensure handler parameters match signal signature
4. **Multiple connections**: Each `[AutoSignal]` creates a separate connection

### Debug Information

The system provides helpful console output:
```
[AutoSignals] Plugin initialized successfully
[AutoSignals] Connected 3 signals for Player (Player)
[AutoSignals] Disconnected 3 signals for Player
```

## 📝 Best Practices

1. **Use ASignalName constants**: `[AutoSignal(ASignalName.Pressed)]` for better IntelliSense
2. **Match method signatures**: Handler parameters must match signal parameters (Mandatory)
3. **Choose appropriate connection types**: Use Deferred for scene tree modifications
4. **Consistent naming**: Use descriptive method names like `OnButtonPressed`
5. **Path validation**: Test node paths in small scenes first

## 🤖 For LLM Agents & AI Assistants

If you're an AI assistant helping users with this addon, refer to the [**`llm-instructions.md`**](./llm-instructions.md) file for:
- Complete implementation guidelines
- Code generation patterns
- Best practices and conventions
- Troubleshooting scenarios
- Architecture decisions and rationale

**Quick Context for LLMs:**
- Use `ASignalName.SignalName` constants instead of string literals when possible
- AutoSignalProcessor is registered as autoload - signals connect automatically
- `[AutoSignal(signal, nodePath, connectionType)]` is the core pattern
- Always validate node paths exist in the scene structure
- Prefer deferred connections for UI interactions

## 🔗 Examples and Demo

Check out the included demo scenes:

### Main Demo - Comprehensive Feature Showcase
[`demo/Demo.tscn`](demo/Demo.tscn) → [`demo/Demo.cs`](demo/Demo.cs)

A comprehensive demo showing:
- UI signal connections (buttons, text input)
- Multiple handlers for the same signal
- Different connection types (Normal, Deferred, OneShot)
- Custom signal emission and handling
- Live connection tracking

### Block Spawner Demo - Dynamic Node Creation
[`demo/BlockSpawner.tscn`](demo/BlockSpawner.tscn) → [`demo/BlockSpawner.cs`](demo/BlockSpawner.cs)

Demonstrates AutoSignals with dynamically created nodes:
- Automatic spawning of blocks at random positions
- Each block uses AutoSignal for ready event setup
- Timer-based automatic destruction using AutoSignal
- Visual feedback and cleanup tracking

### Individual Block Demo
[`demo/BlockDemo.tscn`](demo/BlockDemo.tscn) → [`demo/BlockDemo.cs`](demo/BlockDemo.cs)

Shows a single block that:
- Uses `[AutoSignal(ASignalName.Ready)]` to initialize itself
- Sets up a timer with `[AutoSignal(ASignalName.Timeout, "Timer")]`
- Automatically destroys itself after a random interval
- Demonstrates cleanup and visual effects

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

