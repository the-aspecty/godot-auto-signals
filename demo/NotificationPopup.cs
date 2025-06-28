using Godot;

/// <summary>
/// Simple notification popup for the demo
/// This is a concrete implementation example, not part of the addon
/// </summary>
public partial class NotificationPopup : PanelContainer
{
    private Label _label;
    private Timer _autoHideTimer;

    public override void _Ready()
    {
        SetupUI();
        SetupTimer();
    }

    private void SetupUI()
    {
        // Create the label
        _label = new Label();
        _label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _label.HorizontalAlignment = HorizontalAlignment.Center;
        _label.VerticalAlignment = VerticalAlignment.Center;
        AddChild(_label);

        // Set up the panel style
        var styleBox = new StyleBoxFlat();
        styleBox.BgColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        styleBox.BorderColor = Colors.White;
        styleBox.BorderWidthLeft = 2;
        styleBox.BorderWidthRight = 2;
        styleBox.BorderWidthTop = 2;
        styleBox.BorderWidthBottom = 2;
        styleBox.CornerRadiusTopLeft = 8;
        styleBox.CornerRadiusTopRight = 8;
        styleBox.CornerRadiusBottomLeft = 8;
        styleBox.CornerRadiusBottomRight = 8;
        styleBox.ContentMarginLeft = 16;
        styleBox.ContentMarginRight = 16;
        styleBox.ContentMarginTop = 8;
        styleBox.ContentMarginBottom = 8;

        AddThemeStyleboxOverride("panel", styleBox);

        // Set initial state
        Modulate = new Color(1, 1, 1, 0);
        Position = new Vector2(50, 0);
    }

    private void SetupTimer()
    {
        _autoHideTimer = new Timer();
        _autoHideTimer.WaitTime = 3.0f;
        _autoHideTimer.OneShot = true;
        _autoHideTimer.Timeout += OnAutoHideTimeout;
        AddChild(_autoHideTimer);
    }

    public void ShowNotification(
        string message,
        Color textColor = default,
        Color borderColor = default
    )
    {
        if (textColor == default)
            textColor = Colors.White;
        if (borderColor == default)
            borderColor = Colors.LightGray;

        _label.Text = message;
        _label.Modulate = textColor;

        // Update border color
        var styleBox = GetThemeStylebox("panel") as StyleBoxFlat;
        if (styleBox != null)
        {
            styleBox.BorderColor = borderColor;
        }

        // Animate appearance
        var tween = CreateTween();
        tween.SetParallel(true);

        // Fade in and slide in
        tween.TweenProperty(this, "modulate:a", 1.0f, 0.3f).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "position:x", 0.0f, 0.3f).SetEase(Tween.EaseType.Out);

        // Start auto-hide timer
        _autoHideTimer.Start();
    }

    private void OnAutoHideTimeout()
    {
        HideNotification();
    }

    public void HideNotification()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.3f).SetEase(Tween.EaseType.In);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree)).SetDelay(0.3f);
    }
}
