using UnityEngine;

public class TriggerCorridor : MonoBehaviour
{
    public Corridor m_Corridor;
    public MeshCollider m_planeCol;
    public MeshRenderer m_planeRend;
    public bool m_enabled = true;

    void Start()
    {
        m_Corridor = transform.parent.GetComponent<Corridor>();
        m_planeCol.enabled = false;
        m_planeRend.enabled = false;
    }
    void OnTriggerEnter(Collider _other)
    {
        if (enabled)
        {
            //Debug.Log(transform.parent.name + ">" + name + "Triggered");
            m_Corridor.OnChildTriggered(_other, this);
        }
    }
}
