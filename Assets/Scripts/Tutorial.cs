using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> m_TextList = new List<TMP_Text>();
    private int m_Index;

    private void OnEnable()
    {
        GameEventManager.AddListener(GameEventType.OnPlayerMoved, NextText);
    }

    private void OnDisable()
    {
        GameEventManager.RemoveListener(GameEventType.OnPlayerMoved, NextText);
    }

    public void NextText()
    {
        for (int i = 0; i < m_TextList.Count; i++)
        {
            m_TextList[i].gameObject.SetActive(false);
        }

        m_Index++;

        if (m_Index >= m_TextList.Count - 1)
        {
            m_TextList[m_TextList.Count - 1].gameObject.SetActive(true);

            return;
        }

        m_TextList[m_Index].gameObject.SetActive(true);
    }
}
