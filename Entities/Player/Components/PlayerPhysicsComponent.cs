using System;
using Godot;
using CarGame.Entities.Player;
using CarGame.Systems.Blackboards;
using CarGame.Utils.Debug;

namespace CarGame.Entities.Components;

public partial class PlayerPhysicsComponent : Node
{
    [Export] public BlackboardComponent Blackboard;
    [Export] public CharacterBody3D Body;
    [Export] public MovementConfig MovementConfig;
    [Export] public DriftConfig DriftConfig;

    public override void _PhysicsProcess(double delta)
    {
        if (Blackboard == null || Body == null) return;
        
        float dt = (float)delta;
        
        Blackboard.State.WasOnGround = Blackboard.State.IsOnGround;
        Blackboard.State.IsOnGround = Body.IsOnFloor();
        
        Blackboard.State.WantsGroundPhysics = Blackboard.State.IsOnGround;
        
        if (Blackboard.State.WantsGroundPhysics)
            ApplyGroundPhysics(Blackboard, dt);
        
        Move(dt);
    }

    private void Move(float dt)
    {
        ApplyYaw(dt);
        Vector3 facing = UpdateMovementDirection(dt);
        ApplyVelocity(dt);
        DrawDebugVectors(facing);
    }

    private void ApplyYaw(float dt)
    {
        if (!Blackboard.State.CanMove) return;

        float yawRate = Blackboard.Kinematics.Speed / MovementConfig.Wheelbase * Mathf.Tan(Mathf.DegToRad(Blackboard.Kinematics.SteerAngle));
        Body.Rotation = new Vector3(Body.Rotation.X, Body.Rotation.Y + yawRate * dt, Body.Rotation.Z);
        
        Blackboard.Visuals.VisualDirection = Vector3.Forward.Rotated(Vector3.Up, Body.GlobalRotation.Y);
    }

    /// <summary>
    /// Eases the travel direction toward the direction the car is facing. Lower grip means a slower catch-up, which is the slide.
    /// </summary>
    /// <returns>The direction the car is facing.</returns>
    private Vector3 UpdateMovementDirection(float dt)
    {
        Vector3 facing = -Body.Transform.Basis.Z;
        float catchUpRate = Mathf.Lerp(DriftConfig.HighGripRate, DriftConfig.LowGripRate, Blackboard.Kinematics.DriftFactor);
        catchUpRate *= Mathf.Lerp(1.0f, MovementConfig.BoostGripMult, Blackboard.Kinematics.BoostIntensity);

        if (Blackboard.Kinematics.MovementDirection == Vector3.Zero)
            Blackboard.Kinematics.MovementDirection = facing;

        Vector3 direction = Blackboard.Kinematics.MovementDirection.Lerp(facing, catchUpRate * dt).Normalized();

        float slipAngle = direction.AngleTo(facing);
        float maxSlipAngle = Mathf.DegToRad(DriftConfig.MaxSlipAngleDegrees);
        if (slipAngle > maxSlipAngle)
            direction = direction.Slerp(facing, 1.0f - maxSlipAngle / slipAngle);

        Blackboard.Kinematics.MovementDirection = direction;
        return facing;
    }

    private void ApplyVelocity(float dt)
    {
        float verticalSpeed = Blackboard.Kinematics.Velocity.Y;
        if (!Blackboard.State.IsOnGround)
            verticalSpeed -= MovementConfig.GravityForce * MovementConfig.FallMultiplier * dt;

        Vector3 velocity = Blackboard.Kinematics.MovementDirection * Blackboard.Kinematics.Speed;
        velocity.Y = verticalSpeed * Blackboard.Kinematics.GravityToggler;

        Body.Velocity = velocity;
        Body.MoveAndSlide();

        Blackboard.Kinematics.Velocity = new Vector3(velocity.X, Body.Velocity.Y, velocity.Z);
    }

    private void DrawDebugVectors(Vector3 facing)
    {
        Vector3 carPos = Body.GlobalPosition + Vector3.Up * 0.5f; // Lift slightly off ground
        VectorRenderer.DrawVector(carPos, facing, Colors.Red, 3.0f); // Forward Facing
        VectorRenderer.DrawVector(carPos, Blackboard.Kinematics.MovementDirection, Colors.Green, 3.0f); // Movement Intent
        VectorRenderer.DrawVector(carPos, Body.Velocity, Colors.Blue, 0.2f); // Actual Velocity
    }

    private void ApplyGroundPhysics(BlackboardComponent blackboard, float dt)
    {
        ApplyDrift(blackboard, dt);
        ApplyAcceleration(blackboard, dt);
        ApplyFriction(blackboard, dt);
        ApplySteering(blackboard, dt);
    }

