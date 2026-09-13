using System;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Systems;

public abstract class State
{
    protected Blackboard _blackboard;
    protected StateMachine _stateMachine;
    public Enum StateName { get; private set; }

    public State(Enum stateName)
    {
        StateName = stateName;
    }

    public void RegisterBlackboard(StateMachine stateMachine, Blackboard blackboard)
    {
        _stateMachine = stateMachine;
        _blackboard = blackboard;
    }

    public virtual void Enter(double delta = 0) 
    {
        // GD.Print($"Entering state {StateName}");
    }

    public virtual void Update(double delta)
    {
        
    }

    public virtual void Exit(double delta = 0)
    {
        // GD.Print($"Exiting state {StateName}");
    }
}