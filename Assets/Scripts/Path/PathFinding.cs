using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeon;

// Pathfinding made with the tutorial created by: https://youtu.be/4JaHSLA2CKs?si=LbdEybvyOFEoUWyb

public class PathFinding
{
    [SerializeField] Vector2Int m_StartPosition;
    public Vector2Int StartPosition => m_StartPosition;

    [SerializeField] Vector2Int m_TargetPosition;
    public Vector2Int TargetPosition => m_TargetPosition;

    private Queue<PathTile> m_Frontier;
    private Dictionary<Vector2Int, PathTile> m_Reached;

    private Dictionary<Vector2Int, PathTile> m_CurrentGrid;

    [Header("Debug")]
    [SerializeField] private PathTile m_StartTile;
    [SerializeField] private PathTile m_TargetTile;
    [SerializeField] private PathTile m_CurrentTile;

    public void SetNewDestination(Vector2Int startCoordinates, Vector2Int targetCoordinates)
    {
        if (m_CurrentGrid == null)
        {
            m_CurrentGrid = DungeonGenerator.Instance.CurrentGrid;
        }

        m_StartPosition = startCoordinates;
        m_TargetPosition = targetCoordinates;

        if (m_CurrentTile != null)
        {
            m_CurrentTile.SetOccupation(TileOccupation.Available);
            m_CurrentTile = null;
        }

        m_StartTile = m_CurrentGrid[m_StartPosition];
        m_TargetTile = m_CurrentGrid[m_TargetPosition];

        m_Frontier = new Queue<PathTile>();
        m_Reached = new Dictionary<Vector2Int, PathTile>();

        GetNewPath();
    }

    public List<PathTile> GetNewPath()
    {
        return SearchNewPath(m_StartPosition);
    }

    public List<PathTile> SearchNewPath(Vector2Int startPosition) // Get a new path
    {
        FirstSearch(startPosition);
        
        return BuildPath();
    }

    private void FirstSearch(Vector2Int startPosition)
    {
        m_StartTile.SetOccupation(TileOccupation.Available);
        //m_TargetTile.SetOccupation(TileOccupation.Available);

        m_Frontier.Clear();
        m_Reached.Clear();

        m_Frontier.Enqueue(m_CurrentGrid[startPosition]);
        m_Reached.Add(startPosition, m_CurrentGrid[startPosition]);

        while (m_Frontier.Count > 0)
        {
            m_CurrentTile = m_Frontier.Dequeue();

            CheckNeighborTiles();
            
            if (m_CurrentTile.GetPosition() == m_TargetPosition)
            {
                //m_CurrentTile.SetOccupation(TileOccupation.Unit);

                break;
            }
        }
    }

    private void CheckNeighborTiles()
    {
        List<PathTile> neighbors = new List<PathTile>();

        Vector2Int[] searchDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down};

        foreach (Vector2Int direction in searchDirections)
        {
            Vector2Int neighborPosition = m_CurrentTile.GetPosition() + direction;

            if (m_CurrentGrid.ContainsKey(neighborPosition))
            {
                neighbors.Add(m_CurrentGrid[neighborPosition]);
            }
        }

        foreach (PathTile neighbor in neighbors)
        {
            if (!m_Reached.ContainsKey(neighbor.GetPosition()) && neighbor.GetTileOccupation() == TileOccupation.Available)
            {
                neighbor.SetPathTileConnectedTo(m_CurrentTile);

                m_Reached.Add(neighbor.GetPosition(), neighbor);
                m_Frontier.Enqueue(neighbor);
            }
        }
    }

    private List<PathTile> BuildPath()
    {
        PathTile currentPathTile = m_TargetTile;

        List<PathTile> path = new List<PathTile>();
        path.Add(currentPathTile);

        while (currentPathTile.GetPathTileConnectedTo() != null)
        {
            currentPathTile = currentPathTile.GetPathTileConnectedTo();

            path.Add(currentPathTile);

            if (currentPathTile == m_StartTile)
            {
                break;
            }
        }

        path.Reverse();

        return path;
    }
}