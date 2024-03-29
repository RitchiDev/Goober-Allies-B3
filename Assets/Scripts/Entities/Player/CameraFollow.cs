using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float m_FollowSpeed = 10f;
    [SerializeField] private Transform m_Target;

    private void LateUpdate()
    {
        if (m_Target == null)
        {
            return;
        }
            
        Vector3 position = transform.position;
        position.x = m_Target.position.x;
        position.z = m_Target.position.z;

        position.y = 10;

        transform.position = Vector3.Lerp(transform.position, position, m_FollowSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform transform)
    {
        m_Target = transform;
    }
}
