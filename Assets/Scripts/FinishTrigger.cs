using Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private int m_SceneToLoad = 2;

    private void OnEnable()
    {
        GameEventManager.AddListener(GameEventType.OnGameStarted, SetUp);
    }

    private void OnDisable()
    {
        GameEventManager.RemoveListener(GameEventType.OnGameStarted, SetUp);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            SceneManager.LoadScene(m_SceneToLoad);
        }
    }

    private void SetUp()
    {
        transform.position = DungeonGenerator.Instance.GetRandomPositionInLastRoom();
    }

    public Vector2Int GetTilePosition()
    {
        return MyExtensions.GetPositionToVector2Int(transform.position);
    }
}
