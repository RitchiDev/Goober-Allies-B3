using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : BaseState
{
    [SerializeField] private int m_WhenToPatrolAmount = 5;
    private int m_WhenToPatrolCounter;

    private void OnDisable()
    {
        GameEventManager.RemoveListener(GameEventType.OnPlayerMoved, CheckForPatrolState);
    }

    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();

        m_WhenToPatrolCounter = 0;

        GameEventManager.AddListener(GameEventType.OnPlayerMoved, CheckForPatrolState);
    }

    public override void OnExit()
    {
        m_WhenToPatrolCounter = 0;

        GameEventManager.RemoveListener(GameEventType.OnPlayerMoved, CheckForPatrolState);
    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();
    }

    private void CheckForPatrolState()
    {
        m_WhenToPatrolCounter++;

        if (m_WhenToPatrolCounter < m_WhenToPatrolAmount)
        {
            return;
        }

        m_Owner.SwitchState(typeof(EnemyPatrolState));
    }
}
