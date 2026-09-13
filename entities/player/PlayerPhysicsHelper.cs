using System;
using Godot;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Player;

public static class PlayerPhysicsHelper
{
    public static void ApplyGroundPhysics(Blackboard blackboard, float dt)
    {
        ApplyDrift(blackboard);
        ApplyAcceleration(blackboard, dt);
        ApplyFriction(blackboard, dt);
        ApplySteering(blackboard, dt);
    }

    private static void ApplyDrift(Blackboard blackboard)
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

    private static void ApplyAcceleration(Blackboard blackboard, float dt)
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

    private static void ApplyFriction(Blackboard blackboard, float dt)
    {
        if (blackboard.State.IsOnGround && blackboard.Input.MoveInput.Y == 0)
            blackboard.Kinematics.Speed *= Mathf.Pow(1 - blackboard.MovementConfig.CoastingFriction, dt);
        else
            blackboard.Kinematics.Speed *= Mathf.Pow(1 - blackboard.MovementConfig.AirFriction, dt);
    }

    private static void ApplySteering(Blackboard blackboard, float dt)
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
