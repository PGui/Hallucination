using UnityEngine;
using System.Collections;

public class BoxCorridor : MonoBehaviour
{
    private Corridor temp_parent, temp;
    public int m_instanceID;
    // Use this for initialization
    void Start()
    {
        m_instanceID = GetComponentInParent<Corridor>().instanceID;
        collider.enabled = false;
    }

    void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("BoxCorridor"))
        {
            if (System.Convert.ToInt32(_other.GetComponent<BoxCorridor>().m_instanceID) < System.Convert.ToInt32(this.m_instanceID))
            {
                temp = _other.GetComponentInParent<Corridor>();
                Debug.Log(temp.instanceID);
                temp_parent = temp.m_children[0];
                for (int i=0; i<temp_parent.m_nbExit; i++) //(Corridor _child in temp_parent.m_children)
                {
                    if (temp_parent.m_children[i] != null)
                    {
                        if (temp_parent.m_children[i].instanceID == temp.instanceID)
                        {
                            Debug.Log(temp_parent.instanceID + " child #" + i + " closed");
                            temp_parent.m_exitPositions[i].GetComponentInChildren<MeshCollider>().enabled = true;
                            temp_parent.m_exitPositions[i].GetComponentInChildren<MeshRenderer>().enabled = true;
                        }
                    }
                }
                //_other.transform.parent.gameObject.SetActive(false);
                //Destroy(_other.transform.parent.gameObject);
                temp.poolPosition();
                _other.collider.enabled = false;
            }
        }
    }
}