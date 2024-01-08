using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] Vector2 m_targetPosition;
    [SerializeField] float m_speed = 5;

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            m_targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        transform.position = Vector2.MoveTowards(transform.position, m_targetPosition,m_speed*Time.deltaTime);
        LookAt2D(m_targetPosition);
    }

    void LookAt2D(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
