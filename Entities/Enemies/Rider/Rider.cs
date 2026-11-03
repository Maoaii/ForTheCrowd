using System;
using CarGame.Entities.Components;
using CarGame.Entities.Enemies.Swarm;
using Godot;

[Tool]
public partial class Rider : EnemyBase, ISwarmable, IVelocityHolder
{
	public event Action<Node3D> OnFreed;

	public Vector3 GetVelocity() => Velocity;
}
