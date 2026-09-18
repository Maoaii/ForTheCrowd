using CarGame.Systems.Blackboards;
using Godot;
using System;

[GlobalClass]
public partial class BoostComponent : Node
{
    [Export] public float BoostForce;
    
    [ExportGroup("References")]
    [Export] public BlackboardComponent Blackboard;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Blackboard.Kinematics.BoostForce = Blackboard.Input.WantsBoost ? BoostForce : 1.0f;
    }
}
