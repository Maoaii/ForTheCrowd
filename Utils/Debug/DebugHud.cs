using Godot;
using CarGame.Entities.Enemies.Swarm;

namespace CarGame.Utils.Debug;

[GlobalClass]
public partial class DebugHud : CanvasLayer
{
    [Export] public Key ToggleKey = Key.Key0;
    [Export] public double UpdateInterval = 0.1;
    [Export] public SwarmManager Swarm;

    [ExportGroup("Font")]
    [Export] public Font Font;
    [Export] public int FontSize = 8;

    private PanelContainer _panel;
    private Label _headerLabel;
    private Label _fpsLabel;
    private Label _entitiesLabel;
    private Label _perfLabel;

    private double _timeSinceUpdate = 0.0;

    public override void _Ready()
    {
        Layer = 100;
        Visible = false;
        BuildUI();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.Keycode == ToggleKey)
            {
                Visible = !Visible;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;

        _timeSinceUpdate += delta;
        if (_timeSinceUpdate >= UpdateInterval)
        {
            _timeSinceUpdate = 0.0;
            RefreshMetrics();
        }
    }

    private void BuildUI()
    {
        _panel = new PanelContainer();
        _panel.Name = "DebugHudPanel";
        _panel.MouseFilter = Control.MouseFilterEnum.Ignore;

        // Anchor top-right
        _panel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _panel.GrowHorizontal = Control.GrowDirection.Begin; // Grow leftward from top-right anchor
        _panel.GrowVertical = Control.GrowDirection.End;
        _panel.OffsetTop = 4;
        _panel.OffsetRight = -4;

        // Style background panel
        StyleBoxFlat styleBox = new StyleBoxFlat
        {
            BgColor = new Color(0.04f, 0.04f, 0.07f, 0.75f),
            CornerRadiusTopLeft = 3,
            CornerRadiusTopRight = 3,
            CornerRadiusBottomLeft = 3,
            CornerRadiusBottomRight = 3,
            ContentMarginLeft = 5,
            ContentMarginRight = 5,
            ContentMarginTop = 3,
            ContentMarginBottom = 3
        };
        _panel.AddThemeStyleboxOverride("panel", styleBox);

        VBoxContainer vbox = new VBoxContainer();
        vbox.MouseFilter = Control.MouseFilterEnum.Ignore;
        vbox.AddThemeConstantOverride("separation", 1);
        _panel.AddChild(vbox);

        _headerLabel = CreateLabel("[DEBUG HUD] (Key 0)", new Color(0.6f, 0.8f, 1.0f));
        _fpsLabel = CreateLabel("FPS: --", Colors.Green);
        _entitiesLabel = CreateLabel("Entities: --", Colors.White);
        _perfLabel = CreateLabel("Perf: --", new Color(0.85f, 0.85f, 0.85f));

        vbox.AddChild(_headerLabel);
        vbox.AddChild(_fpsLabel);
        vbox.AddChild(_entitiesLabel);
        vbox.AddChild(_perfLabel);

        AddChild(_panel);
    }

    private Label CreateLabel(string defaultText, Color color)
    {
        Label label = new Label
        {
            Text = defaultText,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        if (Font != null)
        {
            label.AddThemeFontOverride("font", Font);
        }
        label.AddThemeFontSizeOverride("font_size", FontSize);
        label.AddThemeColorOverride("font_color", color);
        return label;
    }

    private void RefreshMetrics()
    {
        // 1. FPS & Frame Time
        double fps = Performance.GetMonitor(Performance.Monitor.TimeFps);
        double processTimeMs = Performance.GetMonitor(Performance.Monitor.TimeProcess) * 1000.0;
        _fpsLabel.Text = $"FPS: {fps:F0} ({processTimeMs:F1} ms)";

        if (fps >= 55)
            _fpsLabel.AddThemeColorOverride("font_color", new Color(0.25f, 0.95f, 0.35f));
        else if (fps >= 30)
            _fpsLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.85f, 0.2f));
        else
            _fpsLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.3f, 0.3f));

        // 2. Entity & Node Counts
        int swarmCount = Swarm?.EntityCount ?? 0;

        int totalNodes = (int)Performance.GetMonitor(Performance.Monitor.ObjectNodeCount);
        int orphanNodes = (int)Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount);

        string orphanStr = orphanNodes > 0 ? $" ({orphanNodes} orph)" : "";
        _entitiesLabel.Text = $"Zombies: {swarmCount} | Nodes: {totalNodes}{orphanStr}";

        // 3. Performance & Memory
        double physicsTimeMs = Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess) * 1000.0;
        int drawCalls = (int)Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);
        double memMb = Performance.GetMonitor(Performance.Monitor.MemoryStatic) / (1024.0 * 1024.0);

        _perfLabel.Text = $"Phys: {physicsTimeMs:F1}ms | Draw: {drawCalls} | {memMb:F1}MB";
    }
}
