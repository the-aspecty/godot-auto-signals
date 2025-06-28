using System;
using System.Collections.Generic;
using Aspecty.AutoSignals;
using Godot;

/// <summary>
/// Demo spawner that creates BlockDemo instances at regular intervals
/// Shows how AutoSignals work with dynamically created nodes
/// </summary>
public partial class BlockSpawner : Node2D
{
    [Export]
    public float SpawnInterval { get; set; } = 2.0f;

    [Export]
    public int MaxBlocks { get; set; } = 10;

    private Timer _spawnTimer;
    private PackedScene _blockScene;
    private int _currentBlockCount = 0;
    private int _totalSpawned = 0;

    // UI Elements
    private Label _titleLabel;
    private Label _statusLabel;
    private Label _countLabel;
    private Label _instructionsLabel;
    private Button _spawnButton;
    private Button _clearButton;
    private Button _pauseButton;
    private ProgressBar _capacityBar;
    private HSlider _speedSlider;
    private Label _speedLabel;
    private CheckBox _autoSpawnCheckbox;
    private bool _isPaused = false;

    public override void _Ready()
    {
        SetupUI();
        SetupSpawner();
        LoadBlockScene();

        GD.Print("[BlockSpawner] Spawner ready - AutoSignals will handle all connections");
    }

    private void SetupUI()
    {
        // Create UI layer
        var canvasLayer = new CanvasLayer();
        AddChild(canvasLayer);

        // Main container with background
        var mainPanel = new PanelContainer();
        mainPanel.Position = new Vector2(20, 20);
        mainPanel.Size = new Vector2(450, 380);
        canvasLayer.AddChild(mainPanel);

        // Background styling
        var bgStyle = new StyleBoxFlat();
        bgStyle.BgColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        bgStyle.BorderColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        bgStyle.SetBorderWidthAll(2);
        bgStyle.SetCornerRadiusAll(12);
        bgStyle.ContentMarginLeft = 20;
        bgStyle.ContentMarginRight = 20;
        bgStyle.ContentMarginTop = 15;
        bgStyle.ContentMarginBottom = 15;
        mainPanel.AddThemeStyleboxOverride("panel", bgStyle);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 10);
        mainPanel.AddChild(vbox);

        // Title with styling
        _titleLabel = new Label();
        _titleLabel.Text = "🎯 Auto Signals Block Spawner";
        _titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _titleLabel.AddThemeColorOverride("font_color", new Color(0.9f, 0.9f, 0.9f, 1.0f));
        vbox.AddChild(_titleLabel);

        // Separator
        var separator1 = new HSeparator();
        separator1.AddThemeColorOverride("separator", new Color(0.4f, 0.4f, 0.4f, 1.0f));
        vbox.AddChild(separator1);

        // Status section
        var statusContainer = new VBoxContainer();
        statusContainer.AddThemeConstantOverride("separation", 5);
        vbox.AddChild(statusContainer);

        _statusLabel = new Label();
        _statusLabel.Text = "🟢 Ready to spawn blocks!";
        _statusLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _statusLabel.AddThemeColorOverride("font_color", Colors.LightGreen);
        statusContainer.AddChild(_statusLabel);

        _countLabel = new Label();
        _countLabel.Text = "Blocks: 0/10 | Total Spawned: 0";
        _countLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _countLabel.AddThemeColorOverride("font_color", Colors.LightBlue);
        statusContainer.AddChild(_countLabel);

        // Capacity bar
        var capacityContainer = new VBoxContainer();
        capacityContainer.AddThemeConstantOverride("separation", 5);
        vbox.AddChild(capacityContainer);

        var capacityLabel = new Label();
        capacityLabel.Text = "Capacity:";
        capacityLabel.AddThemeColorOverride("font_color", Colors.White);
        capacityContainer.AddChild(capacityLabel);

        _capacityBar = new ProgressBar();
        _capacityBar.MinValue = 0;
        _capacityBar.MaxValue = MaxBlocks;
        _capacityBar.Value = 0;
        _capacityBar.ShowPercentage = false;
        capacityContainer.AddChild(_capacityBar);

        // Controls section
        var controlsContainer = new VBoxContainer();
        controlsContainer.AddThemeConstantOverride("separation", 8);
        vbox.AddChild(controlsContainer);

        // Auto-spawn checkbox
        _autoSpawnCheckbox = new CheckBox();
        _autoSpawnCheckbox.Text = "Auto Spawn";
        _autoSpawnCheckbox.ButtonPressed = true;
        _autoSpawnCheckbox.AddThemeColorOverride("font_color", Colors.White);
        controlsContainer.AddChild(_autoSpawnCheckbox);

