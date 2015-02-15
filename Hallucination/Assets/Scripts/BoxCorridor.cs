using UnityEngine;
using System.Collections;

public class BoxCorridor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("BoxCorridor"))
        {
            if (System.Convert.ToInt32(_other.transform.parent.name) < System.Convert.ToInt32(this.transform.parent.name))
            {
                Destroy(_other.transform.parent.gameObject);
            }
        }
    }
}