using Dungeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private FSM m_FSM;

    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    [Header("Attack")]
    private bool m_AttackIsOnCooldown;
    private IEnumerator m_AttackCooldownCoroutine;
    [SerializeField] private List<EnemyAttackState> m_AttackStates = new List<EnemyAttackState>();

    [Header("Target")]
    private Player m_TargetPlayer;
    [SerializeField] private float m_FindNearestPlayerTime = 3f;
    private float m_FindNearestPlayerTimer;

    [Header("Battle")]
    private bool m_IsInbattle;
    private IDamageable m_PlayerDamageable;

    private void Start()
    {
        EnemyManager.Instance.AddEnemy(this);

        BaseState[] statesOnGameObject = GetComponents<BaseState>();
        System.Type startState = typeof(EnemyIdleState);
        m_FSM = new FSM(startState, statesOnGameObject);

        //m_AttackStates = GetComponents<EnemyAttackState>().ToList();
        //m_AttackStates.AddRange(m_AttackStates);
        //Debug.Log(m_AttackStates);

        m_TargetPlayer = PlayerManager.Instance.GetClosestPlayer(transform.position);

        if (m_TargetPlayer)
        {
            m_PlayerDamageable = m_TargetPlayer.GetComponent<IDamageable>();
        }

        m_FindNearestPlayerTimer = 0;
    }

    private void OnEnable()
    {
        GameEventManager.AddListener(GameEventType.OnGameStarted, SetUp);
    }

    private void OnDisable()
    {
        EndBattle();

        GameEventManager.RemoveListener(GameEventType.OnGameStarted, SetUp);

        GameEventManager.RemoveListener(GameEventType.OnPlayerAttackEnded, AttackPlayer);
    }

    private void Update()
    {
        m_FSM.OnUpdate();

        FindNearestPlayerTimer();

        CheckForFlip();
    }

    private void SetUp()
    {
        transform.position = DungeonGenerator.Instance.GetRandomPositionInLastRoom();
    }

    public void StartBattle()
    {
        GameEventManager.AddListener(GameEventType.OnPlayerAttackEnded, AttackPlayer);

        m_IsInbattle = true;
    }

    public void EndBattle()
    {
        m_IsInbattle = false;

        m_FSM.SwitchState(typeof(EnemyIdleState));
    }

    public void AttackPlayer()
    {
        if (!m_IsInbattle)
        {
            return;
        }

        int max = Mathf.Clamp(m_AttackStates.Count - 1, 0, m_AttackStates.Count - 1);
        int rng = Random.Range(0, max);
        EnemyAttackState randomEnemyAttackState = m_AttackStates[rng];

        m_FSM.SwitchState(randomEnemyAttackState.GetType().UnderlyingSystemType);
    }

    public IDamageable GetDamageablePlayer()
    {
        return m_PlayerDamageable;
    }

    private void FindNearestPlayerTimer()
    {
        if (PlayerManager.Instance.GetPlayerCount() <= 1 & m_TargetPlayer != null)
        {
            return;
        }

        m_FindNearestPlayerTimer += Time.deltaTime;

        if (m_FindNearestPlayerTimer > m_FindNearestPlayerTime)
        {
            m_TargetPlayer = PlayerManager.Instance.GetClosestPlayer(transform.position);

            if (m_TargetPlayer)
            {
                m_PlayerDamageable = m_TargetPlayer.GetComponent<IDamageable>();
            }

            m_FindNearestPlayerTimer = 0;
        }
    }

    public void Kill()
    {
        EndBattle();

        m_TargetPlayer.WinBattle();

        gameObject.SetActive(false);
    }

    public void OnAggro()
    {
        m_FSM.SwitchState(m_AttackStates.GetType());
    }

    public bool AttackIsOnCooldown()
    {
        return m_AttackIsOnCooldown;
    }

    public void StartAttackCooldown(float duration)
    {
        if (m_AttackIsOnCooldown)
        {
            return;
        }

        if (m_AttackCooldownCoroutine != null)
        {
            StopCoroutine(m_AttackCooldownCoroutine);
            m_AttackCooldownCoroutine = null;
        }

        m_AttackCooldownCoroutine = AttackCooldownTimer(duration);
        StartCoroutine(m_AttackCooldownCoroutine);
    }

    private IEnumerator AttackCooldownTimer(float duration)
    {
        m_AttackIsOnCooldown = true;

        yield return new WaitForSeconds(duration);

        m_AttackIsOnCooldown = false;
    }

    public Player GetTargetPlayer()
    {
        return m_TargetPlayer;
    }

    public Transform GetTargetTransform()
    {
        if (m_TargetPlayer == null)
        {
            return null;
        }

        return m_TargetPlayer.transform;
    }

    private void CheckForFlip()
    {
        if (m_TargetPlayer == null)
        {
            return;
        }

        if (m_TargetPlayer.transform.position.x > transform.position.x)
        {
            m_SpriteRenderer.flipX = false;
        }
        else if (m_TargetPlayer.transform.position.x < transform.position.x)
        {
            m_SpriteRenderer.flipX = true;
        }
    }

    public bool IsInBattle()
    {
        return m_IsInbattle;
    }
}
