using UnityEngine;
using System.Collections;

public class ChickenAnim : MonoBehaviour
{
    public float m_movementThreshold = 1.0f;

    private Animator m_animator = null;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 speed = rigidbody.velocity;
        if(speed.magnitude > m_movementThreshold)
        {
            m_animator.SetBool("isMoving", true);
        }
        else
        {
            m_animator.SetBool("isMoving", false);
        }
    }
}
