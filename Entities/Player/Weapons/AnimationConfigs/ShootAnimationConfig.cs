using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class ShootAnimationConfig : Resource
{
	[Export] public float LookSmoothing = 15.0f;
}
