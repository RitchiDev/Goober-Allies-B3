using Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PlayerHealth
{
    private static float m_Health;
    public static float Health => m_Health;

    public static void SetStaticHealth(float health)
    {
        m_Health = health;
    }
}

public class Player : MonoBehaviour, IPowerUpable
{
    private FSM m_FSM;

    [SerializeField] private GameObject m_PlayerHolder;
    [SerializeField] private Transform m_CrosshairParent;
    private bool m_PlayerIsAlive = false;
    private static bool m_EnteredDungeon = false;

    [Header("Components")]
    [SerializeField] private Health m_Health;
    private IDamageable m_Damageable;

    [Header("Disconnected")]
    [SerializeField] private UnityEvent m_OnDeviceLost;

    [Header("Power-ups")]
    private List<PowerUpData> m_PowerUps = new List<PowerUpData>();

    [Header("Attack")]
    [SerializeField] private PlayerAttackState m_AttackState;
    [SerializeField] private List<Button> m_AttackButtons = new List<Button>();
    private bool m_AttackIsOnCooldown;
    private IEnumerator m_AttackCooldownCoroutine;

    [Header("Run")]
    [SerializeField] private int m_DamageOnRun = 5;
    [SerializeField] private int m_RunStrength = 3;

    [Header("Defend")]
    private bool m_DefendIsOnCooldown;

    [Header("Battle")]
    [SerializeField] private GameObject m_BattleUIContainer;
    private Enemy m_EnemyInBattle;
    private IDamageable m_DamageableEnemy;
    private Transform m_DamageableEnemyTransform;
    private bool m_IsInbattle;

    private void Awake()
    {
        m_Damageable = GetComponent<IDamageable>();
    }

    private void Start()
    {
        BaseState[] statesOnGameObject = GetComponents<BaseState>();
        System.Type startState = typeof(PlayerIdleState);
        m_FSM = new FSM(startState, statesOnGameObject);

        PlayerManager.Instance.AddPlayer(this);

        SetPlayerIsAlive(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        m_Health.AddListenerToReceiveDamage(UpdateStaticHealth);
        m_Health.AddListenerToReceiveHealing(UpdateStaticHealth);

        GameEventManager.AddListener(GameEventType.OnGameStarted, SetToStartPosition);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        m_Health.RemoveListenerFromReceiveDamage(UpdateStaticHealth);
        m_Health.RemoveListenerFromReceiveHealing(UpdateStaticHealth);

        GameEventManager.RemoveListener(GameEventType.OnGameStarted, SetToStartPosition);

        GameEventManager.RemoveListener(GameEventType.OnEnemyAttackEnded, ButtonsSetInteractable);
    }

    private void Update()
    {
        m_FSM.OnUpdate();
    }

    public void OnDeviceLost(PlayerInput playerInput)
    {
        m_OnDeviceLost?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            // Dungeon scene
            case 2:
                m_EnteredDungeon = true;
                m_Health.SetHealth(PlayerHealth.Health);

                break;

            default:
                m_EnteredDungeon = false;
                break;
        }
    }

    private void SetToStartPosition()
    {
        Vector2Int startTilePosition = DungeonGenerator.Instance.GetRandomTilePositionInFirstRoom();
        Vector3 startPosition = new Vector3(startTilePosition.x, transform.position.y, startTilePosition.y);

        transform.position = startPosition;
    }

    public void Revive()
    {
        SetPlayerIsAlive(true);

        PlayerManager.Instance.AddPlayer(this);

        if (m_CrosshairParent != null)
        {
            m_CrosshairParent.gameObject.SetActive(true);
        }

        m_PowerUps = new List<PowerUpData>();
    }

    public void Kill()
    {
        SetPlayerIsAlive(false);

        PlayerManager.Instance.RemovePlayer(this);

        if (m_CrosshairParent != null)
        {
            m_CrosshairParent.gameObject.SetActive(false);
        }

        m_PowerUps.Clear();
    }

    public void StartBattle(Enemy enemy)
    {
        ButtonsSetInteractable();
        GameEventManager.AddListener(GameEventType.OnEnemyAttackEnded, ButtonsSetInteractable);

        m_EnemyInBattle = enemy;
        m_DamageableEnemy = enemy.GetComponent<IDamageable>();
        m_DamageableEnemyTransform = enemy.transform;

        m_BattleUIContainer.SetActive(true);
        m_IsInbattle = true;
    }

    public void WinBattle()
    {
        ButtonsSetNonInteractive();
        GameEventManager.RemoveListener(GameEventType.OnEnemyAttackEnded, ButtonsSetInteractable);

        m_EnemyInBattle = null;
        m_DamageableEnemy = null;
        m_DamageableEnemyTransform = null;

        m_BattleUIContainer.SetActive(false);
        m_IsInbattle = false;
    }

    public void RunFromBattle()
    {
        m_Health.TakeDamage(m_DamageOnRun);
        ButtonsSetNonInteractive();
        GameEventManager.RemoveListener(GameEventType.OnEnemyAttackEnded, ButtonsSetInteractable);

        m_BattleUIContainer.SetActive(false);

        m_EnemyInBattle.EndBattle();
        m_DamageableEnemy = null;
        m_DamageableEnemyTransform = null;

        m_IsInbattle = false;

        m_FSM.SwitchState(typeof(PlayerMovingState));
    }

    /// <summary>
    /// Used for the attack button
    /// </summary>
    public void AttackEnemy()
    {
        ButtonsSetNonInteractive();

        m_FSM.SwitchState(m_AttackState.GetType());
    }

    public void ButtonsSetInteractable()
    {
        for (int i = 0; i < m_AttackButtons.Count; i++)
        {
            m_AttackButtons[i].interactable = true;
        }
    }

    public void ButtonsSetNonInteractive()
    {
        for (int i = 0; i < m_AttackButtons.Count; i++)
        {
            m_AttackButtons[i].interactable = false;
        }
    }

    public IDamageable GetDamageableEnemy()
    {
        return m_DamageableEnemy;
    }

    public Vector3 GetRunDirection()
    {
        Vector3 direction = transform.forward;
        Vector3 multy = new Vector3(m_RunStrength, m_RunStrength, m_RunStrength);

        return Vector3.Scale(direction, multy);
    }

    public bool IsInBattle()
    {
        return m_IsInbattle;
    }

    public void UpdatePowerUps()
    {
        for (int i = 0; i < m_PowerUps.Count; i++)
        {
            PowerUpData powerUp = m_PowerUps[i];
            powerUp?.OnUpdate();
        }
    }

    public void GainPowerUp(PowerUpData powerUp)
    {
        if (m_PowerUps.Contains(powerUp))
        {
            Debug.LogWarning($"Already contains this power-up: {powerUp}", gameObject);
            return;
        }

        powerUp.Initialize(this, this);
        m_PowerUps.Add(powerUp);
    }

    public void RemovePowerUp(PowerUpData powerUp)
    {
        if (!m_PowerUps.Contains(powerUp))
        {
            Debug.LogWarning($"Player does not have this power-up: {powerUp}", gameObject);
            return;
        }

        m_PowerUps.Remove(powerUp);
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

    public bool DefendIsOnCooldown()
    {
        return m_DefendIsOnCooldown;
    }

    public void SetDefendIsOnCooldown(bool value)
    {
        m_DefendIsOnCooldown = value;
    }

    private void UpdateStaticHealth()
    {
        PlayerHealth.SetStaticHealth(m_Health.GetCurrentHealth());
    }

    public void SetPlayerIsAlive(bool value)
    {
        m_PlayerIsAlive = value;
    }
}
