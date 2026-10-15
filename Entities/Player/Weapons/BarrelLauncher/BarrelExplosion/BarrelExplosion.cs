using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class BarrelExplosion : Area3D
{
	[Export] public float Lifetime = 2.5f;

	public override void _Ready()
	{
		GetTree().CreateTimer(Lifetime).Timeout += QueueFree;
	}
}
