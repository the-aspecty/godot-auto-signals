using System;
using Aspecty.AutoSignals;
using Godot;

/// <summary>
/// Demo block that showcases AutoSignal with ready event and automatic cleanup
/// Each block spawns at a random position and destroys itself after a random time
/// </summary>
public partial class BlockDemo : Control
{
    private Random _random = new Random();
    private Timer _destroyTimer;
    private ColorRect _blockVisual;

    // Random colors for variety
    private readonly Color[] _colors =
    {
        Colors.Red,
        Colors.Green,
        Colors.Blue,
        Colors.Yellow,
        Colors.Purple,
        Colors.Orange,
        Colors.Cyan,
        Colors.Pink,
    };

    public override void _Ready()
    {
        // AutoSignal will handle the connection automatically
        SetupVisual();
        SetupTimer();
        SetRandomPosition();

        GD.Print($"[BlockDemo] Block created at {GlobalPosition}");
    }

    private void SetupVisual()
    {
        // Create a colored rectangle as the visual representation
        _blockVisual = new ColorRect();
        _blockVisual.Size = new Vector2(80, 80);
        _blockVisual.Color = _colors[_random.Next(_colors.Length)];
        AddChild(_blockVisual);

        // Add a border for better visibility
        var border = new StyleBoxFlat();
        border.BgColor = _blockVisual.Color;
        border.BorderColor = Colors.White;
        border.SetBorderWidthAll(2);
        border.SetCornerRadiusAll(8);

        _blockVisual.AddThemeStyleboxOverride("panel", border);

        // Add some visual effects
        var tween = CreateTween();
        tween.SetParallel(true);
        Modulate = new Color(1, 1, 1, 0);
        Scale = Vector2.Zero;

        // Fade in and scale up
        tween.TweenProperty(this, "modulate:a", 1.0f, 0.3f).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "scale", Vector2.One, 0.3f).SetEase(Tween.EaseType.Out);
    }

    private void SetupTimer()
    {
        _destroyTimer = new Timer();
        _destroyTimer.Name = "BlockTimer";
        _destroyTimer.WaitTime = _random.Next(2, 8); // Random time between 2-8 seconds
        _destroyTimer.OneShot = true;
        AddChild(_destroyTimer);
    }

    private void SetRandomPosition()
    {
        // Get viewport size for positioning
        var viewport = GetViewport();
        var viewportSize = viewport.GetVisibleRect().Size;

        // Random position within viewport bounds (with some margin)
        var margin = 100;
        var randomX = _random.Next(margin, (int)viewportSize.X - margin - 80);
        var randomY = _random.Next(margin, (int)viewportSize.Y - margin - 80);

        GlobalPosition = new Vector2(randomX, randomY);
    }

    // AutoSignal handler for ready event
    [AutoSignal(ASignalName.Ready)]
    private void OnAutoReady()
    {
        GD.Print($"[BlockDemo] AutoSignal Ready triggered for block at {GlobalPosition}");
    }

    // AutoSignal handler for the timer timeout - this destroys the block
    [AutoSignal(ASignalName.Timeout, "BlockTimer", SignalConnectionType.Deferred, dynamic: true)]
    private void OnDestroyTimerTimeout()
    {
        GD.Print($"[BlockDemo] Timer timeout - destroying block at {GlobalPosition}");
        DestroyBlock();
    }

    private void DestroyBlock()
    {
        // Animate destruction
        var tween = CreateTween();
        tween.SetParallel(true);

        // Fade out and scale down
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.3f).SetEase(Tween.EaseType.In);
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.3f).SetEase(Tween.EaseType.In);

        // Queue free after animation
        tween.TweenCallback(new Callable(this, MethodName.QueueFree)).SetDelay(0.3f);
    }

    // Start the timer when ready (called by AutoSignal)
    [AutoSignal(ASignalName.Ready)]
    private void StartDestroyTimer()
    {
        if (_destroyTimer != null)
        {
            _destroyTimer.Start();
            GD.Print(
                $"[BlockDemo] Destroy timer started - will destroy in {_destroyTimer.WaitTime} seconds"
            );
        }
    }
}
