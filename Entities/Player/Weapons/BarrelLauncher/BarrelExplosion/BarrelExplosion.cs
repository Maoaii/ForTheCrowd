using Godot;

public partial class BarrelExplosion : Area3D
{
	public override void _Ready()
	{
		GetTree().CreateTimer(2.5f).Timeout += QueueFree;
	}
}
