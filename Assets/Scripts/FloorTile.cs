using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private List<Material> m_Materials = new List<Material>();

    private void OnEnable()
    {
        SetRandomFloorTile();
    }

    private void SetRandomFloorTile()
    {
        if (m_Materials.Count <= 0)
        {
            return;
        }

        int rng = Random.Range(0, 10);

        if (rng <= 8)
        {
            return;
        }

        rng = Random.Range(0, m_Materials.Count);

        m_MeshRenderer.material = m_Materials[rng];
    }
}
