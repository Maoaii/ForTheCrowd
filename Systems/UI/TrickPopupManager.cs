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
    [ExportGroup("Layout")]
    [Export] public int MaxLines = 4;
    [Export] public float MaxLineWidth = 800f;
    [Export] public float LineHeight = 10f;
    [Export] public float SeparatorWidth = 10f;
    [Export] public float TextBottomMargin = 30f;

    [ExportGroup("Font")]
    [Export] public Font Font;
    [Export] public int FontSize = 8;
    [Export] public Color FontColor = new Color(1.0f, 0.498f, 0.313f);

    [ExportGroup("Tricks Animations")]
    [Export] public float TrickRotationRangeDeg = 10f;
    [Export] public float TrickScaleXMin = 1.0f;
    [Export] public float TrickScaleXMax = 1.3f;
    [Export] public float TrickScaleYMin = 1.0f;
    [Export] public float TrickScaleYMax = 1.3f;
    [Export] public float TrickScaleUpDurationS = 0.12f;
    [Export] public float TrickScaleDownDurationS = 0.1f;
    [Export] public float TrickRotationDurationS = 0.15f;
    
    [ExportGroup("Total Animations")]
    [Export] public float TotalScalePeak = 1.8f;
    [Export] public float TotalScaleFinal = 1.3f;
    [Export] public float TotalPopUpDurationS = 0.15f;
    [Export] public float TotalPopDownDurationS = 0.15f;
    
    [ExportGroup("Fade Animations")]
    [Export] public float TrickFadeOutTimeS = 1.0f;
    [Export] public float TrickFadeDriftY = -40f;

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
    
    private float _hiddenScoreTotal = 0f;
    
    private Node2D _activeContainer;
    private float _lastTimeTricked;

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

        Label label = CreateTrickLabel(trickData.ToString());
        _activeContainer.AddChild(label);
        trickData.UINode = label;

        float randomRotation = Mathf.DegToRad(
            (float)(GD.Randf() * 2.0 - 1.0) * TrickRotationRangeDeg
        );
        float peakScaleX = Mathf.Lerp(TrickScaleXMin, TrickScaleXMax, GD.Randf());
        float peakScaleY = Mathf.Lerp(TrickScaleYMin, TrickScaleYMax, GD.Randf());

        label.PivotOffset = label.GetMinimumSize() / 2.0f; // Center pivot
        label.Scale = Vector2.Zero;
        label.Rotation = randomRotation;

        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(label, "scale", new Vector2(peakScaleX, peakScaleY), TrickScaleUpDurationS)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
        
        tween.TweenProperty(label, "rotation", 0f, TrickRotationDurationS)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
             
        tween.Chain().TweenProperty(label, "scale", Vector2.One, TrickScaleDownDurationS)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.InOut);
             
        LayoutTricks(_activeTricks, _activeContainer);
        _lastTimeTricked = Time.GetTicksMsec();
    }

    public void ResetTricks()
    {
        if (_activeTricks.Count == 0 && _hiddenScoreTotal == 0f)
            return;

        float totalScore = _activeTricks.Sum(t => t.Score) + _hiddenScoreTotal;
        var totalEntry = new TrickData
        {
            Name = "TOTAL",
            Score = totalScore,
            IsTotal = true
        };
        _activeTricks.Add(totalEntry);

        Label totalLabel = CreateTrickLabel(totalEntry.ToString());
        _activeContainer.AddChild(totalLabel);
        totalEntry.UINode = totalLabel;
        
        totalLabel.PivotOffset = totalLabel.GetMinimumSize() / 2.0f;
        totalLabel.Scale = Vector2.Zero;

        Tween tween = CreateTween();
        tween.TweenProperty(totalLabel, "scale", new Vector2(TotalScalePeak, TotalScalePeak), TotalPopUpDurationS)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
             
        tween.TweenProperty(totalLabel, "scale", new Vector2(TotalScaleFinal, TotalScaleFinal), TotalPopDownDurationS)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.InOut);
             
        LayoutTricks(_activeTricks, _activeContainer);

        // Transition active container to fading sequence
        var sequence = new TrickSequence { Tricks = _activeTricks, ContainerNode = _activeContainer };
        _fadingSequences.Add(sequence);
        
        _activeTricks = new List<TrickData>();
        _hiddenScoreTotal = 0f;
        _activeContainer = new Node2D();
        _container.AddChild(_activeContainer);
        
        Tween fadeTween = CreateTween();
        fadeTween.SetParallel(true);
        fadeTween.TweenProperty(sequence.ContainerNode, "position:y", sequence.ContainerNode.Position.Y + TrickFadeDriftY, TrickFadeOutTimeS)
                 .SetTrans(Tween.TransitionType.Cubic)
                 .SetEase(Tween.EaseType.Out);
                 
        fadeTween.TweenProperty(sequence.ContainerNode, "modulate:a", 0f, TrickFadeOutTimeS)
                 .SetTrans(Tween.TransitionType.Cubic)
                 .SetEase(Tween.EaseType.In);
                 
        fadeTween.Chain().TweenCallback(Callable.From(() => {
            sequence.ContainerNode.QueueFree();
            _fadingSequences.Remove(sequence);
        }));
    }

    private Label CreateTrickLabel(string text)
    {
        Label label = new Label();
        label.Text = text;
        if (Font != null)
        {
            label.AddThemeFontOverride("font", Font);
        }
        label.AddThemeColorOverride("font_color", FontColor);
        label.AddThemeFontSizeOverride("font_size", FontSize);
        return label;
    }

    public bool StaleTricks()
    {
        return Time.GetTicksMsec() - _lastTimeTricked > 3000.0f;
    }

    private void LayoutTricks(List<TrickData> tricks, Node2D containerNode)
    {
        if (tricks.Count == 0) return;

        List<List<TrickData>> lines = BuildWrappedLines(tricks);

        float blockHeight = LineHeight * MaxLines;
        
        // Start from bottom center
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        float startY = viewportSize.Y - blockHeight - TextBottomMargin;

        for (int i = 0; i < lines.Count; i++)
        {
            List<TrickData> line = lines[i];
            float y = startY + i * LineHeight;

            float totalWidth = 0;
            foreach(var t in line)
            {
                totalWidth += t.UINode.GetMinimumSize().X;
            }
            totalWidth += (line.Count - 1) * SeparatorWidth; // Separator space

            float x = viewportSize.X / 2f - totalWidth / 2f;

            foreach (var trick in line)
            {
                Vector2 size = trick.UINode.GetMinimumSize();
                trick.UINode.Position = new Vector2(x, y);
                trick.UINode.PivotOffset = size / 2.0f;
                x += size.X + SeparatorWidth;
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
            float addedWidth = currentLine.Count == 0 ? width : currentWidth + SeparatorWidth + width;

            if (addedWidth <= MaxLineWidth || currentLine.Count == 0)
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

        if (lines.Count > MaxLines)
        {
            int linesToRemove = lines.Count - MaxLines;
            for (int i = 0; i < linesToRemove; i++)
            {
                foreach (var trick in lines[i])
                {
                    _hiddenScoreTotal += trick.Score;
                    trick.UINode?.QueueFree();
                    _activeTricks.Remove(trick);
                }
            }
            lines.RemoveRange(0, linesToRemove);
        }

        return lines;
    }
}
