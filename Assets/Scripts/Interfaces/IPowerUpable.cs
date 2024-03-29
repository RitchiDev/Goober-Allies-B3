using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerUpable
{
    public void GainPowerUp(PowerUpData powerUp);
    public void RemovePowerUp(PowerUpData powerUp);
    public void UpdatePowerUps();
}
