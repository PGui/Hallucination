using UnityEngine;
using System.Collections;

public class Corridor : MonoBehaviour {

    public int m_nbExit;
    public Transform[] m_exitPositions;
    private Corridor[] m_children = null;

	// Use this for initialization
	void Start () {
        m_children = new Corridor[m_nbExit];
        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Player") && _other.GetComponent<Player>().GetCurrentLocation() != this.transform.GetInstanceID())
        {
            InstantiateChildren();
            for (int i = 0; i < m_nbExit; i++)
            {
                m_children[i].PropagateDeallocation(this);
            }
            _other.GetComponent<Player>().SetCurrentLocation(this.transform.GetInstanceID());
        }
    }

    Corridor InstantiateCorridor(Transform exitPosition)
    {
        Corridor tempCorridor;
        int randomCorridorID = (int) Random.Range(0,4); //Select the shape of the corridor (I, L, T, +)
        int randomAnchorID = 0; 
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
        tempCorridor = Instantiate(Resources.Load("Corridor#" + randomCorridorID.ToString() + randomAnchorID.ToString())) as Corridor;
        tempCorridor.transform.position = new Vector3(exitPosition.position.x, exitPosition.position.y, exitPosition.position.z);

        //int randomExitID = (int)Random.Range(0, tempCorridor.m_nbExit);
        tempCorridor.transform.localEulerAngles = new Vector3(tempCorridor.m_exitPositions[0].localEulerAngles.x + exitPosition.localEulerAngles.x, tempCorridor.m_exitPositions[0].localEulerAngles.y + exitPosition.localEulerAngles.y, tempCorridor.m_exitPositions[0].localEulerAngles.z + exitPosition.localEulerAngles.z);
        
        /***** Rescale -1 to revert 'L-shaped' Corridor -> problem when not using the "default" exitPoint
        if(randomCorridorID == 1) {
            if ((int)Random.Range(0, 100) % 2 == 0)
            {
                tempCorridor.transform.localScale = new Vector3(1f, -1f, 1f);
            }
        }
        */

        return tempCorridor;
    }

    void InstantiateChildren()
    {
        for (int i = 0; i < m_nbExit; i++)
        {
            if (m_children[i] == null)
            {
                Corridor RandomCorridor = new Corridor(); //Instantiate(RandomCorridor);
                //m_children[i] = CreateInstance(m_exitPositions[i]);
                RandomCorridor.AddExit(this);
                if (RandomCorridor.m_nbExit > 2)
                {
                    RandomCorridor.InstantiateChildren();
                }
            }
        }
    }
    void PropagateDeallocation(Corridor parent)
    {
        if (m_nbExit > 2)
        {
            for (int i = 0; i < m_nbExit; i++)
            {
                if (m_children[i] != parent && m_children[i] != null)
                {
                    m_children[i].PropagateDeallocation(this);
                }
            }
        }
        else
        {
            for (int i = 0; i < m_nbExit; i++)
            {
                Destroy(m_children[i]);
            }
        }
    }

    void AddExit(Corridor corridor)
    {
        bool result = false;

        for (int i = 0; i < m_nbExit; i++)
        {
            if (m_children[i] != null)
            {
                m_children[i] = corridor;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("Error");
        }
    }
}
