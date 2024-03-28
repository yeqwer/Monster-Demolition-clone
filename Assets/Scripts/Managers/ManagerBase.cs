using System;
using UnityEngine;

public abstract class ManagerBase<TState> : MonoBehaviour
    where TState : Enum
{
    private TState _currentState;

    public TState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            if (_currentState.Equals(value))
                return;

            _currentState = value;
            OnStateChanged?.Invoke(value);
            OnStateChangedCallback(value);
        }
    }

    public event Action<TState> OnStateChanged;

    protected virtual void Start()
    {
        CurrentState = default;
        OnStateChanged?.Invoke(CurrentState);
        OnStateChangedCallback(CurrentState);
    }

    public virtual void SwitchState(TState state)
    {
        CurrentState = state;
    }

    protected virtual void OnStateChangedCallback(TState newState) { }
}
