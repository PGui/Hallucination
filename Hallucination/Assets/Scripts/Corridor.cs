using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Corridor : MonoBehaviour {

    public int m_nbExit;
    public Transform[] m_exitPositions;
    public Corridor[] m_children = null;
    public TriggerCorridor[] m_triggers = null;
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
        m_triggers = new TriggerCorridor[m_nbExit];
        for (int i = 0; i < m_nbExit; i++)
        {
            m_children[i] = null;
        }
        m_triggers = this.GetComponentsInChildren<TriggerCorridor>();
    }
    public void OnChildTriggered(Collider _other, TriggerCorridor _triggered)
    {
        if (_other.CompareTag("Player") && _other.GetComponent<Player>().GetCurrentLocation() != this.transform.GetInstanceID())
        {
          //  if (_triggered != m_children[0])
            Player.m_creationCounter = 0;
            StartCoroutine("InstantiateChildren", _other.GetComponent<Player>().GetCurrentCorridor());//InstantiateChildren(/*_other.GetComponent<Player>().GetCurrentCorridor()*/);
            m_entered = true;
                m_player.SetCurrentLocation(this.transform.GetInstanceID(), this);
                if (m_player.GetCurrentCorridor().m_typeCorridor == TypeCorridor.Ankle && m_player.GetCurrentCorridor().name != "666")
                {
                    m_entered = false;
                    //for (int i = 0; i < m_nbExit; i++)
                    //{
                    if (m_children[0] != m_player.GetCurrentCorridor() && m_children[0] != null)
                    {
                        StartCoroutine("PropagateDeallocation", m_children[0]);
                        StartCoroutine(InstantiateChildrenReverse(this, _triggered != m_children[0]));
                    }
                    //}
                }
        }
    }
    Transform reverseAngle(Transform _toRevese)
    {
        if (_toRevese.eulerAngles.y == 180)
        {
            _toRevese.eulerAngles = new Vector3(_toRevese.eulerAngles.x, 0f, _toRevese.eulerAngles.z);
        }
        else if (_toRevese.eulerAngles.y == 90)
        {
            _toRevese.eulerAngles = new Vector3(_toRevese.eulerAngles.x, 270f, _toRevese.eulerAngles.z);
        }
        if (_toRevese.eulerAngles.y == 0)
        {
            _toRevese.eulerAngles = new Vector3(_toRevese.eulerAngles.x, 180f, _toRevese.eulerAngles.z);
        }
        else if (_toRevese.eulerAngles.y == 270)
        {
            _toRevese.eulerAngles = new Vector3(_toRevese.eulerAngles.x, 90f, _toRevese.eulerAngles.z);
        }
        return _toRevese;
    }
    IEnumerator InstantiateChildrenReverse(Corridor previous, bool firstLoop = false)
    {
        yield return new WaitForSeconds(1f);
       // Debug.Log("Je vais créer tes fils dans l'autre sens !");
        for (int i = 0; i < 1/*previous.m_nbExit*/; i++)
        {
            //Debug.Log(this.name + i + /*previous.name +*/ " - " + (m_children[i] == null)); //Debug current corridor name and if current child is null
            if (m_children[i] == null)
            {
                // Debug.Log(this.name + i /*+ previous.name*/);
                Player.m_creationCounter++;
                Transform test = m_exitPositions[i];
                if (firstLoop)
                {
                   test = reverseAngle(test);
                }
                RandomCorridor2 = InstantiateCorridor(m_exitPositions[i]); //RandomCorridor = the new instantiated Corridor
                m_exitPositions[i].eulerAngles = new Vector3(m_exitPositions[i].eulerAngles.x, 0f, m_exitPositions[i].eulerAngles.z);
                //    m_corridorManager.AddNewCorridor(RandomCorridor2);
                RandomCorridor2.initArrays(); //Init new corridor arrays
                // Debug.Log("Adding : " + RandomCorridor.name + " to " + this.name + "children[" + i + "]");
                m_children[i] = RandomCorridor2; //The random Corridor is one of the corridor's child
                //Debug.Log("ADDED" + m_children[i].name);
                //      test.Add(RandomCorridor2.name);
                if (RandomCorridor2.m_typeCorridor != TypeCorridor.Ankle)
                {
                    //If the new corridor is not an ankle
                    yield return new WaitForSeconds(0.01f);
                    // Debug.Log(Player.m_creationCounter + " >? " + m_player.m_creationLimit);
                    m_children[i].StartCoroutine(InstantiateChildrenReverse(this, false)); //.InstantiateChildren(); //Instantiate a new child.
                    // m_children[i].SendMessage("InstantiateChildren");
                }
            }
            //   m_children[i].m_children[m_nbExit-1] = this; //Add preious corridor as a child too
        }
        //PRESQUE
        yield return new WaitForSeconds(0.1f);
        Player.m_creationCounter = 0;
        m_children[0].StartCoroutine("InstantiateChildren", m_player.GetCurrentCorridor());
        // Debug.Log(this.name + " lol "+ test[0]);
        // this.m_children[0] = GameObject.Find("" + test[0] + "").GetComponent<Corridor>();
    }

    /*** Check all Corridor Children and instantiate one for each exit ***/
    IEnumerator InstantiateChildren(Corridor previous)
    {
        Debug.Log("CHILD");
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
                    if (m_children[i] != null)
                    {
                        m_children[i].StartCoroutine("InstantiateChildren", this);
                    }
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
            if (m_triggers[i] != null)
            {
                m_triggers[i].GetComponent<BoxCollider>().enabled = true;
            }
        }
    }
    public void DisableTriggers()
    {
        for (int i = 0; i < m_nbExit; i++)
        {
            if (m_triggers[i] != null)
            {
                m_triggers[i].GetComponent<BoxCollider>().enabled = false;
            }
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
