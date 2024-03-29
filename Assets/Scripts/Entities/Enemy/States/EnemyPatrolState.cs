using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : BaseState
{
    [Header("Components")]
    [SerializeField] protected Enemy m_Enemy;

    [Header("Patrol")]
    [SerializeField] private float m_ViewDistance = 15f;

    [Header("Path")]
    [SerializeField] private PathWalker m_PathWalker;
    [SerializeField] private LayerMask m_Obstacles;

    private void OnDisable()
    {
        GameEventManager.RemoveListener(GameEventType.OnPlayerMoved, CheckForPlayer);
    }

    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();

        GameEventManager.AddListener(GameEventType.OnPlayerMoved, CheckForPlayer);
    }

    public override void OnExit()
    {
        GameEventManager.RemoveListener(GameEventType.OnPlayerMoved, CheckForPlayer);
    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();
    }

    private void CheckForPlayer()
    {
        if (m_Enemy.GetTargetPlayer() == null)
        {
            return;
        }

        if (m_Enemy.GetTargetPlayer().IsInBattle())
        {
            return;
        }

        if (Vector3.Distance(transform.position, m_Enemy.GetTargetTransform().position) < m_ViewDistance) // Player is in range
        {
            m_Owner.SwitchState(typeof(EnemyChasingState));
        }
    }
}
