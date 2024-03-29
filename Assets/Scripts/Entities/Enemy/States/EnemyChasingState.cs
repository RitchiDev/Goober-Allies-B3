using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingState : BaseState
{
    [Header("Components")]
    [SerializeField] protected Enemy m_Enemy;

    [Header("Movement")]
    [SerializeField] private float m_MovementSpeed = 10f;

    [Header("Path")]
    [SerializeField] private PathWalker m_PathWalker;
    [SerializeField] private LayerMask m_Obstacles;
    [SerializeField] private float m_StartBattleDistance = 1f;

    private void Awake()
    {
        m_PathWalker.SetMovementSpeed(m_MovementSpeed);
    }

    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();

        Vector2Int startPosition = MyExtensions.GetPositionToVector2Int(transform.position);

        Vector3 targetPosition = m_Enemy.GetTargetTransform().position;
        Vector2Int targetPositionToInt = new Vector2Int((int)targetPosition.x, (int)targetPosition.z);

        m_PathWalker.WalkTowardsTarget(startPosition, targetPositionToInt, OnEnemyMoved);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();
    }

    private void OnEnemyMoved()
    {
        if (m_Enemy.GetTargetPlayer() != null)
        {
            CheckForBattle();
            return;
        }

        m_Owner.SwitchState(typeof(EnemyPatrolState));
    }

    private void CheckForBattle()
    {
        bool playerIsOutsideBattleRange = Vector3.Distance(transform.position, m_Enemy.GetTargetTransform().position) > m_StartBattleDistance;

        if (m_Enemy.GetTargetPlayer().IsInBattle() || playerIsOutsideBattleRange)
        {
            m_Owner.SwitchState(typeof(EnemyPatrolState));

            return;
        }

        m_Enemy.StartBattle();
        m_Enemy.GetTargetPlayer().StartBattle(m_Enemy);

        m_Owner.SwitchState(typeof(EnemyIdleState));
    }
}
