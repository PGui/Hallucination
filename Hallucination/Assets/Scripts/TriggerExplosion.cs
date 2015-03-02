using UnityEngine;
using System.Collections;

public class TriggerExplosion : MonoBehaviour
{
    public ParticleSystem[] m_emitters = null;
    public SandExplosion[] m_sandExplosion = null;
	// Use this for initialization
	void Start () {
        m_emitters = gameObject.GetComponentsInChildren<ParticleSystem>();
        m_sandExplosion = gameObject.GetComponentsInChildren<SandExplosion>();
        for (int i = 0; i < m_sandExplosion.Length; i++)
        {
           // m_sandExplosion[i].rigidbody.mass = 0;
            m_sandExplosion[i].rigidbody.useGravity = false;
            m_sandExplosion[i].rigidbody.isKinematic = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("e"))
        {
            startExplosion();
        }
        else if (Input.GetKeyDown("r"))
        {
            resetExplosion();
        }
	}
    public void startExplosion()
    {
        for (int i = 0; i < m_sandExplosion.Length; i++)
        {
            m_sandExplosion[i].rigidbody.mass = 1;
            m_sandExplosion[i].rigidbody.useGravity = true;
            m_sandExplosion[i].rigidbody.isKinematic = false;
            m_sandExplosion[i].explosion();
        }

        for (int i = 0; i < m_emitters.Length; i++)
        {
            m_emitters[i].gameObject.particleSystem.Play();
        }
    }
    public void resetExplosion()
    {
        for (int i = 0; i < m_emitters.Length; i++)
        {
            m_emitters[i].gameObject.particleSystem.Stop();
        }
        for (int i = 0; i < m_sandExplosion.Length; i++)
        {
            m_sandExplosion[i].reset();
        }
    }
}
