using System;
using Godot;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Components;

public partial class PlayerPhysicsComponent : Node
{
    [Export] public BlackboardComponent Blackboard;
    [Export] public CharacterBody3D Body;

    public override void _PhysicsProcess(double delta)
    {
        if (Blackboard == null || Body == null) return;
        
        float dt = (float)delta;
        
        Blackboard.State.WasOnGround = Blackboard.State.IsOnGround;
        Blackboard.State.IsOnGround = Body.IsOnFloor();
        
        Blackboard.State.WantsGroundPhysics = Blackboard.State.IsOnGround;
        
        if (Blackboard.State.WantsGroundPhysics)
        {
            ApplyGroundPhysics(Blackboard, dt);
        }
        
        Move(dt);
    }

    private void Move(float dt)
    {
        if (Blackboard.State.CanMove)
        {
            float yawRate = Blackboard.Kinematics.Speed / Blackboard.MovementConfig.Wheelbase * Mathf.Tan(Mathf.DegToRad(Blackboard.Kinematics.SteerAngle));
            Body.Rotation = new Godot.Vector3(Body.Rotation.X, Body.Rotation.Y + yawRate * dt, Body.Rotation.Z);
        }

        Godot.Vector3 targetDirection = -Body.Transform.Basis.Z; // Forward in Godot is -Z
        float catchUpRate = Mathf.Lerp(
            Blackboard.MovementConfig.HighGripRate, 
            Blackboard.MovementConfig.LowGripRate, 
            Blackboard.Kinematics.DriftFactor
        );

        if (Blackboard.Kinematics.MovementDirection == Godot.Vector3.Zero)
            Blackboard.Kinematics.MovementDirection = targetDirection;
            
        Blackboard.Kinematics.MovementDirection = Blackboard.Kinematics.MovementDirection.Lerp(targetDirection, catchUpRate * dt);
        Blackboard.Kinematics.MovementDirection = Blackboard.Kinematics.MovementDirection.Normalized();

        float gravityComponent = Blackboard.Kinematics.Velocity.Y;

        Godot.Vector3 newVelocity = Blackboard.Kinematics.MovementDirection * Blackboard.Kinematics.Speed;
        
        if (!Blackboard.State.IsOnGround)
        {
            gravityComponent -= Blackboard.MovementConfig.GravityForce * Blackboard.MovementConfig.FallMultiplier * dt;
        }
        
        newVelocity.Y = gravityComponent * Blackboard.Kinematics.GravityToggler;
        
        Blackboard.Kinematics.Velocity = newVelocity;
        Body.Velocity = newVelocity;
        
        Body.MoveAndSlide();
        
        Godot.Vector3 updatedVel = Blackboard.Kinematics.Velocity;
        updatedVel.Y = Body.Velocity.Y;
        Blackboard.Kinematics.Velocity = updatedVel;
    }

    private void ApplyGroundPhysics(BlackboardComponent blackboard, float dt)
    {
        ApplyDrift(blackboard);
        ApplyAcceleration(blackboard, dt);
        ApplyFriction(blackboard, dt);
        ApplySteering(blackboard, dt);
    }

    private void ApplyDrift(BlackboardComponent blackboard)
    {
        float speedFactor = Mathf.Clamp(
            (Math.Abs(blackboard.Kinematics.Speed) - blackboard.MovementConfig.MinDriftSpeed) / (blackboard.MovementConfig.MaxSpeedForward - blackboard.MovementConfig.MinDriftSpeed), 
            0f, 
            1f
        );

        bool wantsDrift = blackboard.Input.MoveInput != Vector2.Zero && speedFactor > 0f;

        float targetDriftFactor = wantsDrift ? speedFactor : 0.0f;
        float rate = wantsDrift ? blackboard.MovementConfig.DriftBuildRate : blackboard.MovementConfig.DriftDecayRate;
        blackboard.Kinematics.DriftFactor = Mathf.Lerp(blackboard.Kinematics.DriftFactor, targetDriftFactor, rate);
        
        if (blackboard.Kinematics.DriftFactor > 0.33f && blackboard.Kinematics.SteerAngle != 0f)
            blackboard.Events.OnDrifting?.Invoke(blackboard.Kinematics.DriftFactor);
    }

    private void ApplyAcceleration(BlackboardComponent blackboard, float dt)
    {
        if (!blackboard.State.IsOnGround)
            return;

        if (blackboard.Input.MoveInput.Y > 0) // forward
            blackboard.Kinematics.Speed += blackboard.MovementConfig.Acceleration * dt;
        else if (blackboard.Input.MoveInput.Y < 0) // backward
            blackboard.Kinematics.Speed -= blackboard.MovementConfig.Acceleration * dt;

        if (blackboard.Kinematics.Speed > 0)
            blackboard.Kinematics.Speed = Mathf.Clamp(blackboard.Kinematics.Speed, 0, blackboard.MovementConfig.MaxSpeedForward);
        else
            blackboard.Kinematics.Speed = Mathf.Clamp(blackboard.Kinematics.Speed, -blackboard.MovementConfig.MaxSpeedBackwards, 0);

        if (Math.Abs(blackboard.Kinematics.Speed) < blackboard.MovementConfig.SpeedLowerThreshold)
            blackboard.Kinematics.Speed = 0;
    }

    private void ApplyFriction(BlackboardComponent blackboard, float dt)
    {
        if (blackboard.State.IsOnGround && blackboard.Input.MoveInput.Y == 0)
            blackboard.Kinematics.Speed *= Mathf.Pow(1 - blackboard.MovementConfig.CoastingFriction, dt);
        else
            blackboard.Kinematics.Speed *= Mathf.Pow(1 - blackboard.MovementConfig.AirFriction, dt);
    }

    private void ApplySteering(BlackboardComponent blackboard, float dt)
    {
        if (!blackboard.State.IsOnGround || blackboard.Kinematics.Speed == 0)
        {
            blackboard.Kinematics.SteerAngle = 0;
            return;
        }

        if (blackboard.Input.MoveInput.X < 0) // Left
        {
            if (blackboard.Kinematics.SteerAngle < 0)
                blackboard.Kinematics.SteerAngle = 0;
            blackboard.Kinematics.SteerAngle = Mathf.MoveToward(blackboard.Kinematics.SteerAngle, blackboard.MovementConfig.MaxSteerAngleDegrees, blackboard.MovementConfig.MaxSteerAngleDegrees / blackboard.MovementConfig.SteerRate * dt);
        }
        else if (blackboard.Input.MoveInput.X > 0) // Right
        {
            if (blackboard.Kinematics.SteerAngle > 0)
                blackboard.Kinematics.SteerAngle = 0;
            blackboard.Kinematics.SteerAngle = Mathf.MoveToward(blackboard.Kinematics.SteerAngle, -blackboard.MovementConfig.MaxSteerAngleDegrees, blackboard.MovementConfig.MaxSteerAngleDegrees / blackboard.MovementConfig.SteerRate * dt);
        }
        else
            blackboard.Kinematics.SteerAngle = 0;
    }
}
