using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Power-up/Damage", fileName = "Damage Power-up")]
public class DamagePowerUpData : PowerUpData
{
    [Header("Power-up")]
    [SerializeField] private float m_AddedDamage = 3;

    public override void Initialize(MonoBehaviour owner, IPowerUpable powerUpable)
    {
        m_Owner = owner;
        m_PowerUpable = powerUpable;

        IDamageDealable damageDealable = owner.GetComponent<IDamageDealable>();

        if (damageDealable != null)
        {
            damageDealable.ChangeAttackDamage(m_AddedDamage, true);

            m_ActionOnCooldownEnded +=
                () =>
                {
                    // Remove speed
                    damageDealable.ChangeAttackDamage(-m_AddedDamage);
                };
        }

        StartCooldownTimer();
    }

    public override void OnUpdate()
    {

    }
}
