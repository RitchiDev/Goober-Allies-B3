using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttackState : BaseState, IDamageDealable
{
    [Header("Components")]
    [SerializeField] protected Player m_Player;
    [SerializeField] private AudioSource m_AttackAudioSource;
    protected Rigidbody m_Rigidbody;

    [Header("Attack")]
    [SerializeField] protected float m_AttackDamage = 10f;
    [SerializeField] protected float m_DamageMargin = 1f;
    [SerializeField] protected float m_AttackDuration = 0.2f;

    [SerializeField] private TextPopUp m_TextPopUpPrefab;
    [SerializeField] private Color m_TextPopUpColor = Color.white;
    [SerializeField] private ParticleSystem m_AttackEffect;
    [SerializeField] private GameObject m_AttackBoostEffect;
    private bool m_HasAttackBoost = false;
    private IEnumerator m_AttackCoroutine;


    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();

        StartAttack(m_AttackDuration);
    }

    public override void OnExit()
    {
        m_AttackEffect.Stop();

        m_AttackAudioSource.Stop();

        GameEventManager.RaiseEvent(GameEventType.OnPlayerAttackEnded);
    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();
    }

    protected virtual void Attack()
    {
        m_AttackEffect.Play();

        m_AttackAudioSource.Play();

        float damage = m_AttackDamage + Mathf.RoundToInt(Random.Range(-m_DamageMargin, m_DamageMargin));

        TextPopUp textPopUp = Instantiate(m_TextPopUpPrefab, transform.position, Quaternion.identity);
        textPopUp.SetText($"Enemy took: {damage} damage!");
        textPopUp.SetColor(m_TextPopUpColor);

        m_Player.GetDamageableEnemy().TakeDamage(damage);
    }

    public void StartAttack(float duration)
    {
        if (m_AttackCoroutine != null)
        {
            StopCoroutine(m_AttackCoroutine);
            m_AttackCoroutine = null;
        }

        m_AttackCoroutine = AttackedTimer(duration);
        StartCoroutine(m_AttackCoroutine);
    }

    private IEnumerator AttackedTimer(float duration)
    {
        Attack();

        yield return new WaitForSeconds(duration);

        m_Owner.SwitchState(typeof(PlayerIdleState));
    }

    public void ChangeAttackDamage(float amount, bool attackBoost = false)
    {
        m_AttackBoostEffect.SetActive(attackBoost);

        m_HasAttackBoost = attackBoost;

        m_AttackDamage += amount;
    }

    public bool HasAttackBoost()
    {
        return m_HasAttackBoost;
    }
}
