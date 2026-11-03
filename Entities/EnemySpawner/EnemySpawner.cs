using CarGame.Entities.Enemies;
using CarGame.Entities.Enemies.Swarm;
using Godot;

namespace CarGame.Entities.Spawners;

public partial class EnemySpawner : Node3D
{
	[Export] public float SpawnRate = 1.0f;
	[Export] public Node3D Target;

	[Export] public SwarmManager Swarm;
	[Export] public PackedScene EnemyScene;

	private double _timer;
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
		EnemyBase walker = EnemyScene.Instantiate<EnemyBase>();
		walker.Target = Target;
		walker.Swarm = Swarm;

		GetTree().CurrentScene.AddChild(walker);
		walker.GlobalPosition = GlobalPosition;
	}
}
