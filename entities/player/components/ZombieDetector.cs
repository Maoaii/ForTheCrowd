using Godot;
using CarGame.Systems.Blackboards;
using CarGame.Entities.Enemies;

namespace CarGame.Entities.Components;

[GlobalClass]
public partial class ZombieDetector : Area3D
{
    [Export] public BlackboardComponent Blackboard;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (area is Zombie zombie)
        {
            // Only responsible for updating the player's internal state
            Blackboard?.Events.OnZombieRunOver?.Invoke();
        }
    }
}
