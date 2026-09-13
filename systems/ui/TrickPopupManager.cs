using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CarGame.Systems.UI;

public class TrickData 
{
    public string Name { get; set; }
    public float Score { get; set; }
    public Label UINode { get; set; }
    public bool IsTotal { get; set; } = false;

    public override string ToString() => $"{Name} ({Score})";
}

public partial class TrickPopupManager : CanvasLayer
{
    private const int MAX_LINES = 4;
    private const float MAX_LINE_WIDTH = 300f;
    private const string TRICK_SEPARATOR = " ";
    private const float TRICK_TEXT_BOTTOM_MARGIN = 10f;
    private const float TRICK_ROTATION_RANGE_DEG = 10f;
    private const float TRICK_SCALE_X_MIN = 0.5f, TRICK_SCALE_X_MAX = 0.7f;
    private const float TRICK_SCALE_Y_MIN = 0.5f, TRICK_SCALE_Y_MAX = 0.7f;
    private const float TRICK_FADE_OUT_TIME_S = 1.0f;
    private const float TRICK_SCALE_UP_DURATION_S = 0.12f;
    private const float TRICK_SCALE_DOWN_DURATION_S = 0.1f;
    private const float TRICK_ROTATION_DURATION_S = 0.15f;
    private const float TOTAL_SCALE_PEAK = 1.8f;
    private const float TOTAL_SCALE_FINAL = 1.0f;
    private const float TOTAL_POP_UP_DURATION_S = 0.15f;
    private const float TOTAL_POP_DOWN_DURATION_S = 0.15f;
    private const float TRICK_FADE_DRIFT_Y = -40f;

    private static TrickPopupManager _instance;
    public static TrickPopupManager Instance => _instance;
    
    private Control _container;

    private class TrickSequence
    {
        public List<TrickData> Tricks = new();
        public Node2D ContainerNode;
    }

    private List<TrickSequence> _fadingSequences = new();
    private List<TrickData> _activeTricks = new();
    
    private Node2D _activeContainer;
    
    // We'll use a dynamic font or default theme font.

    public override void _Ready()
    {
        _instance = this;
        _container = new Control();
        _container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _container.MouseFilter = Control.MouseFilterEnum.Ignore;
        AddChild(_container);
        
        _activeContainer = new Node2D();
        _container.AddChild(_activeContainer);
    }

    public void PopupTrick(string trickName, float score)
    {
        var trickData = new TrickData { Name = trickName, Score = score };
        _activeTricks.Add(trickData);

        Label label = new Label();
        label.Text = trickData.ToString();
        label.AddThemeColorOverride("font_color", new Color(1.0f, 0.498f, 0.313f)); // Coral
        label.AddThemeFontSizeOverride("font_size", 8);
        _activeContainer.AddChild(label);
        trickData.UINode = label;

        float randomRotation = Mathf.DegToRad(
            (float)(GD.Randf() * 2.0 - 1.0) * TRICK_ROTATION_RANGE_DEG
        );
        float peakScaleX = Mathf.Lerp(TRICK_SCALE_X_MIN, TRICK_SCALE_X_MAX, GD.Randf());
        float peakScaleY = Mathf.Lerp(TRICK_SCALE_Y_MIN, TRICK_SCALE_Y_MAX, GD.Randf());

        label.PivotOffset = label.GetMinimumSize() / 2.0f; // Center pivot
        label.Scale = Vector2.Zero;
        label.Rotation = randomRotation;

        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(label, "scale", new Vector2(peakScaleX, peakScaleY), TRICK_SCALE_UP_DURATION_S)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
        
        tween.TweenProperty(label, "rotation", 0f, TRICK_ROTATION_DURATION_S)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
             
        tween.Chain().TweenProperty(label, "scale", Vector2.One, TRICK_SCALE_DOWN_DURATION_S)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.InOut);
             
