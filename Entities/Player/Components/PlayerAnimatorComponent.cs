using Godot;
using CarGame.Entities.Player;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Components;

[Tool]
[GlobalClass]
public partial class PlayerAnimatorComponent : Node
{
    [Export] public BlackboardComponent Blackboard;
    [Export] public Node3D VisualsRoot;
    [Export] public PlayerAnimationConfig Config;

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
        if (VisualsRoot == null || Config == null) return;
        Tween tween = VisualsRoot.CreateTween();
        tween.TweenProperty(VisualsRoot, "scale", Config.StretchScale, Config.LaunchStretchTweenDuration)
            .SetTrans(Config.LaunchStretchTweenTrans)
            .SetEase(Config.LaunchStretchTweenEase);
            
        tween.TweenProperty(VisualsRoot, "scale", Config.DefaultScale, Config.AirTweenDuration)
            .SetTrans(Config.AirTweenTrans)
            .SetEase(Config.AirTweenEase);
    }

    public void PlayLandingAnimation()
    {
        if (VisualsRoot == null || Config == null) return;
        Tween tween = VisualsRoot.CreateTween();
        tween.TweenProperty(VisualsRoot, "scale", Config.SquashScale, Config.LandingSquashTweenDuration)
            .SetTrans(Config.LandingSquashTweenTrans)
            .SetEase(Config.LandingSquashTweenEase);
            
        tween.TweenProperty(VisualsRoot, "scale", Config.DefaultScale, Config.RecoveryTweenDuration)
            .SetTrans(Config.RecoveryTweenTrans)
            .SetEase(Config.RecoveryTweenEase);
    }
}