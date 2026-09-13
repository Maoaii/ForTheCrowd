using Godot;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Player;

public partial class Player : CharacterBody3D
{
    [Export] public BlackboardComponent Blackboard;

    public override void _Ready()
    {
        if (Blackboard != null)
            Blackboard.Owner = this;
    }
}

