using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    /// <summary>
    /// Calls the ReceiveHealing() function.
    /// </summary>
    /// <param name="healAmount">Amount of healing.</param>
    public void ReceiveHealing(float healAmount);

    /// <summary>
    /// Calls the FullHeal() function.
    /// </summary>
    public void FullHeal();

    public bool IsFullHealth();
}
