using CarGame.Systems.Blackboards;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public partial class Player : CharacterBody3D
{
	[Export] public BlackboardComponent Blackboard;
	[Export] public HealthComponent HealthComponent;
	[Export] public HurtBox HurtBox;

	private Vector3 _spawnPosition;

	public override void _Ready()
	{
		_spawnPosition = GlobalPosition;
		Blackboard.Owner = this;

		Blackboard.Events.OnEnemyRunOver += () => TrickPopupManager.Instance.PopupTrick("Roadkill", 2);

		Blackboard.Events.OnEnemyRunOver += () =>
		{
			Blackboard.Camera.WantsCameraShake = true;
			Blackboard.Camera.CameraShakeTime = 1.0f;
		};

		HurtBox.OnHit += damageInfo => HealthComponent.TakeDamage(damageInfo.Amount);
		HealthComponent.HealthDepleted += ResetPlayer;
	}

	public override void _Process(double delta)
	{
		SpeedLines.Instance?.SetIntensity(Blackboard.Kinematics.BoostIntensity);
	}

	private void ResetPlayer()
	{
		HealthComponent.Reset();

		GlobalPosition = _spawnPosition;
	}
}