        LayoutTricks(_activeTricks, _activeContainer);
    }

    public void ResetTricks()
    {
        if (_activeTricks.Count == 0)
            return;

        float totalScore = _activeTricks.Sum(t => t.Score);
        var totalEntry = new TrickData
        {
            Name = "TOTAL",
            Score = totalScore,
            IsTotal = true
        };
        _activeTricks.Add(totalEntry);

        Label totalLabel = new Label();
        totalLabel.Text = totalEntry.ToString();
        totalLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.498f, 0.313f));
        totalLabel.AddThemeFontSizeOverride("font_size", 8);
        _activeContainer.AddChild(totalLabel);
        totalEntry.UINode = totalLabel;
        
        totalLabel.PivotOffset = totalLabel.GetMinimumSize() / 2.0f;
        totalLabel.Scale = Vector2.Zero;

        Tween tween = CreateTween();
        tween.TweenProperty(totalLabel, "scale", new Vector2(TOTAL_SCALE_PEAK, TOTAL_SCALE_PEAK), TOTAL_POP_UP_DURATION_S)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
             
        tween.TweenProperty(totalLabel, "scale", new Vector2(TOTAL_SCALE_FINAL, TOTAL_SCALE_FINAL), TOTAL_POP_DOWN_DURATION_S)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.InOut);
             
        LayoutTricks(_activeTricks, _activeContainer);

        // Transition active container to fading sequence
        var sequence = new TrickSequence { Tricks = _activeTricks, ContainerNode = _activeContainer };
        _fadingSequences.Add(sequence);
        
        _activeTricks = new List<TrickData>();
        _activeContainer = new Node2D();
        _container.AddChild(_activeContainer);
        
        Tween fadeTween = CreateTween();
        fadeTween.SetParallel(true);
        fadeTween.TweenProperty(sequence.ContainerNode, "position:y", sequence.ContainerNode.Position.Y + TRICK_FADE_DRIFT_Y, TRICK_FADE_OUT_TIME_S)
                 .SetTrans(Tween.TransitionType.Cubic)
                 .SetEase(Tween.EaseType.Out);
                 
        fadeTween.TweenProperty(sequence.ContainerNode, "modulate:a", 0f, TRICK_FADE_OUT_TIME_S)
                 .SetTrans(Tween.TransitionType.Cubic)
                 .SetEase(Tween.EaseType.In);
                 
        fadeTween.Chain().TweenCallback(Callable.From(() => {
            sequence.ContainerNode.QueueFree();
            _fadingSequences.Remove(sequence);
        }));
    }

    private void LayoutTricks(List<TrickData> tricks, Node2D containerNode)
    {
        if (tricks.Count == 0) return;

        List<List<TrickData>> lines = BuildWrappedLines(tricks);
        float lineHeight = 10f; // Approx height

        float blockHeight = lineHeight * MAX_LINES;
        
        // Start from bottom center
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        float startY = viewportSize.Y - blockHeight - TRICK_TEXT_BOTTOM_MARGIN;

        for (int i = 0; i < lines.Count; i++)
        {
            List<TrickData> line = lines[i];
            float y = startY + i * lineHeight;

            float totalWidth = 0;
            foreach(var t in line)
            {
                totalWidth += t.UINode.GetMinimumSize().X;
            }
            totalWidth += (line.Count - 1) * 10f; // Separator space

            float x = viewportSize.X / 2f - totalWidth / 2f;

            foreach (var trick in line)
            {
                Vector2 size = trick.UINode.GetMinimumSize();
                trick.UINode.Position = new Vector2(x, y);
                trick.UINode.PivotOffset = size / 2.0f;
                x += size.X + 10f;
            }
        }
    }

    private List<List<TrickData>> BuildWrappedLines(List<TrickData> tricks)
    {
        var lines = new List<List<TrickData>>();
        var currentLine = new List<TrickData>();
        float currentWidth = 0f;

        foreach (var trick in tricks)
        {
            if (trick.IsTotal && currentLine.Count > 0)
            {
                lines.Add(currentLine);
                currentLine = new List<TrickData>();
                currentWidth = 0f;
            }

            float width = trick.UINode.GetMinimumSize().X;
            float separatorWidth = 10f;
            float addedWidth = currentLine.Count == 0 ? width : currentWidth + separatorWidth + width;

            if (addedWidth <= MAX_LINE_WIDTH || currentLine.Count == 0)
            {
                currentLine.Add(trick);
                currentWidth = addedWidth;
            }
            else
            {
                lines.Add(currentLine);
                currentLine = new List<TrickData> { trick };
                currentWidth = width;
            }
        }

        if (currentLine.Count > 0)
            lines.Add(currentLine);

        if (lines.Count > MAX_LINES)
            lines.RemoveRange(0, lines.Count - MAX_LINES);

        return lines;
    }
}
