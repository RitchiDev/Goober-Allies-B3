using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy", fileName = "Enemy Holder")]
public class EnemyHolderData : ScriptableObject
{
    [SerializeField] private GameObject m_EnemyPrefab;
    public GameObject EnemyPrefab => m_EnemyPrefab;

    [Range(0.01f, 1f)]
    [SerializeField] private float m_SpawnChance = 1f;
    public float SpawnChance => m_SpawnChance;
}
