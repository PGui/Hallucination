using UnityEngine;
using System.Collections;

public class TriggerTurtle : MonoBehaviour {

    public enum Trigger_Type
    {
        appear,
        up,
        down
    }
    public Trigger_Type m_Trigger_Type;
    public TurtleAnims m_turtleAnims;
    public TriggerExplosion m_explosion;
    void OnTriggerEnter(Collider _other)
    {
        if (m_Trigger_Type == Trigger_Type.appear)
        {
            m_turtleAnims.launchAnimation();
            StartCoroutine("startExplo");
        }
        else if (m_Trigger_Type == Trigger_Type.up)
        {
            m_turtleAnims.animUp();
        }
        else if (m_Trigger_Type == Trigger_Type.down)
        {
            m_turtleAnims.animDown();
        }
    }
    void Update()
    {
        if (m_Trigger_Type == Trigger_Type.appear && Input.GetKeyDown(KeyCode.N))
        {
            m_turtleAnims.launchAnimation();
            StartCoroutine("startExplo");
        }
    }
    IEnumerator startExplo()
    {
        yield return new WaitForSeconds(0.5f);
        m_explosion.startExplosion();
    }
}