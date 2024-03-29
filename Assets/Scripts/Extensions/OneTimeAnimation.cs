using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeAnimation : MonoBehaviour
{
    [SerializeField] private MyAnimation m_Animation;

    private void OnEnable()
    {
        m_Animation.OnEnteredAnimation();
    }

    private void Update()
    {
        m_Animation.PlayAnimation();
    }

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
