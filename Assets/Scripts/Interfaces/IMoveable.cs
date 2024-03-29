using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    public void ChangeMovementSpeed(float amount, bool speedBoost = false);
    public bool HasSpeedBoost();
}
