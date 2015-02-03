using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Corridor : MonoBehaviour {

    public int m_nbExit;
    public Transform[] m_exitPositions;
    public Corridor[] m_children = null;
    public BoxCollider[] m_triggers = null;
    public Corridor RandomCorridor;
    public GameObject tempCorridor;
    public Corridor RandomCorridor2;
    private int randomCorridorID; //Select the shape of the corridor (I, L, T, +)
    private int randomAnchorID;
    private Player m_player;
    public bool m_entered = false;

    public TypeCorridor m_typeCorridor;
    public enum TypeCorridor
    {
        Line,
        Ankle,
        Triple,
        Cross
    }

	void Start () {
        initArrays();
        m_player = GameObject.Find("Player").GetComponent<Player>();
	}
    void initArrays()
    {
        m_children = new Corridor[m_nbExit];
        m_triggers = new BoxCollider[m_nbExit];
        for (int i = 0; i < m_nbExit; i++)
        {
            m_children[i] = null;
        }
        m_triggers = this.GetComponentsInChildren<BoxCollider>();
    }
    public void OnChildTriggered(Collider _other)
    {
        if (_other.CompareTag("Player") && _other.GetComponent<Player>().GetCurrentLocation() != this.transform.GetInstanceID())
        {
            Player.m_creationCounter = 0;
            StartCoroutine("InstantiateChildren", _other.GetComponent<Player>().GetCurrentCorridor());//InstantiateChildren(/*_other.GetComponent<Player>().GetCurrentCorridor()*/);
            m_entered = true;
                m_player.SetCurrentLocation(this.transform.GetInstanceID(), this);
                if (m_player.GetCurrentCorridor().m_typeCorridor == TypeCorridor.Ankle && m_player.GetCurrentCorridor().name != "Corridor#01Start")
                {
                    m_entered = false;
                    //for (int i = 0; i < m_nbExit; i++)
                    //{
                    if (m_children[0] != m_player.GetCurrentCorridor() && m_children[0] != null)
                    {
                        StartCoroutine("PropagateDeallocation", m_children[0]);
                    }
                    //}
                }
        }
    }

    /*** Check all Corridor Children and instantiate one for each exit ***/
    IEnumerator InstantiateChildren(Corridor previous)
    {
       // previous need to be set as a child, equivalent to AddExit function...to do later
        if (m_children[0] == null)
        {
            m_children[0] = previous;
        }

        for (int i = 0; i < m_nbExit; i++)
        {
            if (m_children[i] == null)
            {
                Player.m_creationCounter++;
                RandomCorridor = InstantiateCorridor(m_exitPositions[i]); 
                RandomCorridor.initArrays(); //Init new corridor arrays
                m_children[i] = RandomCorridor; //The random Corridor is one of the corridor's child
                if (RandomCorridor.m_typeCorridor != TypeCorridor.Ankle)
                {
                    //If the new corridor is not an ankle
                    yield return new WaitForSeconds(0.01f);
                    m_children[i].StartCoroutine("InstantiateChildren",this); 
                }
            }
         //   m_children[i].m_children[m_nbExit-1] = this; //Add preious corridor as a child too
        }
    }
    Corridor InstantiateCorridor(Transform exitPosition)
    {
        randomCorridorID = (int)Random.Range(0, 4); //Select the shape of the corridor (I, L, T, +)

        //Avoir unlimited corridor, if counter > limit, force creating an ankle
        if (Player.m_creationCounter > m_player.m_creationLimit)
        {
            randomCorridorID = 1;
        }
        randomAnchorID = 0;
        switch (randomCorridorID)
        {
            case 0:
                randomAnchorID = (int)Random.Range(0, 2);
                break;
            case 1:
                randomAnchorID = (int)Random.Range(0, 2);
                break;
            case 2:
                randomAnchorID = (int)Random.Range(0, 3);
                break;
            case 3:
                randomAnchorID = 0;
                break;
        }

        tempCorridor = GameObject.Instantiate(Resources.Load("Prefab/Corridor#" + randomCorridorID.ToString() + randomAnchorID.ToString())) as GameObject;

        //Rotate the corridor
        tempCorridor.transform.localEulerAngles = new Vector3(Mathf.Round(tempCorridor.GetComponent<Corridor>().m_exitPositions[0].localEulerAngles.x + exitPosition.eulerAngles.x), Mathf.Round(tempCorridor.GetComponent<Corridor>().m_exitPositions[0].localEulerAngles.y + exitPosition.eulerAngles.y), Mathf.Round(tempCorridor.GetComponent<Corridor>().m_exitPositions[0].localEulerAngles.z + exitPosition.eulerAngles.z));
        
        //Place the corridor
        tempCorridor.transform.position = new Vector3(exitPosition.position.x, exitPosition.position.y, exitPosition.position.z);

        //Rename the corridor with InstanceID so that they have different names
        tempCorridor.name = tempCorridor.GetInstanceID().ToString();

        return tempCorridor.GetComponent<Corridor>();
    }
    public void EnableTriggers()
    {
        for (int i = 0; i < m_nbExit; i++)
        {
            m_triggers[i].enabled = true;
        }
    }
    public void DisableTriggers()
    {
        for (int i = 0; i < m_nbExit; i++)
        {
            m_triggers[i].enabled = false;
        }
    }
    IEnumerator PropagateDeallocation(Corridor parent)
    {
        for (int i = 0; i < parent.m_nbExit; i++)
        {
            if (parent.m_children[i] != parent && parent.m_children[i] != null && parent.m_children[i] != m_player.GetCurrentCorridor())
            {
                yield return new WaitForSeconds(0.05f + i * 0.1f);
                StartCoroutine("PropagateDeallocation", parent.m_children[i]);
            }
        }
        if (parent != null)
        {
            Destroy(parent.gameObject);
        }
    }

    void AddExit(Corridor corridor)
    {
       /* bool result = false;

        for (int i = 0; i < m_nbExit; i++)
        {
            Debug.Log(i);
            if (m_children[i] != null)
            {
                Debug.Log("Test"+i);
                m_children[i] = corridor;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("Error");
        }
        */
    }
}
