using Godot;
using CarGame.Entities.Enemies.Swarm;
using CarGame.Entities.Enemies;

namespace CarGame.Entities.Spawners;

public partial class ZombieSpawner : Node3D
{
    [Export] public float SpawnRate = 1.0f;
    [Export] public Node3D Target;
    
    private SwarmBehaviour _swarm;
    private double _timer;
    private PackedScene _zombieScene;

    public override void _Ready()
    {
        _swarm = new SwarmBehaviour();
        _zombieScene = GD.Load<PackedScene>("res://entities/zombie/Zombie.tscn");
    }

    public override void _Process(double delta)
    {
        _timer += delta;
        if (_timer >= SpawnRate)
        {
            _timer -= SpawnRate;
            Spawn();
        }
    }

    private void Spawn()
    {
        if (_zombieScene == null) return;
        
        Zombie zombie = _zombieScene.Instantiate<Zombie>();
        zombie.Target = Target;
        zombie.Swarm = _swarm;
        
        GetTree().CurrentScene.AddChild(zombie);
        zombie.GlobalPosition = GlobalPosition;
    }
}
