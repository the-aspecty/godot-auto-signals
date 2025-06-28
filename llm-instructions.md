# Auto Signals - LLM Instructions

**Purpose**: This file provides concise instructions for AI coding assistants when helping users implement Godot C# signal handling using the Auto Signals addon. It contains essential patterns, syntax, and best practices specifically formatted for LLM consumption.

## Core Concept
Godot C# addon that auto-connects signals via `[AutoSignal]` attribute. Uses autoload singleton - no manual setup needed.

## Essential Usage

### Basic Pattern
```csharp
using Aspecty.AutoSignals;

public partial class Player : Node2D
{
    // Automatic - signals connect when node enters scene tree
    [AutoSignal(ASignalName.Ready)]
    private void OnReady() 
    { 
        GD.Print("Ready!"); 
    }
    
    [AutoSignal(ASignalName.Pressed, "UI/Button")]
    private void OnButtonPressed() 
    { 
        DoAction(); 
    }
    
    [AutoSignal("custom_signal")]  // For custom signals
    private void OnCustomSignal() 
    { 
        HandleCustom(); 
    }
}
```

### Attribute Syntax
```csharp
[AutoSignal(signalName)]                           // Self signal
[AutoSignal(signalName, nodePath)]                 // Child/other node
[AutoSignal(signalName, nodePath, type)]           // With connection type
[AutoSignal(signalName, nodePath, type, dynamic)]  // With dynamic monitoring
```

### Connection Types
- `SignalConnectionType.Normal` (default) - Immediate
- `SignalConnectionType.Deferred` - End of frame (safe for scene changes)
- `SignalConnectionType.OneShot` - Disconnect after first call

### Dynamic Connections
- `dynamic: false` (default) - Connect to existing nodes only
- `dynamic: true` - Monitor child_entered_tree/child_exiting_tree for runtime connections

### Node Paths
- `""` or omit - Same node
- `"ChildNode"` - Direct child
- `"UI/Button"` - Nested child
- `".."` - Parent
- `"../Sibling"` - Sibling
- `"/root/Global"` - Absolute

## Key Rules for LLMs

1. **Always use `ASignalName` constants** when available: `ASignalName.Pressed` not `"pressed"`
2. **Match method signatures** exactly to signal parameters
3. **Use deferred for UI**: `[AutoSignal(ASignalName.Pressed, "Button", SignalConnectionType.Deferred)]`
4. **Use dynamic for runtime content**: `[AutoSignal(ASignalName.Timeout, "Timer", dynamic: true)]`
5. **Validate paths exist** in the scene structure (or will exist for dynamic)
6. **No manual connection code** needed - autoload handles everything

## Common Patterns

```csharp
// UI Interaction
[AutoSignal(ASignalName.Pressed, "UI/StartButton", SignalConnectionType.Deferred)]
private void OnStartPressed() 
{ 
    StartGame(); 
}

// Input Handling
[AutoSignal(ASignalName.TextSubmitted, "UI/NameInput")]
private void OnNameSubmitted(string text) 
{ 
    SetPlayerName(text); 
}

// Timer Events
[AutoSignal(ASignalName.Timeout, "GameTimer")]
private void OnGameTimeout() 
{ 
    EndGame(); 
}

// Custom Signals
[AutoSignal("player_died")]
private void OnPlayerDied() 
{ 
    ShowGameOver(); 
}

// One-shot Events
[AutoSignal(ASignalName.AnimationFinished, "Intro", SignalConnectionType.OneShot)]
private void OnIntroFinished()
{
    ShowMainMenu();
}

// Dynamic Connections (for runtime-created nodes)
[AutoSignal(ASignalName.Timeout, "Timer", dynamic: true)]
private void OnTimerTimeout()
{
    // Automatically connects to Timer nodes added as children at runtime
    DestroyObject();
}

// Dynamic spawning example
[AutoSignal(ASignalName.Ready, dynamic: true)]
private void OnChildReady()
{
    // Connects to any child that gets added and becomes ready
    InitializeChild();
}
```

## Quick Reference

- **Namespace**: `using Aspecty.AutoSignals;`
- **Constants**: Use `ASignalName.*` for common signals
- **No boilerplate**: No `_Ready()`, `Connect()`, or `Disconnect()` needed
- **Automatic cleanup**: Signals auto-disconnect when nodes exit tree
- **Dynamic support**: `dynamic: true` for runtime-created child nodes
- **Method naming**: Use `OnSignalName` convention

## Dynamic Connections Use Cases

**Perfect for:**
- Spawned game objects with timers
- Procedurally generated elements
- Runtime scene composition
- Plugin systems with dynamic nodes

**Example Pattern:**
```csharp
public partial class Spawner : Node
{
    [AutoSignal(ASignalName.Timeout, "Timer", dynamic: true)]
    private void OnTimerFinished()
    {
        // Automatically connects to any Timer added as child
    }
    
    private void SpawnObject()
    {
        var obj = new GameObject();
        var timer = new Timer();
        timer.Name = "Timer";
        obj.AddChild(timer); // AutoSignal connects automatically!
        AddChild(obj);
    }
}
```
