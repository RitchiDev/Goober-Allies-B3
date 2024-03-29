using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    [Header("Health")]
    [SerializeField] private float m_MaxHealth;
    [SerializeField] private Slider m_HealthSlider;
    private float m_CurrentHealth;

    [Header("Invincible")]
    [SerializeField] private bool m_IsInvincibleAfterDamage;
    [SerializeField] private float m_InvincibleAfterDamageTime = 1.5f;
    private bool m_IsInvincible = false;
    private IEnumerator m_InvincibilityTimer;

    [Header("Hurt")]
    [SerializeField] private float m_TimeHurt = 0.5f;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Color m_ColorOnHurt = Color.red;

    [Header("Effects")]
    [SerializeField] private ParticleSystem m_DeathEffectPrefab;

    [Header("Events")]
    [SerializeField] private UnityEvent m_OnReceiveHealing;

    [SerializeField] private UnityEvent m_OnAlreadyMaxHealthEvent;

    [SerializeField] private UnityEvent m_OnReceiveDamage;

    [SerializeField] private UnityEvent m_OnDeath;

    private void Awake()
    {
        SetHealth(m_MaxHealth);
    }

    public void SetHealth(float amount)
    {
        m_CurrentHealth = amount;

        UpdateHealthSlider(m_CurrentHealth, m_MaxHealth);
    }

    public void ChangeMaxHealth(float amount)
    {
        m_MaxHealth = amount;
    }

    public float GetMaxHealth()
    {
        return m_MaxHealth;
    }

    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }

    public float GetHealthProgress()
    {
        return m_CurrentHealth / m_MaxHealth;
    }

    public bool IsDamaged()
    {
        return m_CurrentHealth != m_MaxHealth;
    }

    public void AddListenerToOnDeath(UnityAction function)
    {
        m_OnDeath.AddListener(function);
    }

    public void AddListenerToReceiveDamage(UnityAction function)
    {
        m_OnReceiveDamage.AddListener(function);
    }

    public void AddListenerToReceiveHealing(UnityAction function)
    {
        m_OnReceiveHealing.AddListener(function);
    }

    public void RemoveListenerFromOnDeath(UnityAction function)
    {
        m_OnDeath.RemoveListener(function);
    }

    public void RemoveListenerFromReceiveDamage(UnityAction function)
    {
        m_OnReceiveDamage.RemoveListener(function);
    }

    public void RemoveListenerFromReceiveHealing(UnityAction function)
    {
        m_OnReceiveHealing.RemoveListener(function);
    }

    /// <summary>
    /// Used as inspector event
    /// </summary>
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Invokes death event.
    /// </summary>
    private void RaiseDeathEvent()
    {
        if (m_DeathEffectPrefab != null)
        {
            ParticleSystem deathEffect = Instantiate(m_DeathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 1f);
        }

        m_OnDeath?.Invoke();
    }

    public void InstaKill()
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - m_CurrentHealth, 0, m_MaxHealth);

        UpdateHealthSlider(m_CurrentHealth, m_MaxHealth);

        m_OnReceiveDamage?.Invoke();

        // Died
        RaiseDeathEvent();
    }

    /// <summary>
    /// Add damage to the current health.
    /// </summary>
    /// <param name="damage">Damage to deal.</param>
    public void TakeDamage(float damage)
    {
        if (m_IsInvincible)
        {
            return;
        }

        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - damage, 0, m_MaxHealth);

        UpdateHealthSlider(m_CurrentHealth, m_MaxHealth);

        m_OnReceiveDamage?.Invoke();

        if (m_CurrentHealth <= 0) // Died
        {
            RaiseDeathEvent();

            return;
        }

        if (m_IsInvincibleAfterDamage)
        {
            StartInvincibilityTimer(m_InvincibleAfterDamageTime);
        }

        if (m_SpriteRenderer != null)
        {
            StartCoroutine(FlashSpriteRendererTimer());
        }
    }

    /// <summary>
    /// Add health to the current health.
    /// </summary>
    public void ReceiveHealing(float healAmount)
    {
        // When health is already max.
        if (m_CurrentHealth >= m_MaxHealth)
        {
            RaiseAlreadyMaxHealthEvent();
        }

        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth + healAmount, 0, m_MaxHealth);

        UpdateHealthSlider(m_CurrentHealth, m_MaxHealth);

        m_OnReceiveHealing?.Invoke();
    }

    public void FullHeal()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    private void RaiseAlreadyMaxHealthEvent()
    {
        m_OnAlreadyMaxHealthEvent?.Invoke();
    }

    public void StartInvincibilityTimer(float duration)
    {
        if (m_InvincibilityTimer != null)
        {
            StopCoroutine(m_InvincibilityTimer);
            m_InvincibilityTimer = null;
        }

        m_InvincibilityTimer = InvincibilityTimer(duration);
        StartCoroutine(m_InvincibilityTimer);
    }

    private IEnumerator InvincibilityTimer(float duration)
    {
        SetInvincible(true);

        yield return new WaitForSeconds(duration);

        SetInvincible(false);
    }

    public void SetInvincible(bool value)
    {
        if (m_SpriteRenderer)
        {
            Color newColor = m_SpriteRenderer.color;
            newColor.a = value ? 0.5f : 1f;

            m_SpriteRenderer.color = newColor;
        }

        m_IsInvincible = value;
    }

    public bool IsInvincible()
    {
        return m_IsInvincible;
    }

    public IEnumerator FlashSpriteRendererTimer()
    {
        m_SpriteRenderer.color = m_ColorOnHurt;

        yield return new WaitForSeconds(m_TimeHurt);

        m_SpriteRenderer.color = Color.white;
    }

    private void UpdateHealthSlider(float currentHealth, float maximumHealth)
    {
        if (m_HealthSlider == null)
        {
            return;
        }

        if (m_HealthSlider.maxValue != maximumHealth)
        {
            m_HealthSlider.maxValue = maximumHealth;
        }

        m_HealthSlider.value = currentHealth;
    }

    public bool IsFullHealth()
    {
        return m_CurrentHealth >= m_MaxHealth;
    }
}
