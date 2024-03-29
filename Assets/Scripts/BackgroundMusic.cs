using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    [Header("Instance")]
    [SerializeField] private bool m_DontDestroyOnLoad = false;
    [SerializeField] private bool m_WarnOnExistingInstance = true;

    [Header("Scene")]
    [SerializeField] private List<int> m_ActiveScenes = new List<int>();

    private void Awake()
    {
        CreateInstance();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (m_ActiveScenes.Contains(scene.buildIndex))
        {
            return;
        }

        Destroy(gameObject);
    }
}
