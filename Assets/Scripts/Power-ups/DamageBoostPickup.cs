using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBoostPickup : PowerUpPickup
{
    public override void OnPickUp(Collider other)
    {
        IPowerUpable powerUpable = other.GetComponent<IPowerUpable>();
        if (powerUpable == null)
        {
            return;
        }

        IDamageDealable damageDealable = other.GetComponent<IDamageDealable>();
        if (damageDealable == null)
        {
            return;
        }

        if (damageDealable.HasAttackBoost())
        {
            return;
        }

        PowerUpData powerUp = Instantiate(m_PowerUp); // To prevent issues when mutliple GameObjects use the same Power-up.
        powerUpable.GainPowerUp(powerUp);

        Destroy(gameObject);
    }
}
