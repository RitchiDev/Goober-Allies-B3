using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine.Events;

namespace Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        public static DungeonGenerator Instance { get; private set; }

        [Header("Instance")]
        [SerializeField] private bool m_DontDestroyOnLoad = true;
        [SerializeField] private bool m_WarnOnExistingInstance = true;

        [Header("Dungeon")]
        [SerializeField] private UnityEvent m_OnDungeonGeneratedEvent;

        [Header("Prefab")]
        [SerializeField] private GameObject m_FloorPrefab;
        [SerializeField] private GameObject m_WallPrefab;

        [Header("Grid")]
        [SerializeField] private int m_GridWidth = 50;
        [SerializeField] private int m_GridHeight = 70;
        private int m_GridMargin;
        private Dictionary<Vector2Int, PathTile> m_CurrentGrid = new Dictionary<Vector2Int, PathTile>();
        public Dictionary<Vector2Int, PathTile> CurrentGrid => m_CurrentGrid;

        [Header("Rooms")]
        [SerializeField] private int m_RoomYPosition = -1;
        [SerializeField] private int m_MinimumRoomSize = 6;
        [SerializeField] private int m_MaximumRoomSize = 12;
        [SerializeField] private int m_NumberOfRooms = 4;

        [Header("Debug")]
        [SerializeField] private List<DungeonRoom> m_RoomList = new List<DungeonRoom>();
        [SerializeField] private Dictionary<Vector3Int, DungeonTileType> m_Dungeon = new Dictionary<Vector3Int, DungeonTileType>();
        [SerializeField] private List<GameObject> m_AllInstantiatedPrefabs = new List<GameObject>();

        private void Awake()
        {
            CreateInstance();
        }

        private void Start()
        {
            m_GridMargin = m_MaximumRoomSize + 1;

            SetupPathTileGrid();

            Generate();
        }

        /// <summary>
        /// Check if an instance already exists. If it does, destroy the duplicate class.
        /// </summary>
        private void CreateInstance()
        {
            if (Instance)
            {
                if (m_WarnOnExistingInstance)
                {
                    Debug.LogWarning("An instance of: " + Instance.ToString() + " already exists!");
                }

                if (m_DontDestroyOnLoad)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(this);
                }
            }
            else
            {
                Instance = this;

                if (m_DontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        private void SetupPathTileGrid()
        {
            for (int x = 0; x < m_GridWidth + m_GridMargin; x++)
            {
                for (int y = 0; y < m_GridHeight + m_GridMargin; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    m_CurrentGrid.Add(position, new PathTile(position, TileOccupation.Unavailable));
                }
            }
        }

        /// <summary>
        /// Generates the dungeon
        /// </summary>
        [ContextMenu("Generate Dungeon")]
        public void Generate()
        {
            Debug.Log("Start Generating");

            ClearDungeon();
            AllocateRooms();
            ConnectRooms();
            AllocateWalls();
            SpawnDungeon();

            m_OnDungeonGeneratedEvent?.Invoke();
        }

        [ContextMenu("Clear Dungeon")]
        public void ClearDungeon()
        {
            for (int i = m_AllInstantiatedPrefabs.Count - 1; i >= 0; i--)
            {
                Destroy(m_AllInstantiatedPrefabs[i]);    
            }

            m_Dungeon.Clear();
            m_RoomList.Clear();
        }

        // There's a slight chance of a soft lock happening.
        private void ConnectRooms()
        {
            for (int i = 0; i < m_RoomList.Count; i++)
            {
                DungeonRoom room = m_RoomList[i];
                DungeonRoom otherRoom = m_RoomList[(i + Random.Range(1, m_RoomList.Count)) % m_RoomList.Count];
                ConnectRooms(room, otherRoom);
            }
        }

        private void AllocateRooms()
        {
            for (int i = 0; i < m_NumberOfRooms; i++)
            {
                int minX = Random.Range(0, m_GridWidth);
                int maxX = minX + Random.Range(m_MinimumRoomSize, m_MaximumRoomSize + 1);

                int minZ = Random.Range(0, m_GridHeight);
                int maxZ = minZ + Random.Range(m_MinimumRoomSize, m_MaximumRoomSize + 1);

                DungeonRoom room = new DungeonRoom(minX, maxX, minZ, maxZ);

                if (CanRoomFitInDungeon(room))
                {
                    AddRoomToDungeon(room);
                }
                else
                {
                    i--;
                }
            }
        }

        public void AllocateWalls()
        {
            List<Vector3Int> keys = m_Dungeon.Keys.ToList();

            foreach(Vector3Int kv in keys)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3Int newPosition = kv + new Vector3Int(x, 0, z);

                        if (m_Dungeon.ContainsKey(newPosition))
                        {
                            continue;
                        }

                        m_Dungeon.Add(newPosition, DungeonTileType.Wall);
                    }
                }
            }
        }

        public void ConnectRooms(DungeonRoom roomOne, DungeonRoom roomTwo)
        {
            Vector3Int positionOne = roomOne.GetCenter();
            Vector3Int positionTwo = roomTwo.GetCenter();

            int xDirection = positionTwo.x > positionOne.x ? 1 : -1;

            int x = 0;
            for (x = positionOne.x; x != positionTwo.x; x += xDirection)
            {
                Vector3Int position = new Vector3Int(x, 0, positionOne.z);
                if (m_Dungeon.ContainsKey(position))
                {
                    continue;
                }

                m_Dungeon.Add(position, DungeonTileType.Floor);
            }

            for (x = positionOne.x; x != positionTwo.x; x += xDirection)
            {
                Vector3Int position = new Vector3Int(x - 1, 0, positionOne.z - 1);
                if (m_Dungeon.ContainsKey(position))
                {
                    continue;
                }

                m_Dungeon.Add(position, DungeonTileType.Floor);
            }

            int zDirection = positionTwo.z > positionOne.z ? 1 : -1;
            for (int z = positionOne.z; z != positionTwo.z; z += zDirection)
            {
                Vector3Int position = new Vector3Int(x, 0, z);
                if (m_Dungeon.ContainsKey(position))
                {
                    continue;
                }

                m_Dungeon.Add(position, DungeonTileType.Floor);
            }

            for (int z = positionOne.z; z != positionTwo.z; z += zDirection)
            {
                Vector3Int position = new Vector3Int(x - 1, 0, z - 1);
                if (m_Dungeon.ContainsKey(position))
                {
                    continue;
                }

                m_Dungeon.Add(position, DungeonTileType.Floor);
            }
        }

        public void SpawnDungeon()
        {
            foreach (KeyValuePair<Vector3Int, DungeonTileType> kv in m_Dungeon)
            {
                GameObject newDungeonObject = null;

                switch (kv.Value)
                { 
                    case DungeonTileType.Floor:

                        Vector2Int pathTilePosition = new Vector2Int(kv.Key.x, kv.Key.z);
                        m_CurrentGrid[pathTilePosition].SetOccupation(TileOccupation.Available);

                        newDungeonObject = Instantiate(m_FloorPrefab, kv.Key, m_FloorPrefab.transform.rotation, transform); 

                    break;

                    case DungeonTileType.Wall:
                        
                        newDungeonObject = Instantiate(m_WallPrefab, kv.Key, m_WallPrefab.transform.rotation, transform); 

                    break;
                }

                m_AllInstantiatedPrefabs.Add(newDungeonObject);
            }

            SetRoomsPosition();

            GameEventManager.RaiseEvent(GameEventType.OnGameStarted);
        }

        private void SetRoomsPosition()
        {
            transform.position = new Vector3(0, m_RoomYPosition, 0);
        }

        public void AddRoomToDungeon(DungeonRoom room)
        {
            for (int x = room.minX; x <= room.maxX; x++)
            {
                for (int z = room.minZ; z <= room.maxZ; z++)
                {
                    m_Dungeon.Add(new Vector3Int(x, 0, z), DungeonTileType.Floor);
                }
            }

            m_RoomList.Add(room);
        }
    
        public bool CanRoomFitInDungeon(DungeonRoom room)
        {
            for (int x = room.minX - 1; x <= room.maxX + 2; x++)
            {
                for (int z = room.minZ - 1; z <= room.maxZ + 2; z++)
                {
                    if (m_Dungeon.ContainsKey(new Vector3Int(x, 0, z)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Vector2Int GetRandomTilePositionInFirstRoom()
        {
            Vector3Int positionInFirstRoom = GetRandomPositionInFirstRoom();

            Vector2Int tilePosition = new Vector2Int(positionInFirstRoom.x, positionInFirstRoom.z);

            return tilePosition;
        }

        public Vector2Int GetRandomTilePositionInLastRoom()
        {
            Vector3Int positionInLastRoom = GetRandomPositionInLastRoom();

            Vector2Int tilePosition = new Vector2Int(positionInLastRoom.x, positionInLastRoom.z);

            return tilePosition;
        }

        public Vector3Int GetRandomPositionInFirstRoom()
        {
            Vector3Int randomPositionInRoom = GetRooms()[0].GetRandomPositionInRoom();
            return randomPositionInRoom;
        }

        public Vector3Int GetRandomPositionInLastRoom()
        {
            Vector3Int randomPositionInRoom = GetRooms()[GetRooms().Count - 1].GetRandomPositionInRoom();
            return randomPositionInRoom;
        }

        public List<DungeonRoom> GetRooms()
        {
            return m_RoomList;
        }
    }
}