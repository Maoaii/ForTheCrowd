
using System;
using CarGame.Systems.Blackboards;
using System.Collections.Generic;
using Godot;

namespace CarGame.Systems;

public abstract class StateMachine
{
    private List<State> _states = [];
    private State _previousState;
    private State _currentState;
    private Blackboard _blackboard;

    public Enum CurrentStateName => _currentState?.StateName;
    public StateMachine(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public void RegisterState(State state)
    {
        if (state == null) return;
        if (_states.Contains(state)) return;

        state.RegisterBlackboard(this, _blackboard);
        _states.Add(state);
    }

    public void SetInitialState(State state, double delta = 0)
    {
        if (state == null) return;
        if (!_states.Contains(state)) return;

        _currentState = state;
        _previousState = _currentState;
        _currentState.Enter(delta);
    }

    public virtual void TransitionState(Enum toStateName, double delta = 0)
    {
        State toState = _states.Find((st) => st.StateName.Equals(toStateName));
        if (toState == null) return;

        _currentState?.Exit(delta);
        _previousState = _currentState;
        _currentState = toState;
        _currentState.Enter(delta);
    }

    public virtual void Update(double delta)
    {
        _currentState?.Update(delta);
    }
}