using Godot;
using System;
using CarGame.Systems.Blackboards;
using System.Threading.Tasks;

namespace CarGame.Entities.Player;

public partial class CameraRig : Node3D
{
    [Export] public Node3D Target { get; set; }
    [Export] public BlackboardComponent Blackboard;
    
    [ExportGroup("Distance & Feel")]
    [Export] public float DistanceFromTarget { get; set; } = 35.0f;
    [Export] public float LagFactor { get; set; } = -12.0f;
    
    [ExportGroup("Input Settings")]
    [Export] public float Sensitivity { get; set; } = 0.006f;
    [Export] public int InvertX { get; set; } = -1;
    [Export] public int InvertY { get; set; } = -1;
    
    [ExportGroup("Limits")]
    [Export] public float MinYaw { get; set; } = Mathf.Pi / 6.0f;
    [Export] public float MaxYaw { get; set; } = (Mathf.Pi / 2.0f) - 0.1f;
    
    [ExportGroup("FOV")]
    [Export] public float BaseFov { get; set; } = Mathf.Pi / 4.0f;
    [Export] public float SpeedFovDegrees { get; set; } = 6.0f;
    [ExportSubgroup("Boost FOV")]
    [Export] public float BoostFovDegrees { get; set; } = 6.0f;
    [Export] public float BoostFovAttackTime { get; set; } = 0.6f;
    [Export] public float BoostFovReleaseTime { get; set; } = 0.4f;
    [Export] public Curve BoostFovAttackCurve { get; set; }
    [Export] public Curve BoostFovReleaseCurve { get; set; }

    [ExportGroup("Camera Shake")]
    [Export] public float ShakeMagnitude { get; set; } = 0.2f;

    [ExportGroup("References")]
    [Export] private SpringArm3D _springArm;
    [Export] private Camera3D _camera;

    public float OrbitalRotation { get; set; }
    public float HeightRotation { get; set; }
    private float _targetOrbitalRotation;
    private float _targetHeightRotation;
    
    private bool _isMouseLocked = true;
    private float _kickOffsetDegrees = 0.0f;

    private Tween _boostTween;
    private float _boostFovOffset;
    private bool _wasBoosting;
    
    public override void _Ready()
    {
        TopLevel = true;
        
        _springArm.SpringLength = DistanceFromTarget;
        
        _camera.Fov = Mathf.RadToDeg(BaseFov);
        
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Keycode == Key.Escape && keyEvent.Pressed)
        {
            GetTree().Quit();
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Right)
        {
            if (mouseButton.IsPressed())
            {
                _isMouseLocked = !_isMouseLocked;
                Input.MouseMode = _isMouseLocked ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
            }
        }
        
        if (_isMouseLocked && @event is InputEventMouseMotion mouseMotion)
        {
            _targetOrbitalRotation += mouseMotion.Relative.X * Sensitivity * InvertX;
            _targetHeightRotation = Mathf.Clamp(
                _targetHeightRotation - mouseMotion.Relative.Y * Sensitivity * InvertY, 
                MinYaw, 
                MaxYaw
            );
        }
    }
    
    public override void _Process(double delta)
    {
        if (Target == null) return;
        
        GlobalPosition = Target.GlobalPosition;
        
        UpdateBoostFOV();
        ApplyFOV();
        _ = TryApplyCameraShake(delta);
        
        float dt = (float)delta;
        float lerpAmount = 1.0f - Mathf.Exp(LagFactor * dt);
        
        OrbitalRotation = Mathf.LerpAngle(OrbitalRotation, _targetOrbitalRotation, lerpAmount);
        HeightRotation = Mathf.Lerp(HeightRotation, _targetHeightRotation, lerpAmount);
        
        // Apply rotation to this node
        Basis basis = Basis.Identity;
        basis = basis.Rotated(Vector3.Up, OrbitalRotation);
        basis = basis.Rotated(basis.X, -HeightRotation);
        Basis = basis;
    }

    private void UpdateBoostFOV()
    {
        bool boosting = Blackboard.State.IsBoosting;
        if (boosting == _wasBoosting) return;

        _wasBoosting = boosting;
        StartBoostTween(boosting);
    }

    private void StartBoostTween(bool boosting)
    {
        _boostTween?.Kill();

        float start = _boostFovOffset;
        float end = boosting ? 1.0f : 0.0f;
        Curve curve = boosting ? BoostFovAttackCurve : BoostFovReleaseCurve;
        float duration = boosting ? BoostFovAttackTime : BoostFovReleaseTime;

        _boostTween = CreateTween();
        _boostTween.TweenMethod(
            Callable.From((float t) =>
            {
                float shaped = curve.SampleBaked(t);
                _boostFovOffset = Mathf.Lerp(start, end, shaped);
            }),
            0.0f, 1.0f, duration
        );
    }

    private void ApplyFOV()
    {
        TryApplyFOVKick();

        float fov = Mathf.RadToDeg(BaseFov) + CalculateSpeedTerm()  + CalculateBoostTerm() + _kickOffsetDegrees;
        _camera.Fov = Mathf.Clamp(fov, 1.0f, 179.0f);
    }

    private float CalculateSpeedTerm()
    {
        return SpeedFovDegrees * Mathf.Clamp(Mathf.Abs(Blackboard.Kinematics.Speed) / Blackboard.MovementConfig.MaxSpeedForward, 0, 1);
    }

    private float CalculateBoostTerm()
    {
        return BoostFovDegrees * _boostFovOffset;
    }
    
    private void TryApplyFOVKick()
    {
        if (!Blackboard.Camera.WantsFOVKick) return;
        
        Tween tween = CreateTween();
        
        tween.TweenMethod(
            Callable.From((float value) => _kickOffsetDegrees = value),
            0,
            Blackboard.Camera.FOVKick,
            Blackboard.Camera.FOVKickUpTime
        )
        .SetTrans(Tween.TransitionType.Cubic)
        .SetEase(Tween.EaseType.Out);

        tween.TweenMethod(
            Callable.From((float value) => _kickOffsetDegrees = value),
            Blackboard.Camera.FOVKick,
            0,
            Blackboard.Camera.FOVKickDownTime
        )
        .SetTrans(Tween.TransitionType.Cubic)
        .SetEase(Tween.EaseType.InOut);
            
        Blackboard.Camera.WantsFOVKick = false;
    }

    private async Task TryApplyCameraShake(double delta)
    {
        if (!Blackboard.Camera.WantsCameraShake) return;
        Blackboard.Camera.WantsCameraShake = false;
        
        Transform3D initial_transform = this.Transform;
        float elapsed_time = 0.0f;

        while(elapsed_time < Blackboard.Camera.CameraShakeTime)
        {
            var offset = new Vector3(
                (float)GD.RandRange(-ShakeMagnitude, ShakeMagnitude),
                (float)GD.RandRange(-ShakeMagnitude, ShakeMagnitude),
                0.0f
            );

            Transform3D new_transform = initial_transform;
            new_transform.Origin += offset;
            Transform = new_transform;

            elapsed_time += (float)GetProcessDeltaTime();

            await ToSignal(GetTree(), "process_frame");
        }

        Transform = initial_transform;
    }
}
