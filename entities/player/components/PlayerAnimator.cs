using Godot;
using CarGame.Entities.Player;

namespace CarGame.Entities.Components;

public class PlayerAnimator
{
    private readonly PlayerAnimationConfig _animConfig;
    private readonly Node3D _transform;

    public PlayerAnimator(Node3D transform, PlayerAnimationConfig animConfig)
    {
        _transform = transform;
        _animConfig = animConfig;
    }

    public void PlayShoveItAnimation()
    {
        Tween tween = _transform.CreateTween();
        tween.TweenProperty(_transform, "rotation", _transform.Rotation + new Godot.Vector3(0, Mathf.DegToRad(360), 0), 0.2f)
            .SetTrans(Tween.TransitionType.Linear);
    }

    public void PlayKickflipAnimation()
    {
        Tween tween = _transform.CreateTween();
        tween.TweenProperty(_transform, "rotation", _transform.Rotation + new Godot.Vector3(0, 0, Mathf.DegToRad(360)), 0.2f)
            .SetTrans(Tween.TransitionType.Linear);
    }

    public void PlayBackManualAnimation(bool reverse = false)
    {
        Tween tween = _transform.CreateTween();
        if (reverse)
        {
            tween.TweenProperty(_transform, "rotation", new Godot.Vector3(0, 0, 0), 0.2f)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.In);
        }
        else
        {
            tween.TweenProperty(_transform, "rotation", _transform.Rotation + new Godot.Vector3(Mathf.DegToRad(30), 0, 0), 0.2f)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.In);
        }
    }

    public void PlayJumpAnimation()
    {
        Tween tween = _transform.CreateTween();
        tween.TweenProperty(_transform, "scale", _animConfig.StretchScale, _animConfig.LaunchStretchTweenDuration)
            .SetTrans(_animConfig.LaunchStretchTweenTrans)
            .SetEase(_animConfig.LaunchStretchTweenEase);
            
        tween.TweenProperty(_transform, "scale", _animConfig.DefaultScale, _animConfig.AirTweenDuration)
            .SetTrans(_animConfig.AirTweenTrans)
            .SetEase(_animConfig.AirTweenEase);
    }

    public void PlayLandingAnimation()
    {
        Tween tween = _transform.CreateTween();
        tween.TweenProperty(_transform, "scale", _animConfig.SquashScale, _animConfig.LandingSquashTweenDuration)
            .SetTrans(_animConfig.LandingSquashTweenTrans)
            .SetEase(_animConfig.LandingSquashTweenEase);
            
        tween.TweenProperty(_transform, "scale", _animConfig.DefaultScale, _animConfig.RecoveryTweenDuration)
            .SetTrans(_animConfig.RecoveryTweenTrans)
            .SetEase(_animConfig.RecoveryTweenEase);
    }
}