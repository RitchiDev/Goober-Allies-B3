using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [System.Serializable]
    public class DungeonRoom
    {
        public int minX { get; private set; }
        public int maxX { get; private set; }
        public int minZ { get; private set; }
        public int maxZ { get; private set; }

        public DungeonRoom(int _minX, int _maxX, int _minZ, int _maxZ)
        {
            minX = _minX;
            maxX = _maxX;
            minZ = _minZ;
            maxZ = _maxZ;
        }

        public Vector3 GetFloatCenter()
        {
            return new Vector3(Mathf.RoundToInt(Mathf.Lerp(minX, maxX, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(minZ, maxZ, 0.5f)));
        }

        public Vector3Int GetCenter()
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(minX, maxX, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(minZ, maxZ, 0.5f)));
        }

        public Vector3Int GetRandomPositionInRoom()
        {
            return new Vector3Int(Random.Range(minX, maxX + 1), 0, Random.Range(minZ, maxZ + 1));
        }
    }
}