using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Calls the TakeDamage() function.
    /// </summary>
    /// <param name="damage">Amount of damage.</param>
    public void TakeDamage(float damage);
    public void SetInvincible(bool value);
    public bool IsInvincible();
    public IEnumerator FlashSpriteRendererTimer();
}
