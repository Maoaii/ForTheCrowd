using Godot;
using System;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Player;

public partial class CameraRig : Node3D
{
    [Export] public Node3D Target { get; set; }
    
    private SpringArm3D _springArm;
    private Camera3D _camera;
    [Export] public BlackboardComponent Blackboard;
    
    public float OrbitalRotation { get; set; }
    public float HeightRotation { get; set; }
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
    
    private float _targetOrbitalRotation;
    private float _targetHeightRotation;
    
    private bool _isMouseLocked = true;
    
    public override void _Ready()
    {
        TopLevel = true;
        
        _springArm = GetNode<SpringArm3D>("SpringArm3D");
        _camera = GetNode<Camera3D>("SpringArm3D/Camera3D");
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
        
        if (Blackboard != null)
        {
            TryApplyFOVKick();
        }
        
        float dt = (float)delta;
        float lerpAmount = 1.0f - Mathf.Exp(LagFactor * dt);
        
        OrbitalRotation = Mathf.LerpAngle(OrbitalRotation, _targetOrbitalRotation, lerpAmount);
        HeightRotation = Mathf.Lerp(HeightRotation, _targetHeightRotation, lerpAmount);
        
        // Apply rotation to this node
        Basis basis = Basis.Identity;
        basis = basis.Rotated(Vector3.Up, OrbitalRotation);
        basis = basis.Rotated(basis.X, -HeightRotation); // Note: Godot's Y is up, X is right. Rotating around local X for height.
        Basis = basis;
    }
    
    private void TryApplyFOVKick()
    {
        if (!Blackboard.Camera.WantsFOVKick) return;
        
        Tween tween = CreateTween();
        
        float defaultFov = Mathf.RadToDeg(BaseFov);
        float kickFov = Mathf.RadToDeg(Blackboard.Camera.FOVKick);
        
        tween.TweenProperty(_camera, "fov", kickFov, Blackboard.Camera.FOVKickUpTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
            
        tween.TweenProperty(_camera, "fov", defaultFov, Blackboard.Camera.FOVKickDownTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
            
        Blackboard.Camera.WantsFOVKick = false;
    }
}
