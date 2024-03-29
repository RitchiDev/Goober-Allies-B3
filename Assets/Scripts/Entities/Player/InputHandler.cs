using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Movement")]
    private Vector3 m_MovementDirection;
    public Vector3 MovementDirection => m_MovementDirection;

    [Header("Aim")]
    private Vector3 m_AimDirection;
    public Vector3 AimDirection => m_AimDirection;

    [Header("Mouse")]
    private Vector2 m_MousePosition;
    public Vector2 MousePosition => m_MousePosition;

    [Header("Defend")]
    private bool m_IsDefending;
    public bool IsDefending => m_IsDefending;

    [Header("Attack")]
    private bool m_IsAttacking;
    public bool IsAttacking => m_IsAttacking;

    private void OnDisable()
    {
        m_MovementDirection = Vector3.zero;
        m_AimDirection = Vector3.zero;
        m_MousePosition = Vector2.zero;
        m_IsDefending = false;
        m_IsAttacking = false;
    }

    #region Input

    // To disable a player's input, call PlayerInput.DeactivateInput. To re-enable it, call PlayerInput.ActivateInput. The latter enables the default Action Map, if it exists.

    public void OnMousePosition(InputValue value)
    {
        if (!enabled)
        {
            return;
        }

        m_MousePosition = value.Get<Vector2>();
    }

    public void OnMovement(InputValue value)
    {
        if (!enabled)
        {
            return;
        }
        
        m_MovementDirection.x = value.Get<Vector2>().x;
        m_MovementDirection.z = value.Get<Vector2>().y;
    }

    public void OnAttack(InputValue value)
    {
        if (!enabled)
        {
            return;
        }
        
        m_IsAttacking = value.isPressed;
    }

    public void OnDefend(InputValue value)
    {
        if (!enabled)
        {
            return;
        }
        
        StartCoroutine(IsDefendingTimer());
    }

    public void OnAim(InputValue value)
    {
        if (!enabled)
        {
            return;
        }
        
        m_AimDirection.x = value.Get<Vector2>().x;
        m_AimDirection.z = value.Get<Vector2>().y;
    }

    #endregion

    private IEnumerator IsDefendingTimer()
    {
        m_IsDefending = true;

        yield return new WaitForEndOfFrame();

        m_IsDefending = false;
    }
}
