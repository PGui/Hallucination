using UnityEngine;

public class TriggerCorridor : MonoBehaviour
{
    public Corridor m_Corridor;
    public bool m_enabled = true;

    void Start()
    {
        m_Corridor = transform.parent.GetComponent<Corridor>();
    }
    void OnTriggerEnter(Collider _other)
    {
        if (enabled)
        {
            m_Corridor.OnChildTriggered(_other, this);
        }
    }
}