        // Speed control
        var speedContainer = new HBoxContainer();
        speedContainer.AddThemeConstantOverride("separation", 10);
        controlsContainer.AddChild(speedContainer);

        var speedLabelStatic = new Label();
        speedLabelStatic.Text = "Speed:";
        speedLabelStatic.AddThemeColorOverride("font_color", Colors.White);
        speedContainer.AddChild(speedLabelStatic);

        _speedSlider = new HSlider();
        _speedSlider.MinValue = 0.5;
        _speedSlider.MaxValue = 5.0;
        _speedSlider.Value = 2.0;
        _speedSlider.Step = 0.1;
        _speedSlider.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        speedContainer.AddChild(_speedSlider);

        _speedLabel = new Label();
        _speedLabel.Text = "2.0s";
        _speedLabel.AddThemeColorOverride("font_color", Colors.Yellow);
        _speedLabel.CustomMinimumSize = new Vector2(40, 0);
        speedContainer.AddChild(_speedLabel);

        // Button section
        var buttonContainer = new HBoxContainer();
        buttonContainer.AddThemeConstantOverride("separation", 10);
        controlsContainer.AddChild(buttonContainer);

        _spawnButton = new Button();
        _spawnButton.Text = "🎯 Spawn Block";
        _spawnButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        SetButtonStyle(_spawnButton, Colors.DarkGreen);
        buttonContainer.AddChild(_spawnButton);

        _pauseButton = new Button();
        _pauseButton.Text = "⏸️ Pause";
        _pauseButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        SetButtonStyle(_pauseButton, Colors.DarkBlue);
        buttonContainer.AddChild(_pauseButton);

        _clearButton = new Button();
        _clearButton.Text = "🗑️ Clear All";
        _clearButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        SetButtonStyle(_clearButton, Colors.DarkRed);
        buttonContainer.AddChild(_clearButton);

        // Connect UI signals directly since these are programmatically created
        _spawnButton.Pressed += () =>
        {
            SpawnBlock();
            GD.Print("[BlockSpawner] Manual spawn requested");
        };

        _pauseButton.Pressed += () =>
        {
            TogglePause();
        };

        _clearButton.Pressed += () =>
        {
            ClearAllBlocks();
        };

        _speedSlider.ValueChanged += (double value) =>
        {
            UpdateSpawnSpeed(value);
        };

        _autoSpawnCheckbox.Toggled += (bool pressed) =>
        {
            ToggleAutoSpawn(pressed);
        };

        // Instructions
        var separator2 = new HSeparator();
        separator2.AddThemeColorOverride("separator", new Color(0.4f, 0.4f, 0.4f, 1.0f));
        vbox.AddChild(separator2);

