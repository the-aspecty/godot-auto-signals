# Changelog

All notable changes to the Auto Signals addon will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] 

### Added
- **AutoSignalProcessor Autoload**: Automatic signal processing system properly registered as Godot autoload singleton
- **Attribute-Based Signal Declaration**: `[AutoSignal]` attribute for marking signal handler methods
- **Multiple Connection Types**: Support for Normal, Deferred, and OneShot connections via `SignalConnectionType` enum
- **Path-Based Connections**: Connect to signals from any node using relative or absolute paths
- **ASignalName Constants**: Compile-time safe signal name constants for common Godot signals with IntelliSense support
- **Namespace Support**: All classes properly organized under `Aspecty.AutoSignals` namespace
management
- **Performance Optimization**: Reflection overhead minimized to initialization time only
- **Debug Logging**: Comprehensive console output with prefixed messages and connection counts
- **Error Handling**: Defensive programming with descriptive error messages and safe disconnection
- **Memory Management**: Proper tracking and cleanup of signal connections
- **LLM Instructions**: Dedicated instruction file for AI coding assistants

### Features
- **Zero Boilerplate**: No need for manual `Connect()`, `Disconnect()`, `_Ready()`, or `_ExitTree()` calls
- **Automatic Lifecycle**: Signals connected when nodes enter scene tree, disconnected when they exit
- **Type Safety**: Method signature validation ensures handlers match signal parameters
- **Node Path Resolution**: Flexible path system supporting child, parent, sibling, and absolute node references
- **Connection Tracking**: Runtime monitoring of connection counts and node statistics
- **Plugin Integration**: Proper Godot plugin with autoload registration and cleanup

### Technical Implementation
- **Autoload Singleton**: `AutoSignalProcessor` registered via `AddAutoloadSingleton()` for optimal performance
- **Reflection Caching**: Attribute scanning performed once during node initialization
- **Scene Tree Monitoring**: Automatic detection of node additions and removals
- **Memory Efficiency**: Minimal runtime overhead with proper resource cleanup
- **Error Recovery**: Graceful handling of invalid nodes, missing paths, and method signature mismatches
- `SignalManager` singleton for signal connection management
- `NodeExtensions` for convenient signal initialization
- Basic demo scene with usage examples
- Support for different connection types (Normal, Deferred, OneShot)
- Node path-based signal connections

### Usage Examples

**Basic Signal Handling:**
```csharp
using Aspecty.AutoSignals;
using Godot;

public partial class Player : Node2D
{
    [AutoSignal(ASignalName.Ready)]
    private void OnReady() => GD.Print("Player ready!");

    [AutoSignal(ASignalName.Pressed, "UI/AttackButton")]
    private void OnAttackPressed() => Attack();
}
```

**Advanced Features:**
```csharp
// Deferred connection for scene tree modifications
[AutoSignal(ASignalName.Pressed, "UI/QuitButton", SignalConnectionType.Deferred)]
private void OnQuitPressed() => GetTree().Quit();

// OneShot connection (disconnects after first use)
[AutoSignal(ASignalName.Timeout, "IntroTimer", SignalConnectionType.OneShot)]
private void OnIntroComplete() => ShowMainMenu();

// Custom signals
[Signal]
public delegate void OnPlayerDiedEventHandler();

[AutoSignal(nameof(OnPlayerDied))]
private void OnPlayerDied() => GameOver();

// Dynamic signal connection (for procedural created nodes)
[AutoSignal(ASignalName.Timeout, "Timer", dynamic: true)]


```



