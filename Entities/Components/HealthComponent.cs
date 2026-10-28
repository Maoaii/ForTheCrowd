using System;
using Godot;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Signal]
	public delegate void HealthDepletedEventHandler();

	[Signal]
	public delegate void HealthChangedEventHandler(int oldHealth, int newHealth);

	[Export] public int MaxHealth = 1;

	private int _currentHealth;
	public override void _Ready()
	{
		_currentHealth = MaxHealth;
	}

	public void TakeDamage(int damage)
	{
		int previousHealth = _currentHealth;
		_currentHealth = Mathf.Max(_currentHealth - damage, 0);

		EmitSignal(SignalName.HealthChanged, previousHealth, _currentHealth);
		if (_currentHealth <= 0)
			EmitSignal(SignalName.HealthDepleted);
	}

	public void Heal(int healAmount)
	{
		int previousHealth = _currentHealth;
		_currentHealth = Mathf.Min(_currentHealth + healAmount, 0);

		EmitSignal(SignalName.HealthChanged, previousHealth, _currentHealth);
	}
}
