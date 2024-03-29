using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MyImageAnimation
{
    [SerializeField] private Image m_Image;
    [SerializeField] private float m_AnimationSpeed = 12f;
    [SerializeField] private List<Sprite> m_Frames = new List<Sprite>();
    [SerializeField] private UnityEvent m_OnLoopPointReachedEvent;
    private int m_AnimationIndex;
    private float m_AnimationTimer;

    public void OnEnteredAnimation()
    {
        if (m_Image == null)
        {
            return;
        }

        m_AnimationTimer = 0f;
        m_AnimationIndex = 0;

        m_Image.sprite = m_Frames[m_AnimationIndex];
    }

    public void PlayAnimation()
    {
        if (m_Image == null)
        {
            return;
        }

        m_AnimationTimer += Time.deltaTime;

        if (m_AnimationTimer > (1f / m_AnimationSpeed))
        {
            m_AnimationIndex++;

            if (m_AnimationIndex >= m_Frames.Count)
            {

                m_OnLoopPointReachedEvent?.Invoke();
                m_AnimationIndex = 0;
            }

            m_Image.sprite = m_Frames[m_AnimationIndex];

            m_AnimationTimer = 0f;
        }
    }
}
