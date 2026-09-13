using Godot;
using System;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Player;

public partial class Player : CharacterBody3D
{
	private Blackboard _blackboard;
	private PlayerInputTranslator _inputTranslator;
	private CameraRig _cameraRig;
	
	private PlayerStateMachine _stateMachine;
	
    private CarGame.Entities.Components.TireMarks _tireMarksL;
    private CarGame.Entities.Components.TireMarks _tireMarksR;
    
    private GpuParticles3D _landDustFL;
    private GpuParticles3D _landDustFR;
    private GpuParticles3D _landDustRL;
    private GpuParticles3D _landDustRR;
    
    private GpuParticles3D _driftDustRL;
    private GpuParticles3D _driftDustRR;
    
    public override void _Ready()
    {
        _blackboard = new Blackboard();
        _blackboard.Owner = this;
        _blackboard.MovementConfig = MovementConfigs.Default;
        _blackboard.AnimationConfig = PlayerAnimationConfigs.Default;
        
        _inputTranslator = new PlayerInputTranslator(_blackboard);
        
        _cameraRig = GetNode<CameraRig>("CameraRig");
        if (_cameraRig != null)
        {
            _cameraRig.Target = this;
            _cameraRig.SetBlackboard(_blackboard);
        }
        
        _blackboard.Animator = new Components.PlayerAnimator(GetNode<Node3D>("Visuals/MeshInstance3D"), _blackboard.AnimationConfig);
        
        _tireMarksL = new Components.TireMarks();
        _tireMarksL.LocalOffset = new Godot.Vector3(0.8f, -0.4f, 0.85f);
        AddChild(_tireMarksL);
        _tireMarksL.Init(_blackboard);
        
        _tireMarksR = new Components.TireMarks();
        _tireMarksR.LocalOffset = new Godot.Vector3(-0.8f, -0.4f, 0.85f);
        AddChild(_tireMarksR);
        _tireMarksR.Init(_blackboard);
        
        _landDustFL = GetNode<GpuParticles3D>("VFX/LandingDustFL");
        _landDustFR = GetNode<GpuParticles3D>("VFX/LandingDustFR");
        _landDustRL = GetNode<GpuParticles3D>("VFX/LandingDustRL");
        _landDustRR = GetNode<GpuParticles3D>("VFX/LandingDustRR");
        
        _driftDustRL = GetNode<GpuParticles3D>("VFX/DriftDustRL");
        _driftDustRR = GetNode<GpuParticles3D>("VFX/DriftDustRR");
        
        _blackboard.Events.OnLanded += HandleLanding;
        _blackboard.Events.OnDrifting += HandleDrifting;
        
        _stateMachine = new PlayerStateMachine(_blackboard);
        var idleState = new IdleState();
        _stateMachine.RegisterState(idleState);
        _stateMachine.RegisterState(new MovingState());
        _stateMachine.RegisterState(new AirState());
        _stateMachine.RegisterState(new OllieState());
        _stateMachine.RegisterState(new KickflipState());
        _stateMachine.RegisterState(new ShoveItState());
        _stateMachine.RegisterState(new BackManualState());
        
        _stateMachine.SetInitialState(idleState);
	}

    private void HandleLanding(float impactVelocity)
    {
        _landDustFL.Restart();
        _landDustFR.Restart();
        _landDustRL.Restart();
        _landDustRR.Restart();
    }
    
    private void HandleDrifting(float driftFactor)
    {
        _driftDustRL.Emitting = true;
        _driftDustRR.Emitting = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        
        _inputTranslator.Update(delta);
        
        _driftDustRL.Emitting = false;
        _driftDustRR.Emitting = false;
        
        _blackboard.State.WasOnGround = _blackboard.State.IsOnGround;
		_blackboard.State.IsOnGround = IsOnFloor();
		
		_stateMachine.Update(delta);
		
		if (_blackboard.State.WantsGroundPhysics)
		{
			PlayerPhysicsHelper.ApplyGroundPhysics(_blackboard, dt);
		}
		
		Move(dt);
	}
	
	private void Move(float dt)
	{
		if (_blackboard.State.CanMove)
		{
			float yawRate = _blackboard.Kinematics.Speed / _blackboard.MovementConfig.Wheelbase * Mathf.Tan(Mathf.DegToRad(_blackboard.Kinematics.SteerAngle));
			Rotation = new Godot.Vector3(Rotation.X, Rotation.Y + yawRate * dt, Rotation.Z);
		}

		Godot.Vector3 targetDirection = -Transform.Basis.Z;
		float catchUpRate = Mathf.Lerp(
			_blackboard.MovementConfig.HighGripRate, 
			_blackboard.MovementConfig.LowGripRate, 
			_blackboard.Kinematics.DriftFactor
		);

		if (_blackboard.Kinematics.MovementDirection == Godot.Vector3.Zero)
			_blackboard.Kinematics.MovementDirection = targetDirection;
			
		_blackboard.Kinematics.MovementDirection = _blackboard.Kinematics.MovementDirection.Lerp(targetDirection, catchUpRate * dt);
		_blackboard.Kinematics.MovementDirection = _blackboard.Kinematics.MovementDirection.Normalized();

        float gravityComponent = _blackboard.Kinematics.Velocity.Y;

        Godot.Vector3 newVelocity = _blackboard.Kinematics.MovementDirection * _blackboard.Kinematics.Speed;
        
        if (!_blackboard.State.IsOnGround)
        {
            gravityComponent -= _blackboard.MovementConfig.GravityForce * _blackboard.MovementConfig.FallMultiplier * dt;
        }
        
        newVelocity.Y = gravityComponent * _blackboard.Kinematics.GravityToggler;
        
        _blackboard.Kinematics.Velocity = newVelocity;
        Velocity = newVelocity;
        
        MoveAndSlide();
        
        Godot.Vector3 updatedVel = _blackboard.Kinematics.Velocity;
        updatedVel.Y = Velocity.Y;
        _blackboard.Kinematics.Velocity = updatedVel;
	}
}
