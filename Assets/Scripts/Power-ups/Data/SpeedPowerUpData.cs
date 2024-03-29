using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Power-up/Speed", fileName = "Speed Power-up")]
public class SpeedPowerUpData : PowerUpData
{
    [Header("Power-up")]
    [SerializeField] private float m_AddedSpeed = 3;

    public override void Initialize(MonoBehaviour owner, IPowerUpable powerUpable)
    {
        m_Owner = owner;
        m_PowerUpable = powerUpable;

        IMoveable moveable = owner.GetComponent<IMoveable>();

        if (moveable != null)
        {
            moveable.ChangeMovementSpeed(m_AddedSpeed, true);

            m_ActionOnCooldownEnded +=
                () =>
                {
                    // Remove speed
                    moveable.ChangeMovementSpeed(-m_AddedSpeed);
                };
        }

        StartCooldownTimer();
    }

    public override void OnUpdate()
    {

    }
}