    private void ApplyDrift(BlackboardComponent blackboard, float dt)
    {
        float speedFactor = Mathf.Clamp(
            (Math.Abs(blackboard.Kinematics.Speed) - DriftConfig.MinDriftSpeed) / (MovementConfig.MaxSpeedForward - DriftConfig.MinDriftSpeed), 
            0f, 
            1f
        );

        bool wantsDrift = blackboard.Input.MoveInput != Vector2.Zero && speedFactor > 0f;

        float targetDriftFactor = wantsDrift ? speedFactor : 0.0f;
        float rate = wantsDrift ? DriftConfig.DriftBuildRate : DriftConfig.DriftDecayRate;
        blackboard.Kinematics.DriftFactor = Mathf.Lerp(blackboard.Kinematics.DriftFactor, targetDriftFactor, 1.0f - Mathf.Exp(-rate * dt));
        
        if (blackboard.Kinematics.DriftFactor > 0.33f && blackboard.Kinematics.SteerAngle != 0f)
            blackboard.Events.OnDrifting?.Invoke(blackboard.Kinematics.DriftFactor);
    }

    private void ApplyAcceleration(BlackboardComponent blackboard, float dt)
    {
        if (!blackboard.State.IsOnGround)
            return;

        bool throttlingForward = blackboard.Input.MoveInput.Y > 0 || blackboard.State.IsBoosting;
        bool throttlingBackwards = blackboard.Input.MoveInput.Y < 0;

        if (throttlingForward)
        {
            float acceleration = MovementConfig.Acceleration * Mathf.Lerp(1.0f, MovementConfig.BoostAccelerationMult, blackboard.Kinematics.BoostIntensity);
            blackboard.Kinematics.Speed += acceleration * dt;
        }
        else if (throttlingBackwards)
            blackboard.Kinematics.Speed -= MovementConfig.Acceleration * dt;

        bool movingForward = blackboard.Kinematics.Speed > 0;
        if (movingForward)
        {
            float maxSpeed = MovementConfig.MaxSpeedForward * Mathf.Lerp(1.0f, MovementConfig.BoostSpeedMult, blackboard.Kinematics.BoostIntensity);
            blackboard.Kinematics.Speed = Mathf.Clamp(blackboard.Kinematics.Speed, 0, maxSpeed);
        }
        else
            blackboard.Kinematics.Speed = Mathf.Clamp(blackboard.Kinematics.Speed, -MovementConfig.MaxSpeedBackwards, 0);

        bool speedCloseZero = Math.Abs(blackboard.Kinematics.Speed) < MovementConfig.SpeedLowerThreshold;
        if (speedCloseZero)
            blackboard.Kinematics.Speed = 0;
    }

    private void ApplyFriction(BlackboardComponent blackboard, float dt)
    {
        bool throttling = blackboard.Input.MoveInput.Y != 0 || blackboard.State.IsBoosting;

        float friction = throttling ? MovementConfig.AirFriction : MovementConfig.CoastingFriction;
        blackboard.Kinematics.Speed *= Mathf.Pow(1 - friction, dt);
    }

    private void ApplySteering(BlackboardComponent blackboard, float dt)
    {
        if (!blackboard.State.IsOnGround || blackboard.Kinematics.Speed == 0)
        {
            blackboard.Kinematics.SteerAngle = 0;
            return;
        }

        float absSpeed = Mathf.Abs(blackboard.Kinematics.Speed);
        float speedNorm = absSpeed / MovementConfig.MaxSpeedForward;
        float steerLimit = MovementConfig.MaxSteerAngleDegrees * Mathf.Pow(MovementConfig.HighSpeedSteerScale, speedNorm);

        float yawCapAngle = Mathf.RadToDeg(Mathf.Atan(MovementConfig.MaxYawRate * MovementConfig.Wheelbase / absSpeed));
        steerLimit = Mathf.Min(steerLimit, yawCapAngle);

        float steerTime = MovementConfig.SteerRate * Mathf.Lerp(1f, MovementConfig.BoostSteerTimeMult, blackboard.Kinematics.BoostIntensity);
        float steerStep = MovementConfig.MaxSteerAngleDegrees / steerTime * dt;

        bool turningLeft = blackboard.Input.MoveInput.X < 0;
        bool turningRight = blackboard.Input.MoveInput.X > 0;
        if (turningLeft)
        {
            if (blackboard.Kinematics.SteerAngle < 0)
                blackboard.Kinematics.SteerAngle = 0;
            blackboard.Kinematics.SteerAngle = Mathf.MoveToward(blackboard.Kinematics.SteerAngle, steerLimit, steerStep);
        }
        else if (turningRight)
        {
            if (blackboard.Kinematics.SteerAngle > 0)
                blackboard.Kinematics.SteerAngle = 0;
            blackboard.Kinematics.SteerAngle = Mathf.MoveToward(blackboard.Kinematics.SteerAngle, -steerLimit, steerStep);
        }
        else
            blackboard.Kinematics.SteerAngle = 0;

        blackboard.Kinematics.SteerAngle = Mathf.Clamp(blackboard.Kinematics.SteerAngle, -steerLimit, steerLimit);
    }
}
