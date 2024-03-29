using Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Instance")]
    [SerializeField] private bool m_DontDestroyOnLoad = false;
    [SerializeField] private bool m_WarnOnExistingInstance = true;

    [Header("UI")]
    [SerializeField] private GameObject m_NoPlayersImage;
    [SerializeField] private GameObject m_OnThirdPlayerImage;

    [Header("Player")]
    [SerializeField] private List<GameObject> m_PlayerPrefabs = new List<GameObject>();
    [SerializeField] private List<Player> m_CurrentActivePlayers = new List<Player>();
    private bool m_GameHasStarted = false;

    private void Awake()
    {
        CreateInstance();
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

    public void AddPlayer(Player player)
    {
        if (m_CurrentActivePlayers.Contains(player))
        {
            Debug.LogWarning("This player was already added!");
            return;
        }

        m_GameHasStarted = true;

        m_CurrentActivePlayers.Add(player);

        UpdatePlayersUI();
    }

    public void RemovePlayer(Player player)
    {
        if (!m_CurrentActivePlayers.Contains(player))
        {
            Debug.LogWarning("This player is not in the active players list!");
            return;
        }

        m_CurrentActivePlayers.Remove(player);

        CheckForGameOver();
        //UpdatePlayersUI();
    }

    private void CheckForGameOver()
    {
        if (!m_GameHasStarted)
        {
            return;
        }

        if (m_CurrentActivePlayers.Count >= 1)
        {
            return;
        }

        GameManager.Instance.GameOver();
    }

    private void UpdatePlayersUI()
    {
        if (m_OnThirdPlayerImage != null)
        {
            m_OnThirdPlayerImage.SetActive(m_CurrentActivePlayers.Count == 3);
        }

        if (m_NoPlayersImage != null)
        {
            m_NoPlayersImage.SetActive(m_CurrentActivePlayers.Count == 0);
        }
    }

    public Vector2Int GetPlayerTilePosition()
    {
        Vector2Int position = MyExtensions.GetPositionToVector2Int(m_CurrentActivePlayers[0].transform.position);

        return position;
    }

    public Player GetClosestPlayer(Vector3 position)
    {
        Player closestPlayer = null;

        float minDistance = Mathf.Infinity;

        for (int i = 0; i < m_CurrentActivePlayers.Count; i++)
        {
            Vector3 playerPosition = m_CurrentActivePlayers[i].transform.position;

            float distance = Vector3.Distance(playerPosition, position);

            if (distance < minDistance)
            {
                closestPlayer = m_CurrentActivePlayers[i];
                minDistance = distance;
            }
        }

        return closestPlayer;
    }

    public int GetPlayerCount()
    {
        return m_CurrentActivePlayers.Count;
    }
}
