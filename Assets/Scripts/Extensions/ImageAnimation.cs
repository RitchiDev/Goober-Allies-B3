using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageAnimation : MonoBehaviour
{
    [SerializeField] private MyImageAnimation m_Animation;

    private void Update()
    {
        m_Animation.PlayAnimation();
    }
}
