using Godot;
using CarGame.Entities.Player;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Components;

public partial class PlayerAnimatorComponent : Node
{
    [Export] public BlackboardComponent Blackboard;
    [Export] public Node3D VisualsRoot;

    public void PlayShoveItAnimation()
    {
        if (VisualsRoot == null) return;
        Tween tween = VisualsRoot.CreateTween();
        tween.TweenProperty(VisualsRoot, "rotation", VisualsRoot.Rotation + new Godot.Vector3(0, Mathf.DegToRad(360), 0), 0.2f)
            .SetTrans(Tween.TransitionType.Linear);
    }

    public void PlayKickflipAnimation()
    {
        if (VisualsRoot == null) return;
        Tween tween = VisualsRoot.CreateTween();
        tween.TweenProperty(VisualsRoot, "rotation", VisualsRoot.Rotation + new Godot.Vector3(0, 0, Mathf.DegToRad(360)), 0.2f)
            .SetTrans(Tween.TransitionType.Linear);
    }

    public void PlayBackManualAnimation(bool reverse = false)
    {
        if (VisualsRoot == null) return;
        Tween tween = VisualsRoot.CreateTween();
        if (reverse)
        {
            tween.TweenProperty(VisualsRoot, "rotation", new Godot.Vector3(0, 0, 0), 0.2f)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.In);
        }
        else
        {
            tween.TweenProperty(VisualsRoot, "rotation", VisualsRoot.Rotation + new Godot.Vector3(Mathf.DegToRad(30), 0, 0), 0.2f)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.In);
        }
    }

    public void PlayJumpAnimation()
    {
        if (VisualsRoot == null || Blackboard == null) return;
        var _animConfig = Blackboard.AnimationConfig;
        Tween tween = VisualsRoot.CreateTween();
        tween.TweenProperty(VisualsRoot, "scale", _animConfig.StretchScale, _animConfig.LaunchStretchTweenDuration)
            .SetTrans(_animConfig.LaunchStretchTweenTrans)
            .SetEase(_animConfig.LaunchStretchTweenEase);
            
        tween.TweenProperty(VisualsRoot, "scale", _animConfig.DefaultScale, _animConfig.AirTweenDuration)
            .SetTrans(_animConfig.AirTweenTrans)
            .SetEase(_animConfig.AirTweenEase);
    }

    public void PlayLandingAnimation()
    {
        if (VisualsRoot == null || Blackboard == null) return;
        var _animConfig = Blackboard.AnimationConfig;
        Tween tween = VisualsRoot.CreateTween();
        tween.TweenProperty(VisualsRoot, "scale", _animConfig.SquashScale, _animConfig.LandingSquashTweenDuration)
            .SetTrans(_animConfig.LandingSquashTweenTrans)
            .SetEase(_animConfig.LandingSquashTweenEase);
            
        tween.TweenProperty(VisualsRoot, "scale", _animConfig.DefaultScale, _animConfig.RecoveryTweenDuration)
            .SetTrans(_animConfig.RecoveryTweenTrans)
            .SetEase(_animConfig.RecoveryTweenEase);
    }
}