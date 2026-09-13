using System;
using Godot;
using CarGame.Entities.Player;
using CarGame.Entities.Components;

namespace CarGame.Systems.Blackboards;

public class BlackboardEvents
{
    public Action<float> OnLanded;
    public Action<float> OnDrifting;
    public Action OnZombieRunOver;
}

public class BlackboardKinematics
{
    public float DriftFactor = 0.0f; // 0-1
    public Vector3 MovementDirection = Vector3.Zero;
    public Vector3 Velocity = Vector3.Zero;
    public float SteerAngle = 0.0f;
    public float Speed = 0.0f;
    public float GravityToggler = 1.0f;
}

public class BlackboardState
{
    public bool WasOnGround = false;
    public bool IsOnGround = false;
    public bool WantsGroundPhysics = false;
    public bool CanMove = true;
}

public class BlackboardInput
{
    public Vector2 MoveInput = Vector2.Zero;
    public float LastJumpInputTime = -1.0f;
    public bool WantsJump = false;
    public bool IsHoldingJump = false;
    public bool ReleasedJump = false;
}

public class BlackboardCamera
{
    public bool WantsFOVKick = false;
    public float FOVKick;
    public float FOVKickUpTime;
    public float FOVKickDownTime;
    public void SetFOVKick(float fovKick, float fovKickUpTime, float fovKickDownTime)
    {
        WantsFOVKick = true;
        FOVKick = fovKick;
        FOVKickUpTime = fovKickUpTime;
        FOVKickDownTime = fovKickDownTime;
    }
}

public class BlackboardTricks
{
    public bool WantsShoveIt = false;
    public bool WantsKickflip = false;
    public bool WantsBackManual = false;
    public bool WantsStopBackManual = false;
}

public partial class BlackboardComponent : Node
{
    // Core references
    [Export] public new CharacterBody3D Owner;
    [Export] public PlayerAnimatorComponent Animator;
    
    [Export] public MovementConfig MovementConfig;
    [Export] public PlayerAnimationConfig AnimationConfig;

    // Segmented states
    public BlackboardEvents Events = new();
    public BlackboardKinematics Kinematics = new();
    public BlackboardState State = new();
    public BlackboardInput Input = new();
    public BlackboardCamera Camera = new();
    public BlackboardTricks Tricks = new();
}