using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathTile
{
    [SerializeField] private Vector2Int m_Position;

    [Header("Debug")]
    [SerializeField] private TileOccupation m_TileOccupation;
    [SerializeField] private PathTile pathTileConnectedTo;

    public PathTile(Vector2Int position, TileOccupation tileOccupation)
    {
        m_Position = position;
        m_TileOccupation = tileOccupation;
    }

    public void SetOccupation(TileOccupation tileOccupation)
    {
        m_TileOccupation = tileOccupation;
    }

    public TileOccupation GetTileOccupation()
    {
        return m_TileOccupation;
    }

    public void SetPathTileConnectedTo(PathTile pathTile)
    {
        pathTileConnectedTo = pathTile;
    }

    public PathTile GetPathTileConnectedTo()
    {
        return pathTileConnectedTo;
    }

    public Vector2Int GetPosition()
    {
        return m_Position;
    }

    public Vector3 GetPositionToVector3()
    {
        Vector3 position = new Vector3(GetPositionX(), 0f, GetPositionY());
        return position;
    }

    public int GetPositionX()
    {
        return m_Position.x;
    }

    public int GetPositionY()
    {
        return m_Position.y;
    }
}
