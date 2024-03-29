using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputHandler m_Input;

    [Header("Crosshair")]
    [SerializeField] private Transform m_CrosshairParent;
    [SerializeField] private Transform m_Crosshair;
    [SerializeField] private float m_LerpSpeed = 100f;

    [Header("Target")]
    [SerializeField] private Transform m_Target;

    private void OnDisable()
    {
        m_CrosshairParent.gameObject.SetActive(false);   
    }

    private void Update()
    {
        MoveCrosshairParent();
        HandleCrosshair();
    }

    /// <summary>
    /// Handles the position/rotation of the crosshair.
    /// </summary>
    private void HandleCrosshair()
    {
        Vector3 direction = m_Input.AimDirection;

        if (m_Input.AimDirection == Vector3.zero)
        {
            direction = m_Input.MovementDirection;
        }

        if (direction == Vector3.zero)
        {
            return;
        }

        // Apply the angle as the rotation of the object
        m_CrosshairParent.rotation = Quaternion.LookRotation(direction);
    }

    private void MoveCrosshairParent()
    {
        if (m_CrosshairParent == null)
        {
            return;
        }

        m_CrosshairParent.position = Vector3.Lerp(m_CrosshairParent.position, m_Target.position, Time.deltaTime * m_LerpSpeed);
    }
}
