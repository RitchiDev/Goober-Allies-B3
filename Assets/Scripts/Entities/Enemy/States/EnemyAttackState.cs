using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : BaseState
{
    [Header("Components")]
    [SerializeField] protected Enemy m_Enemy;

    [Header("Attack")]
    [SerializeField] protected float m_AttackDamage = 5f;
    [SerializeField] protected float m_DamageMargin = 1f;
    [SerializeField] protected float m_AttackDuration = 0.2f;
    [SerializeField] protected ParticleSystem m_AttackEffect;
    [SerializeField] protected TextPopUp m_TextPopUpPrefab;
    [SerializeField] protected Color m_TextPopUpColor = Color.red;
    private IEnumerator m_AttackCoroutine;

    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();

        StartAttack(m_AttackDuration);
    }

    public override void OnExit()
    {
        GameEventManager.RaiseEvent(GameEventType.OnEnemyAttackEnded);
    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();
    }

    protected virtual void Attack()
    {
        m_AttackEffect.Play();

        float damage = m_AttackDamage + Mathf.RoundToInt(Random.Range(-m_DamageMargin, m_DamageMargin));

        TextPopUp textPopUp = Instantiate(m_TextPopUpPrefab, transform.position, Quaternion.identity);
        textPopUp.SetText($"Player took: {damage} damage!");
        textPopUp.SetColor(m_TextPopUpColor);

        m_Enemy.GetDamageablePlayer().TakeDamage(damage);
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

        m_Owner.SwitchState(typeof(EnemyIdleState));
    }
}
