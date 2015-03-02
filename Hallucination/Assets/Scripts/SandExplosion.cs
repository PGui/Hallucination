using UnityEngine;
using System.Collections;

public class SandExplosion : MonoBehaviour {

    public bool test = false;
    public float m_X=0,m_Y=0,m_Z=0,m_down=0, m_timeBeforeFall;
    public Vector3 m_initPosition;

    void Start()
    {
        m_initPosition = this.transform.position;
    }
    public void reset()
    {
        rigidbody.AddForce(0f, 0f, 0f);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.isKinematic = true;
        //rigidbody.mass = 0;
        rigidbody.useGravity = false;
        this.transform.position = m_initPosition;
    }
    public void explosion()
    {
        rigidbody.AddForce(m_X, m_Y, m_Z, ForceMode.Impulse);
        StartCoroutine("inverse");
	}
	
	// Update is called once per frame
	void Update () {
        if(test)
        rigidbody.AddForce(0, m_down, 0, ForceMode.Impulse);
	}
    IEnumerator inverse()
    {
        float randomt = Random.Range(0.3f,1f);
       // Debug.Log(randomt);
        yield return new WaitForSeconds(m_timeBeforeFall + randomt);
        test = true;
    }
}
