using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealAttackState : EnemyAttackState
{
    [Header("Heal")]
    [SerializeField] protected float m_HealAmount = 3f;
    [SerializeField] private Health m_Health;

    protected override void Attack()
    {
        m_AttackEffect.Play();

        TextPopUp textPopUp = Instantiate(m_TextPopUpPrefab, transform.position, Quaternion.identity);
        textPopUp.SetText($"Enemy healed for: {m_HealAmount} amount!");
        textPopUp.SetColor(m_TextPopUpColor);

        // Heal enemy
        m_Health.ReceiveHealing(m_HealAmount);
    }
}
