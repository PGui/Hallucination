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
    public int instanceID=0;
    public bool m_entered = false, m_available = true, m_firstLoop = true;
    public BoxCorridor m_boxCorridor;
    public int m_xGridPos, m_yGridPos;

    private BoxCollider col;

    public TypeCorridor m_typeCorridor;
    public enum TypeCorridor
    {
        Line00 = 0,
        Line01 = 1,
        Ankle10 = 10,
        Ankle11 = 11,
        Triple20 = 20,
        Triple21 = 21,
        Triple22 = 22,
        Cross30 = 30,
        BigRoom40 = 40
    }
    void Start()
    {
        if (instanceID == 0)
        {
            instanceID = GetInstanceID();
            initArrays(this);
        }
        m_boxCorridor = GetComponentInChildren<BoxCorridor>();
        m_boxCorridor.m_instanceID = instanceID;
        m_player = GameObject.Find("Player").GetComponent<Player>();
        col = (BoxCollider)this.m_exitPositions[0].collider;
    }
    void initArrays(Corridor toInit)
    {

      //  Debug.Log(this.name+" initArrays");
        toInit.m_children = new Corridor[toInit.m_nbExit];
        toInit.m_triggers = new TriggerCorridor[toInit.m_nbExit];
        for (int i = 0; i < m_nbExit; i++)
        {
            toInit.m_children[i] = null;
        }
        toInit.m_triggers = this.GetComponentsInChildren<TriggerCorridor>();
    }
    public void OnChildTriggered(Collider _other, TriggerCorridor _triggered)
    {
        //If player enters a new corridor
        if (_other.CompareTag("Player") && _other.GetComponent<Player>().GetCurrentLocation() != this.transform.GetInstanceID())
        {
            m_player.m_creationCounter = 0; //number of child created (limitation to avoid too big/infinite corridors)
            instantiateChildren(_other.GetComponent<Player>().GetCurrentCorridor());
            m_entered = true;
            m_player.SetCurrentLocation(this.instanceID, this);

            if (m_player.m_victoryState == false)
            {
                if (this.m_typeCorridor.GetHashCode() == m_player.m_victoryPattern[m_player.m_corridorCollectionCounter] && m_player.m_corridorCollectionCounter < m_player.m_nbCorridorVictory)
                {
                    m_player.m_corridorCollectionCounter++;
                }
            }

            if ((m_player.GetCurrentCorridor().m_typeCorridor == TypeCorridor.Ankle10 ||
                m_player.GetCurrentCorridor().m_typeCorridor == TypeCorridor.Ankle11) && m_player.GetCurrentCorridor().name != "666")
            {
                int i = 0;
                for (i = 0; i < m_nbExit; i++)
                {
                    if (_triggered == m_exitPositions[i].GetComponent<TriggerCorridor>())
                    {
                        break;
                    }
                }
                //If the entered corridor is an Ankle
                m_entered = false;
                if (m_children[i] != m_player.GetCurrentCorridor() && m_children[i] != null)
                {
                    propagateDeallocationV2(m_children[i]);
                    //StartCoroutine("InstChldReverse",i);
                    m_children[i] = null;
                    instantiateChildrenReverse();
                }
            }
        }
    }
    void instantiateChildren(Corridor previous)
    {
        m_children[0] = previous;

        for (int i = 0; i < m_nbExit; i++)
        {
            if (m_children[i] == null && m_exitPositions[i].GetComponent<TriggerCorridor>().m_planeRend.enabled == false/*.GetComponentInChildren<MeshRenderer>().enabled == false*/)
            {
                m_children[i] = setPoolObjActive(ObjectPool.instance.GetObjectForType(randomObjName(this), false).GetComponent<Corridor>(), this, i/*, false*/);
            }
        }
    }
    void instantiateChildrenReverse()
    {
        //Debug.Log("Par la puissance des fils !");
       // m_children[0] = previous;
        if (m_firstLoop)
        {
          //  Debug.Log("Reverse exitPosition0");
            m_exitPositions[0].eulerAngles += new Vector3(0, 180, 0);
            col.center = new Vector3(col.center.x, col.center.y, col.center.z-1f);
            m_firstLoop = false;
        }
        for (int i = 0; i < m_nbExit; i++)
        {
            if (m_children[i] == null && m_exitPositions[i].GetComponent<TriggerCorridor>().m_planeRend.enabled == false/*.GetComponentInChildren<MeshRenderer>().enabled == false*/)
            {
               // Debug.Log("This is the birth of a new son");
                m_children[i] = setPoolObjActive(ObjectPool.instance.GetObjectForType(randomObjName(this), false).GetComponent<Corridor>(), this, i/*, true*/);
            }
            else
            {
                //Debug.Log("lol");
                //Debug.Log(m_exitPositions[i].GetComponent<TriggerCorridor>().m_planeRend.enabled == false/*.GetComponentInChildren<MeshRenderer>().enabled == false*/);
                //Debug.Log(m_children[i] == null);
                //Debug.Log(m_children[i].name + " " + m_children[i].instanceID);
            }
        }
    }
    void propagateDeallocationV2(Corridor toRemove)
    {
        toRemove.removeFromGrid(toRemove.m_xGridPos, toRemove.m_yGridPos);
        toRemove.poolPosition();
        for (int i = 0; i < toRemove.m_nbExit; i++)
        {
            if (toRemove.m_children[i] != null && toRemove.m_children[i] != m_player.GetCurrentCorridor())
            {
                //Debug.Log(toRemove.name + " -"+i+"- " + toRemove.m_children[i].instanceID);
            }
            if (toRemove.m_children[i] != null && toRemove.m_children[i] != m_player.GetCurrentCorridor() && (Mathf.RoundToInt(toRemove.m_children[i].transform.position.x) != 0 || Mathf.RoundToInt(toRemove.m_children[i].transform.position.y) != -20 || Mathf.RoundToInt(toRemove.m_children[i].transform.position.z) != 0))
            {
                propagateDeallocationV2(toRemove.m_children[i]);
            }
        }
        for (int i = 0; i < toRemove.m_nbExit; i++)
        {
            if (toRemove.m_children[i] != null && toRemove.m_children[i] != m_player.GetCurrentCorridor())
            {
                //Debug.Log("Byebye" + m_children[i].name);
                toRemove.m_children[i] = null;
            }
        }
        toRemove.initArrays(toRemove);
        toRemove.m_available = true;
    }
    Corridor setPoolObjActive(Corridor _moved, Corridor _parent, int _exitID/*, bool isRealloc*/)
    {
        m_player.m_creationCounter++;
      /*  if (isRealloc)
        {
            Debug.Log("Creation backward de " + _moved.name + ":" + _moved.instanceID + " fils de " + _parent.name + ":" + _parent.instanceID + "->sortie #" + _exitID);
        }
        else
        {
            Debug.Log("Creation forward de " + _moved.name + ":" + _moved.instanceID + " fils de " + _parent.name + ":" + _parent.instanceID + "->sortie #" + _exitID);
        }*/
       // Debug.Log(_moved.instanceID+"-"+_moved.name);
        //  Corridor _moved;
       // _moved = ObjectPool.instance.GetObjectForType(randomObjName(_parent), false).GetComponent<Corridor>();
        if (_moved == null) return null;

        if (_moved != null /*&& _moved.m_children[0] == null*/ && _moved.m_nbExit > 0)
        {
            if (_moved.m_boxCorridor == null)
            {
                _moved.m_boxCorridor = _moved.GetComponentInChildren<BoxCorridor>();
            } 
            if (_moved.m_boxCorridor != null)
            {
                _moved.m_boxCorridor.collider.enabled = true;
            }
            if (_moved.instanceID == 0)
            {
                return _parent.closeExit(_parent, _exitID);
            }
            if (_parent != null )
            {
              //  Debug.Log(_moved.name + "-" + _moved.instanceID);
                _moved.m_children[0] = _parent;
            }

            _moved.m_xGridPos = _parent.m_xGridPos;
            _moved.m_yGridPos = _parent.m_yGridPos;
            //Debug.Log("3 :" + _moved.m_xGridPos + " " + _moved.m_yGridPos);
            if (Mathf.RoundToInt(_parent.m_exitPositions[_exitID].eulerAngles.y) == 0)
            {
                _moved.m_yGridPos++;
            }
            else if (Mathf.RoundToInt(_parent.m_exitPositions[_exitID].eulerAngles.y) == 180)
            {
                _moved.m_yGridPos--;
            }
            else if (Mathf.RoundToInt(_parent.m_exitPositions[_exitID].eulerAngles.y) == 90)
            {
                _moved.m_xGridPos++;
            }
            else if (Mathf.RoundToInt(_parent.m_exitPositions[_exitID].eulerAngles.y) == -90 || Mathf.RoundToInt(_parent.m_exitPositions[_exitID].eulerAngles.y) == 270)
            {
                _moved.m_xGridPos--;
            }
           // Debug.Log("3.5" + _moved.m_xGridPos + " " + _moved.m_yGridPos);
            if (checkPositionAvailable(_moved.m_xGridPos, _moved.m_yGridPos))
            {
                addToGrid(_moved.m_xGridPos, _moved.m_yGridPos, _moved.m_typeCorridor.GetHashCode());
                _moved.m_available = false;
                //Place the corridor
                //Debug.Log(_moved.transform.position.ToString());
                _moved.transform.localEulerAngles = new Vector3(Mathf.RoundToInt(_moved.m_exitPositions[0].localEulerAngles.x + _parent.m_exitPositions[_exitID].eulerAngles.x),
                    Mathf.RoundToInt(_moved.m_exitPositions[0].localEulerAngles.y + _parent.m_exitPositions[_exitID].eulerAngles.y),
                    Mathf.RoundToInt(_moved.m_exitPositions[0].localEulerAngles.z + _parent.m_exitPositions[_exitID].eulerAngles.z));
                _moved.transform.position = new Vector3(Mathf.RoundToInt(_parent.m_exitPositions[_exitID].position.x), 
                    Mathf.RoundToInt(_parent.m_exitPositions[_exitID].position.y), 
                    Mathf.RoundToInt(_parent.m_exitPositions[_exitID].position.z));
                
                //VERIF si collision
                if (_moved.m_typeCorridor != TypeCorridor.Ankle10 && _moved.m_typeCorridor != TypeCorridor.Ankle11)
                {
                    for (int j = 0; j < _moved.m_nbExit; j++)
                    {
                        if (_moved.m_children[j] == null && _moved.m_exitPositions[j].GetComponent<TriggerCorridor>().m_planeRend.enabled == false/*.GetComponentInChildren<MeshRenderer>().enabled == false*/)
                        {//VERIF si collision
                            _moved.m_children[j] = setPoolObjActive(ObjectPool.instance.GetObjectForType(randomObjName(_parent), false).GetComponent<Corridor>(), _moved, j/*, isRealloc*/);
                        }
                    }
                } 
                //Debug.Log(_moved.transform.position.ToString());
                return _moved;
            }
           // else
            //{
               // return null;
            //}
        }
        //else
        //{
       // Debug.Log("toClose ID : " + _parent.instanceID + " - exit : " + _exitID);
        return _parent.closeExit(_parent, _exitID);
         //   return null;
        //}
    }

    string randomObjName(Corridor previous)
    {
        randomCorridorID = (int)Random.Range(0, 4); //Select the shape of the corridor (I, L, T, +)
        // m_player.m_creationCounter++;
        //Debug.Log(m_player.m_creationCounter);
        /* if (randomCorridorID == previous.randomCorridorID)
         {
             randomCorridorID--;
             if (randomCorridorID < 0) randomCorridorID = 0;
         }*/
        /* while (randomCorridorID == previous.randomCorridorID)
         {
             Debug.Log("rerand");
             randomCorridorID = (int)Random.Range(0, 4); //Select the shape of the corridor (I, L, T, +)
         }*/
        if (m_player.m_creationCounter > m_player.m_creationLimit)
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
            case 4:
                randomAnchorID = 0;
                break;
        }
        return "Corridor#" + randomCorridorID.ToString() + randomAnchorID.ToString();
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
    public void poolPosition()
    {
        this.openExits(this);
        // this.m_available = true;
        if (this.m_firstLoop == false)
        {
            this.m_exitPositions[0].eulerAngles += new Vector3(0, 180, 0);
            col.center = new Vector3(col.center.x, col.center.y, col.center.z + 1f);
            this.m_firstLoop = true;
        }
        this.m_boxCorridor.collider.enabled = false;
        this.transform.position = new Vector3(0f, -20f, 0f);
        this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
    public bool checkPositionAvailable(int x, int y)
    {
        //Debug.Log("Grid search at " + x + " - " + y);
        for (int i = 0; i < ObjectPool.instance._matrix.Count; i++)
        {
            if (x == ObjectPool.instance._matrix[i].m_x && y == ObjectPool.instance._matrix[i].m_y)
            {
                // Debug.Log(ObjectPool.instance._matrix[i].mToString()+" NOT AVAILABLE !!!");
                return false;
            }
            //Debug.Log(ObjectPool.instance._matrix[i].mToString());
        }
        return true;
    }
    public void addToGrid(int x, int y, int _type)
    {
        if (x > ObjectPool.instance.matrixMaxX) ObjectPool.instance.matrixMaxX = x;
        if (x < ObjectPool.instance.matrixMinX) ObjectPool.instance.matrixMinX = x;
        if (y > ObjectPool.instance.matrixMaxY) ObjectPool.instance.matrixMaxY = y;
        if (y < ObjectPool.instance.matrixMinY) ObjectPool.instance.matrixMinY = y;

        ObjectPool.instance._matrix.Add(new ObjectPool.GridElement { m_x = x, m_y = y, m_type = _type });
    }
    public void removeFromGrid(int _xGrid, int _yGrid)
    {
        for (int i = 0; i < ObjectPool.instance._matrix.Count; i++)
        {
            if (_xGrid == ObjectPool.instance._matrix[i].m_x && _yGrid == ObjectPool.instance._matrix[i].m_y)
            {
                ObjectPool.instance._matrix.RemoveAt(i);
                if (_xGrid == ObjectPool.instance.matrixMaxX && ObjectPool.instance._matrix[0].m_x != null) ObjectPool.instance.matrixMaxX = ObjectPool.instance._matrix[0].m_x;
                if (_xGrid == ObjectPool.instance.matrixMinX && ObjectPool.instance._matrix[0].m_x != null) ObjectPool.instance.matrixMinX = ObjectPool.instance._matrix[0].m_x;
                if (_yGrid == ObjectPool.instance.matrixMaxY && ObjectPool.instance._matrix[0].m_y != null) ObjectPool.instance.matrixMaxY = ObjectPool.instance._matrix[0].m_y;
                if (_yGrid == ObjectPool.instance.matrixMinY && ObjectPool.instance._matrix[0].m_y != null) ObjectPool.instance.matrixMinY = ObjectPool.instance._matrix[0].m_y;
            }
        }
        for (int i = 0; i < ObjectPool.instance._matrix.Count; i++)
        {
            if (ObjectPool.instance._matrix[i].m_x > ObjectPool.instance.matrixMaxX) ObjectPool.instance.matrixMaxX = ObjectPool.instance._matrix[i].m_x;
            if (ObjectPool.instance._matrix[i].m_x < ObjectPool.instance.matrixMinX) ObjectPool.instance.matrixMinX = ObjectPool.instance._matrix[i].m_x;
            if (ObjectPool.instance._matrix[i].m_y > ObjectPool.instance.matrixMaxY) ObjectPool.instance.matrixMaxY = ObjectPool.instance._matrix[i].m_y;
            if (ObjectPool.instance._matrix[i].m_y < ObjectPool.instance.matrixMinY) ObjectPool.instance.matrixMinY = ObjectPool.instance._matrix[i].m_y;
        }
    }
    public Corridor closeExit(Corridor toClose, int idToClose)
    {
        if (toClose != null /*&& toClose.m_children[idToClose] != null*/)
        {
          //  Debug.Log("toClose ID : " + toClose.instanceID + " - exit : " + idToClose);
            toClose.m_exitPositions[idToClose].GetComponent<TriggerCorridor>().m_planeCol.enabled = true/*.GetComponentInChildren<MeshCollider>().enabled = true*/;
            toClose.m_exitPositions[idToClose].GetComponent<TriggerCorridor>().m_planeRend.enabled = true/*.GetComponentInChildren<MeshRenderer>().enabled = true*/;
        }
        return null;
    }
    public void openExits(Corridor toOpen)
    {
        //Debug.Log("toOpen ID : " + toOpen.instanceID);
        for (int i = 0; i < toOpen.m_nbExit; i++) //(Corridor _child in temp_parent.m_children)
        {
            if (toOpen.m_exitPositions[i] != null)
            {
                toOpen.m_exitPositions[i].GetComponent<TriggerCorridor>().m_planeCol.enabled = false/*.GetComponentInChildren<MeshCollider>().enabled = false*/;
                toOpen.m_exitPositions[i].GetComponent<TriggerCorridor>().m_planeRend.enabled = false/*.GetComponentInChildren<MeshRenderer>().enabled = false*/;
            }
        }
    }
}
