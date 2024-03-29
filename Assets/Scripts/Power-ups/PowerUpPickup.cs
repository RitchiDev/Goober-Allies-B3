using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] protected PowerUpData m_PowerUp;

    private void OnTriggerStay(Collider other)
    {
        OnPickUp(other);
    }

    public virtual void OnPickUp(Collider other)
    {
        IPowerUpable powerUpable = other.GetComponent<IPowerUpable>();
        if (powerUpable != null)
        {
            PowerUpData powerUp = Instantiate(m_PowerUp); // To prevent issues when mutliple GameObjects use the same Power-up.
            powerUpable.GainPowerUp(powerUp);

            Destroy(gameObject);
        }
    }
}
