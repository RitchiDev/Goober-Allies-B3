using Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWalker : MonoBehaviour
{
    private float m_MovementSpeed = 10f;

    [Header("Path")]
    private PathFinding m_PathFinder = new PathFinding();
    private IEnumerator m_PathFinderEnumerator;
    private System.Action m_OnTargetReached;

    [Header("Debug")]
    [SerializeField] List<PathTile> m_Path = new List<PathTile>();
    [SerializeField] private bool m_WalkSingleTile = false;

    private void Awake()
    {
        //GameEventManager.AddListener(GameEventType.OnPlayerMoved, StartTileMovement);
    }

    private void Update()
    {
        if (m_WalkSingleTile)
        {
            m_WalkSingleTile = false;

            StartTileMovementTest();
        }
    }

    private void OnDisable()
    {
        m_OnTargetReached = null;   
    }

    public void SetMovementSpeed(float speed)
    {
        m_MovementSpeed = speed;
    }

    #region Single Tile Path

    private void StartTileMovementTest() // Single
    {
        Vector2Int startPosition = MyExtensions.GetPositionToVector2Int(transform.position);

        Vector2Int targetPosition = PlayerManager.Instance.GetPlayerTilePosition();

        WalkTowardsTarget(startPosition, targetPosition);
    }

    public void WalkTowardsTarget(Vector2Int startPosition, Vector2Int targetPosition, System.Action onTargetReached = null) // Single
    {
        m_OnTargetReached = onTargetReached;

        transform.position = new Vector3(startPosition.x, 0, startPosition.y);

        m_PathFinder.SetNewDestination(startPosition, targetPosition);

        RecalculatePath(true);
    }

    private void RecalculatePath(bool resetPath) // Single
    {
        Vector2Int startPosition = MyExtensions.GetPositionToVector2Int(transform.position);

        if (resetPath)
        {
            startPosition = m_PathFinder.StartPosition;
        }

        StopAllCoroutines();
        m_Path.Clear();

        m_Path = m_PathFinder.SearchNewPath(startPosition);

        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath() // Single tile
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition;

        try
        {
            endPosition = m_Path[1].GetPositionToVector3();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        float progress = 0f;
        while (progress < 1f)
        {
            progress += m_MovementSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPosition;

        m_OnTargetReached?.Invoke();
        m_OnTargetReached = null;
    }

    #endregion

    #region Full Path

    private void FollowFullPathTest() // Full
    {
        transform.position = DungeonGenerator.Instance.GetRandomPositionInFirstRoom();

        Vector2Int startPosition = MyExtensions.GetPositionToVector2Int(transform.position);

        Vector2Int targetPosition = DungeonGenerator.Instance.GetRandomTilePositionInLastRoom();

        WalkTillCompletion(startPosition, targetPosition);
    }

    public void WalkTillCompletion(Vector2Int startPosition, Vector2Int targetPosition) // Full
    {
        transform.position = new Vector3(startPosition.x, 0, startPosition.y);

        m_PathFinder.SetNewDestination(startPosition, targetPosition);

        RecalculateFullPath(true);
    }

    private IEnumerator FollowPathTillCompletion() // Full
    {
        for (int i = 1; i < m_Path.Count; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = m_Path[i].GetPositionToVector3();

            //selectedUnit.LookAt(endPosition);

            float progress = 0f;
            while (progress < 1f)
            {
                progress += m_MovementSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, progress);

                yield return new WaitForEndOfFrame();
            }

            transform.position = endPosition;
        }

        m_OnTargetReached?.Invoke();
        m_OnTargetReached = null;
    }

    private void RecalculateFullPath(bool resetPath) // Full
    {
        Vector2Int startPosition = MyExtensions.GetPositionToVector2Int(transform.position);

        if (resetPath)
        {
            startPosition = m_PathFinder.StartPosition;
        }

        StopAllCoroutines();
        m_Path.Clear();

        m_Path = m_PathFinder.SearchNewPath(startPosition);

        StartCoroutine(FollowPathTillCompletion());
    }

    #endregion
}
