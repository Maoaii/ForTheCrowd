using System;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Systems;

public abstract partial class State : Node
{
    protected BlackboardComponent _blackboard;
    protected StateMachine _stateMachine;
    
    public virtual Enum StateName => null;

    public void RegisterBlackboard(StateMachine stateMachine, BlackboardComponent blackboard)
    {
        _stateMachine = stateMachine;
        _blackboard = blackboard;
    }

    public virtual void Enter(double delta = 0) 
    {
    }

    public virtual void Update(double delta)
    {
    }

    public virtual void Exit(double delta = 0)
    {
    }
}