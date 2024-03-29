using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageDealable 
{
    public void ChangeAttackDamage(float amount, bool attackBoost = false);
    public bool HasAttackBoost();
}
