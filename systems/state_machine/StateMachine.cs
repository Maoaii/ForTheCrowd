
using System;
using CarGame.Systems.Blackboards;
using System.Collections.Generic;
using Godot;

namespace CarGame.Systems;

public partial class StateMachine : Node
{
    private Dictionary<Enum, State> _states = new();
    private State _previousState;
    private State _currentState;
    
    [Export] public BlackboardComponent Blackboard;
    [Export] public NodePath InitialStatePath;

    public Enum CurrentStateName => _currentState?.StateName;

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is State state)
            {
                if (state.StateName != null)
                {
                    _states[state.StateName] = state;
                }
                state.RegisterBlackboard(this, Blackboard);
            }
        }
        
        if (InitialStatePath != null)
        {
            var initialState = GetNodeOrNull<State>(InitialStatePath);
            if (initialState != null)
            {
                _currentState = initialState;
                _previousState = _currentState;
                _currentState.Enter(0);
            }
        }
    }

    public virtual void TransitionState(Enum toStateName, double delta = 0)
    {
        if (!_states.TryGetValue(toStateName, out State toState)) return;

        _currentState?.Exit(delta);
        _previousState = _currentState;
        _currentState = toState;
        _currentState.Enter(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState?.Update(delta);
    }
}