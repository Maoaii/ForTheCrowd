using Godot;
using CarGame.Systems.Blackboards;
using CarGame.Systems.UI;

namespace CarGame.Entities.Player;

public partial class Player : CharacterBody3D
{
    [Export] public BlackboardComponent Blackboard;

    public override void _Ready()
    {
        Blackboard.Owner = this;

        Blackboard.Events.OnZombieRunOver += () => TrickPopupManager.Instance.PopupTrick("Roadkill", 2);
    }
}

