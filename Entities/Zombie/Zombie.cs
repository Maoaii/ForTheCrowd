using Godot;
using System;
using CarGame.Entities.Enemies.Swarm;
using CarGame.Utils.Debug;

namespace CarGame.Entities.Enemies;

public partial class Zombie : Area3D, ISwarmable
{
    // --- Physics Properties ---
    [ExportGroup("Movement")]
    [Export] public float SeekingForce = 5.0f;
    [Export] public float SeekingWeight = 0.5f;
    [Export] public float SeparationForce = 15.0f;
    [Export] public float SeparationWeight = 0.5f;
    [Export] public float VelocityLerpSpeed = 1.0f;
    
    [ExportGroup("Visuals")]
    [Export] private GpuParticles3D _bloodParticles;
    [Export] private Decal _bloodDecal;

    // --- Fields ---
    public Node3D Target;
    public SwarmManager Swarm;
    private Vector3 _velocity = Vector3.Zero;
    
    public event Action<Node3D> OnFreed;

    public override void _Ready()
    {
        BodyEntered += HandleCollision;
        if (Swarm != null)
        {
            Swarm.RegisterEntity(this);
        }
    }

    public override void _ExitTree()
    {
        if (Swarm != null)
        {
            Swarm.RemoveEntity(this);
        }
        OnFreed?.Invoke(this);
    }

    public new Vector3 GetPosition()
    {
        return GlobalPosition;
    }

    public new Transform3D GetGlobalTransform()
    {
        return GlobalTransform;
    }

    private void HandleCollision(Node3D body)
    {
        if (body is CarGame.Entities.Player.Player)
        {
            GpuParticles3D particles = _bloodParticles;
            _bloodParticles.Reparent(GetTree().CurrentScene);
            _bloodParticles.Emitting = true;
            _bloodParticles.Owner = GetTree().CurrentScene;

            GetTree().CreateTimer(_bloodParticles.Lifetime).Timeout += particles.QueueFree;
            
            Decal decal = _bloodDecal;
            _bloodDecal.Reparent(GetTree().CurrentScene);
            _bloodDecal.Owner = GetTree().CurrentScene;
            _bloodDecal.Visible = true;
            Tween tween = GetTree().CurrentScene.CreateTween();
            tween.TweenProperty(decal, "modulate:a", 0.0f, 3).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
            tween.TweenCallback(Callable.From(() => decal.QueueFree()));
            
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        
        Vector3 directionToTarget = Vector3.Zero;
        if (Target != null)
        {
            directionToTarget = Target.GlobalPosition - GlobalPosition;
            directionToTarget.Y = 0;
            
            if (directionToTarget != Vector3.Zero)
            {
                directionToTarget = directionToTarget.Normalized();
            }
        }

        Vector3 separationVector = Vector3.Zero;
        if (Swarm != null)
        {
            separationVector = Swarm.GetSeparationDirection(this);
        }

        Vector3 seeking = directionToTarget * SeekingForce;
        Vector3 separation = separationVector * SeparationForce;
        Vector3 final = (seeking * SeekingWeight) + (separation * SeparationWeight);

        _velocity = _velocity.Lerp(final, VelocityLerpSpeed * dt);
        GlobalPosition += _velocity * dt;

        // Draw debug vectors
        Vector3 zPos = GlobalPosition + Vector3.Up * 1.5f; 
        VectorRenderer.DrawVector(zPos, seeking * SeekingWeight, Colors.Red, 1.0f); 
        VectorRenderer.DrawVector(zPos, separation * SeparationWeight, Colors.Yellow, 1.0f); 
        VectorRenderer.DrawVector(zPos, _velocity, Colors.Green, 0.5f);
    }
}
