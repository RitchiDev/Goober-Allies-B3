using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyAnimation
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float m_AnimationSpeed = 12f;
    [SerializeField] private List<Sprite> m_Frames = new List<Sprite>();
    [SerializeField] private UnityEvent m_OnLoopPointReachedEvent;
    private int m_AnimationIndex;
    private float m_AnimationTimer;

    public void OnEnteredAnimation()
    {
        if (m_SpriteRenderer == null)
        {
            return;
        }

        m_AnimationTimer = 0f;
        m_AnimationIndex = 0;

        m_SpriteRenderer.sprite = m_Frames[m_AnimationIndex];
        FlipX(false);
    }

    public void PlayAnimation()
    {
        if (m_SpriteRenderer == null)
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

            m_SpriteRenderer.sprite = m_Frames[m_AnimationIndex];

            m_AnimationTimer = 0f;
        }
    }

    public void FlipX(bool value)
    {
        if (m_SpriteRenderer == null)
        {
            return;
        }

        m_SpriteRenderer.flipX = value;
    }
}
