using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUpData : ScriptableObject
{
    protected MonoBehaviour m_Owner;
    protected IPowerUpable m_PowerUpable;

    [SerializeField] protected float m_CooldownTime = 3;
    private IEnumerator m_CooldownTimerEnumerator;
    protected System.Action m_ActionOnCooldownEnded;

    public virtual void Initialize(MonoBehaviour owner, IPowerUpable powerUpable)
    {
        m_Owner = owner;
        m_PowerUpable = powerUpable;

        StartCooldownTimer();
    }

    public virtual void StartCooldownTimer()
    {
        if (m_CooldownTimerEnumerator != null)
        {
            m_Owner.StopCoroutine(m_CooldownTimerEnumerator);
            m_CooldownTimerEnumerator = null;
        }

        m_CooldownTimerEnumerator = CooldownTimer(m_CooldownTime);
        m_Owner.StartCoroutine(m_CooldownTimerEnumerator); // Coroutines can only be started on a MonoBehaviour.
    }

    public virtual IEnumerator CooldownTimer(float duration)
    {
        float progress = 0;
        while (progress <= 1f)
        {
            progress += Time.deltaTime / duration;

            yield return null;
        }

        OnCooldownEnded();
    }

    public virtual void OnCooldownEnded()
    {
        m_ActionOnCooldownEnded?.Invoke();
        m_ActionOnCooldownEnded = null;

        m_PowerUpable.RemovePowerUp(this);
    }

    public abstract void OnUpdate();
}
