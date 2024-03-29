using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour 
{
    protected FSM m_Owner;
    [SerializeField] protected MyAnimation m_Animation;

    public void Initialize(FSM owner)
    {
        m_Owner = owner;
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void OnUpdate();
}
