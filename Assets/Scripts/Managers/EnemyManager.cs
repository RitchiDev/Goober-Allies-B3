using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Instance")]
    [SerializeField] private bool m_DontDestroyOnLoad = true;
    [SerializeField] private bool m_WarnOnExistingInstance = true;

    [Header("Player")]
    [SerializeField] private List<Enemy> m_CurrentActiveEnemies = new List<Enemy>();

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

    public void AddEnemy(Enemy enemy)
    {
        m_CurrentActiveEnemies.Add(enemy);
    }
}
