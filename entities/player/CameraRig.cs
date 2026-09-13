using Godot;
using System;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Player;

public partial class CameraRig : Node3D
{
    [Export] public Node3D Target { get; set; }
    
    private SpringArm3D _springArm;
    private Camera3D _camera;
    private Blackboard _blackboard;
    
    public float OrbitalRotation { get; set; }
    public float HeightRotation { get; set; }
    public float DistanceFromTarget { get; set; } = 35.0f; // Constants.CAMERA_DISTANCE;
    
    private float _targetOrbitalRotation;
    private float _targetHeightRotation;
    
    private bool _isMouseLocked = true;
    
    public override void _Ready()
    {
        TopLevel = true;
        
        _springArm = GetNode<SpringArm3D>("SpringArm3D");
        _camera = GetNode<Camera3D>("SpringArm3D/Camera3D");
        _springArm.SpringLength = DistanceFromTarget;
        
        // Initial FOV
        _camera.Fov = Mathf.RadToDeg(Mathf.Pi / 4.0f); // Constants.CAMERA_FOV
        
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    
    public void SetBlackboard(Blackboard blackboard)
    {
        _blackboard = blackboard;
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
            float sensitivity = 0.006f; // Double the original sensitivity
            int invertY = -1; // Constants.MOUSE_INVERTED equivalent based on existing logic
            int invertX = -1;

            _targetOrbitalRotation += mouseMotion.Relative.X * sensitivity * invertX;
            _targetHeightRotation = Mathf.Clamp(
                _targetHeightRotation - mouseMotion.Relative.Y * sensitivity * invertY, 
                Mathf.Pi / 6.0f, // Constants.CAMERA_YAW_MIN
                Mathf.Pi / 2.0f - 0.1f // Constants.CAMERA_YAW_MAX
            );
        }
    }
    
    public override void _Process(double delta)
    {
        if (Target == null) return;
        
        GlobalPosition = Target.GlobalPosition;
        
        if (_blackboard != null)
        {
            TryApplyFOVKick();
        }
        
        float dt = (float)delta;
        float lagFactor = -12.0f; // Constants.CAMERA_LAG_FACTOR
        float lerpAmount = 1.0f - Mathf.Exp(lagFactor * dt);
        
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
        if (!_blackboard.Camera.WantsFOVKick) return;
        
        Tween tween = CreateTween();
        
        float defaultFov = Mathf.RadToDeg(Mathf.Pi / 4.0f);
        float kickFov = Mathf.RadToDeg(_blackboard.Camera.FOVKick);
        
        tween.TweenProperty(_camera, "fov", kickFov, _blackboard.Camera.FOVKickUpTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
            
        tween.TweenProperty(_camera, "fov", defaultFov, _blackboard.Camera.FOVKickDownTime)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
            
        _blackboard.Camera.WantsFOVKick = false;
    }
}
