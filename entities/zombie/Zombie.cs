using Godot;
using System;
using CarGame.Entities.Enemies.Swarm;

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

    public new Godot.Vector3 GetPosition()
    {
        return GlobalPosition;
    }

    private void HandleCollision(Node3D body)
    {
        if (body is CarGame.Entities.Player.Player)
        {
            // Spawn blood particle (assuming child node)
            var particles = GetNodeOrNull<GpuParticles3D>("BloodParticles");
            if (particles != null)
            {
                particles.Reparent(GetTree().CurrentScene);
                particles.Emitting = true;
                
                // Cleanup particle after it finishes (e.g., SceneTreeTimer)
                GetTree().CreateTimer(particles.Lifetime).Timeout += () => particles.QueueFree();
            }
            
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
            separationVector = Swarm.GetSeparationDirection(this, Time.GetTicksMsec() / 1000.0);
        }

        Vector3 seeking = directionToTarget * SeekingForce;
        Vector3 separation = separationVector * SeparationForce;
        Vector3 final = (seeking * SeekingWeight) + (separation * SeparationWeight);

        _velocity = _velocity.Lerp(final, VelocityLerpSpeed * dt);
        GlobalPosition += _velocity * dt;
    }
}
