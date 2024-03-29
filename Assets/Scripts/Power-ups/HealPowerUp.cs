using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPowerUp : MonoBehaviour
{
    [SerializeField] private TextPopUp m_TextPopUpPrefab;
    [SerializeField] private Color m_TextPopUpColor = Color.green;
    [SerializeField] private float m_HealAmount = 2f;

    private void OnTriggerStay(Collider other)
    {
        IHealable healable = other.GetComponent<IHealable>();

        if (healable == null)
        {
            return;
        }

        if (healable.IsFullHealth())
        {
            return;
        }

        TextPopUp textPopUp = Instantiate(m_TextPopUpPrefab, transform.position, Quaternion.identity);
        textPopUp.SetText($"Enemy healed for: {m_HealAmount} amount!");
        textPopUp.SetColor(m_TextPopUpColor);

        healable.ReceiveHealing(m_HealAmount);
        Destroy(gameObject);
    }
}