        _instructionsLabel = new Label();
        _instructionsLabel.Text =
            "💡 Dynamic AutoSignals Demo:\n• Blocks spawn with random colors and lifespans\n• Each uses [AutoSignal] with dynamic=true\n• Timers are added as children and connected automatically\n• Try adjusting speed and toggling auto-spawn!";
        _instructionsLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _instructionsLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.8f, 0.8f, 1.0f));
        vbox.AddChild(_instructionsLabel);
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;
        _spawnTimer.Paused = _isPaused;

        if (_isPaused)
        {
            _pauseButton.Text = "▶️ Resume";
        }
        else
        {
            _pauseButton.Text = "⏸️ Pause";
        }

        UpdateUI();
        GD.Print($"[BlockSpawner] Spawning {(_isPaused ? "paused" : "resumed")}");
    }

    private void UpdateSpawnSpeed(double value)
    {
        SpawnInterval = (float)value;
        _spawnTimer.WaitTime = SpawnInterval;
        _speedLabel.Text = $"{value:F1}s";
        GD.Print($"[BlockSpawner] Spawn speed changed to {value:F1} seconds");
    }

    private void ToggleAutoSpawn(bool enabled)
    {
        if (enabled && !_spawnTimer.Autostart)
        {
            _spawnTimer.Start();
        }
        else if (!enabled)
        {
            _spawnTimer.Stop();
        }

        GD.Print($"[BlockSpawner] Auto spawn {(enabled ? "enabled" : "disabled")}");
    }

    private void SetButtonStyle(Button button, Color color)
    {
        var style = new StyleBoxFlat();
        style.BgColor = color;
        style.BorderColor = color.Lightened(0.2f);
        style.SetBorderWidthAll(1);
        style.SetCornerRadiusAll(6);
        style.ContentMarginLeft = 10;
        style.ContentMarginRight = 10;
        style.ContentMarginTop = 5;
        style.ContentMarginBottom = 5;

        var hoverStyle = new StyleBoxFlat();
        hoverStyle.BgColor = color.Lightened(0.1f);
        hoverStyle.BorderColor = color.Lightened(0.3f);
        hoverStyle.SetBorderWidthAll(2);
        hoverStyle.SetCornerRadiusAll(6);
        hoverStyle.ContentMarginLeft = 10;
        hoverStyle.ContentMarginRight = 10;
        hoverStyle.ContentMarginTop = 5;
        hoverStyle.ContentMarginBottom = 5;

        button.AddThemeStyleboxOverride("normal", style);
        button.AddThemeStyleboxOverride("hover", hoverStyle);
        button.AddThemeColorOverride("font_color", Colors.White);
    }

    private void SetupSpawner()
    {
        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Autostart = true;
        AddChild(_spawnTimer);
    }

    private void LoadBlockScene()
    {
        _blockScene = GD.Load<PackedScene>("res://demo/BlockDemo.tscn");
        if (_blockScene == null)
        {
            GD.PrintErr("[BlockSpawner] Could not load BlockDemo.tscn");
        }
    }

    private void SpawnBlock()
    {
        if (_blockScene == null || _currentBlockCount >= MaxBlocks)
            return;

        var block = _blockScene.Instantiate<BlockDemo>();
        if (block == null)
        {
            GD.PrintErr("[BlockSpawner] Failed to instantiate block");
            return;
        }

        AddChild(block);
        _currentBlockCount++;
        _totalSpawned++;

        // Connect to the block's tree_exited signal to track when it's destroyed
        block.TreeExited += OnBlockDestroyed;

        UpdateUI();

        GD.Print($"[BlockSpawner] Spawned block #{_totalSpawned} (Current: {_currentBlockCount})");
    }

    private void OnBlockDestroyed()
    {
        // Ensure we don't go below zero
        if (_currentBlockCount > 0)
        {
            _currentBlockCount--;
            UpdateUI();
            GD.Print($"[BlockSpawner] Block destroyed (Current: {_currentBlockCount})");
        }
    }

    private void UpdateUI()
    {
        _countLabel.Text =
            $"Blocks: {_currentBlockCount}/{MaxBlocks} | Total Spawned: {_totalSpawned}";
        _capacityBar.Value = _currentBlockCount;

        if (_currentBlockCount >= MaxBlocks)
        {
            _statusLabel.Text = "🔴 Maximum blocks reached!";
            _statusLabel.AddThemeColorOverride("font_color", Colors.OrangeRed);
        }
        else if (_isPaused)
        {
            _statusLabel.Text = "⏸️ Spawning paused";
            _statusLabel.AddThemeColorOverride("font_color", Colors.Yellow);
        }
        else
        {
            _statusLabel.Text = "🟢 Spawning blocks...";
            _statusLabel.AddThemeColorOverride("font_color", Colors.LightGreen);
        }
    }

    private void ClearAllBlocks()
    {
        // Find all BlockDemo children and queue them for deletion
        var blocksToRemove = new List<BlockDemo>();
        foreach (Node child in GetChildren())
        {
            if (child is BlockDemo block)
            {
                // Disconnect the TreeExited signal to prevent double counting
                block.TreeExited -= OnBlockDestroyed;
                blocksToRemove.Add(block);
            }
        }

        // Queue all blocks for deletion
        foreach (var block in blocksToRemove)
        {
            block.QueueFree();
        }

        _currentBlockCount = 0;
        UpdateUI();
        GD.Print($"[BlockSpawner] Cleared {blocksToRemove.Count} blocks");
    }

    // AutoSignal handlers for the UI
    [AutoSignal(ASignalName.Timeout, "Timer")]
    private void OnSpawnTimer()
    {
        if (_autoSpawnCheckbox?.ButtonPressed == true && !_isPaused)
        {
            SpawnBlock();
        }
    }

    // Note: UI signal connections are handled directly in SetupUI() since these are programmatically created controls
    // The dynamic AutoSignal feature is better demonstrated by the BlockDemo timer connections

    [AutoSignal(ASignalName.Ready)]
    private void OnReady()
    {
        GD.Print("[BlockSpawner] AutoSignal Ready triggered");
        UpdateUI();
    }
}
