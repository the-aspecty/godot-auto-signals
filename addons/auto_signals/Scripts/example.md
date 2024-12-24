# Auto Signals

Automatically connect Godot signals using attributes:

Feature:
```cs
[AutoSignal("OnReady")]
public void HandleOnReady()
{
    // Do something when the signal is emitted
}
```

how it was used before:
```cs
public void Setup()
{
    Node.OnReady += HandleOnReady;
}
```



