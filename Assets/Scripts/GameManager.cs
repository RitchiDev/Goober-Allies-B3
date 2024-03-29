using Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Instance")]
    [SerializeField] private bool m_DontDestroyOnLoad = false;
    [SerializeField] private bool m_WarnOnExistingInstance = true;

    [Header("Items")]
    [SerializeField] private List<GameObject> m_Items = new List<GameObject>();

    [Header("Enemy")]
    [SerializeField] private int m_MaxEnemiesPerRoom = 3;
    [SerializeField] public List<EnemyHolderData> m_Enemies = new List<EnemyHolderData>();
    private List<GameObject> m_ActiveEnemies = new List<GameObject>();

    [Header("Scenes")]
    [SerializeField] private int m_TitleScreenScene = 0;
    [SerializeField] private int m_VictoryScene = 3;
    [SerializeField] private int m_GameOverScene = 4;

    private void Awake()
    {
        CreateInstance();
    }

    private void Update()
    {
        if (Keyboard.current.backspaceKey.wasReleasedThisFrame)
        {
            ReloadScene();
        }
        else if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            LoadScene(m_TitleScreenScene);
        }
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

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void SpawnEnemiesInRooms()
    {
        List<DungeonRoom> rooms = DungeonGenerator.Instance.GetRooms();

        if (rooms.Count <= 1)
        {
            return;
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            if (i <= 0)
            {
                continue;
            }

            SpawnRandomEnemiesInRoom(rooms[i]);
        }
    }

    public void CreateItems()
    {
        List<DungeonRoom> rooms = DungeonGenerator.Instance.GetRooms();

        if (rooms.Count <= 1)
        {
            return;
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            if (i <= 0 || i >= rooms.Count)
            {
                continue;
            }

            GameObject item = Instantiate(m_Items[Random.Range(0, m_Items.Count)], rooms[i].GetRandomPositionInRoom(), Quaternion.identity);
        }
    }

    private void SpawnRandomEnemiesInRoom(DungeonRoom room)
    {
        float random = Random.value;
        float cumulative = 0f;

        List<Vector3> takenPositionsInRoom = new List<Vector3>();
        int spawnedEnemies = 0;

        for (int i = 0; i < m_Enemies.Count; i++)
        {
            cumulative += m_Enemies[i].SpawnChance;

            if (random < cumulative)
            {
                EnemyHolderData chosenEnemy = m_Enemies[i];

                Vector3 spawnPosition = room.GetRandomPositionInRoom();

                int breakout = 0;
                while (takenPositionsInRoom.Contains(spawnPosition))
                {
                    spawnPosition = room.GetRandomPositionInRoom();

                    breakout++;
                    if (breakout >= 20)
                    {
                        Debug.LogWarning("Broke the loop!");
                        break;
                    }
                }

                takenPositionsInRoom.Add(spawnPosition);
                Instantiate(chosenEnemy.EnemyPrefab, spawnPosition, Quaternion.identity);
                spawnedEnemies++;

                if (spawnedEnemies >= m_MaxEnemiesPerRoom)
                {
                    break;
                }
            }
        }
    }

    public void Victory()
    {
        LoadScene(m_VictoryScene);
    }

    public void GameOver()
    {
        LoadScene(m_GameOverScene);
    }
}
