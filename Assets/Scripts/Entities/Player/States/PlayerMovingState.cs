using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovingState : BaseState, IMoveable
{
    [Header("Components")]
    [SerializeField] private Player m_Player;
    [SerializeField] private InputHandler m_Input;

    [Header("Movement")]
    [SerializeField] private AudioSource m_WalkAudioSource;
    [SerializeField] private TrailRenderer m_SpeedBoostTrailRenderer;
    [SerializeField] private float m_MovementSpeed = 8f;
    private bool m_HasSpeedBoost;

    [Header("Path")]
    [SerializeField] private PathWalker m_PathWalker;
    [SerializeField] private LayerMask m_Obstacles;

    [Header("Camera")]
    [SerializeField] private Camera m_Camera;

    private void Awake()
    {
        m_PathWalker.SetMovementSpeed(m_MovementSpeed);
    }

    public override void OnEnter()
    {
        m_Animation.OnEnteredAnimation();

        m_WalkAudioSource.Play();

        Vector3 direction = m_Input.MovementDirection;

        if (direction == Vector3.zero) // Running
        {
            direction = m_Player.GetRunDirection();
        }

        bool hitX = Physics.Raycast(transform.position, direction, direction.x, m_Obstacles);
        bool hitZ = Physics.Raycast(transform.position, direction, direction.z, m_Obstacles);

        if (hitX || hitZ) // Hit a wall
        {
            m_Owner.SwitchState(typeof(PlayerIdleState));

            return;
        }

        Vector2Int startPosition = MyExtensions.GetPositionToVector2Int(transform.position);

        CheckForFlip(direction.x);

        Vector2Int targetPosition = new Vector2Int((int)direction.x, (int)direction.z);
        targetPosition.x += startPosition.x;
        targetPosition.y += startPosition.y;

        m_PathWalker.WalkTowardsTarget(startPosition, targetPosition, OnPlayerMoved);
    }

    public override void OnExit()
    {
        m_WalkAudioSource.Stop();
    }

    public override void OnUpdate()
    {
        m_Animation.PlayAnimation();
    }

    private void OnPlayerMoved()
    {
        GameEventManager.RaiseEvent(GameEventType.OnPlayerMoved);

        m_Owner.SwitchState(typeof(PlayerIdleState));
    }

    private Vector3 LookDirection()
    {
        Vector2 movementDirection = m_Input.MovementDirection;

        return new Vector3(movementDirection.x, 0f, movementDirection.y);
    }

    private void CheckForFlip(float xDirection)
    {
        if (xDirection > 0)
        {
            m_Animation.FlipX(false);
        }
        else if (xDirection < 0)
        {
            m_Animation.FlipX(true);
        }
    }

    public void ChangeMovementSpeed(float amount, bool speedBoost = false)
    {
        m_SpeedBoostTrailRenderer.gameObject.SetActive(speedBoost);

        m_HasSpeedBoost = speedBoost;

        m_MovementSpeed += amount;
    }

    public bool HasSpeedBoost()
    {
        return m_HasSpeedBoost;
    }
}
