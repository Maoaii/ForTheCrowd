using CarGame.Systems.Blackboards;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public partial class Player : CharacterBody3D
{
	[Export] public BlackboardComponent Blackboard;
	[Export] public HealthComponent HealthComponent;
	[Export] public HurtBox HurtBox;

	public override void _Ready()
	{
		Blackboard.Owner = this;

		Blackboard.Events.OnEnemyRunOver += () => TrickPopupManager.Instance.PopupTrick("Roadkill", 2);

		Blackboard.Events.OnEnemyRunOver += () =>
		{
			Blackboard.Camera.WantsCameraShake = true;
			Blackboard.Camera.CameraShakeTime = 1.0f;
		};

		HurtBox.AreaEntered += (area) => HealthComponent.TakeDamage(1);
	}

	public override void _Process(double delta)
	{
		SpeedLines.Instance?.SetIntensity(Blackboard.Kinematics.BoostIntensity);
	}
}
