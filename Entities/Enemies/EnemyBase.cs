using System;
using CarGame.Entities.Components;
using CarGame.Entities.Enemies.Swarm;
using Godot;

public partial class EnemyBase : CharacterBody3D, ISwarmable, IVelocityHolder
{
	public Node3D Target;
	public SwarmManager Swarm;

	public event Action<Node3D> OnFreed;

	protected void RaiseFreed()
	{
		OnFreed?.Invoke(this);
	}

	public Vector3 GetVelocity()
	{
		throw new NotImplementedException();
	}
}
