using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : BaseState
{
    [Header("Components")]
    [SerializeField] private Player m_Player;
    [SerializeField] private InputHandler m_Input;

    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();

        CheckForMovingState();
    }

    private void CheckForMovingState()
    {
        if (!IsTryingToMove() || m_Player.IsInBattle())
        {
            return;
        }

        m_Owner.SwitchState(typeof(PlayerMovingState));
    }

    private bool IsTryingToMove()
    {
        return m_Input.MovementDirection != Vector3.zero;
    }
}
