using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostPickup : PowerUpPickup
{
    public override void OnPickUp(Collider other)
    {
        IPowerUpable powerUpable = other.GetComponent<IPowerUpable>();
        if (powerUpable == null)
        {
            return;
        }

        IMoveable moveable = other.GetComponent<IMoveable>();
        if (moveable == null)
        {
            return;
        }

        if (moveable.HasSpeedBoost())
        {
            return;
        }

        PowerUpData powerUp = Instantiate(m_PowerUp); // To prevent issues when mutliple GameObjects use the same Power-up.
        powerUpable.GainPowerUp(powerUp);

        Destroy(gameObject);
    }
}
