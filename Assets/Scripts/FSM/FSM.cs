using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private Dictionary<System.Type, BaseState> m_StateDictionary = new Dictionary<System.Type, BaseState>();
    private BaseState m_CurrentState;

    public FSM(System.Type startState, params BaseState[] states)
    {
        for (int i = 0; i < states.Length; i++)
        {
            BaseState state = states[i];
            state.Initialize(this);
            m_StateDictionary.Add(state.GetType(), state);
        }

        SwitchState(startState);
    }

    public void OnUpdate()
    {
        m_CurrentState?.OnUpdate();
    }

    public void SwitchState(System.Type newStateType)
    {
        if (!m_StateDictionary.ContainsKey(newStateType))
        {
            Debug.LogWarning("Trying to switch to a state that hasn't been included!");
            return;
        }

        m_CurrentState?.OnExit();

        m_CurrentState = m_StateDictionary[newStateType];

        m_CurrentState?.OnEnter();
    }
}
