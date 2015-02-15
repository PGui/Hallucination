using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public int m_currentCorridor = -1;
    public Corridor m_currentCorridorInstance;
    public int m_creationLimit = 5;
    public static int m_creationCounter = 0;

    void Start()
    {
        m_currentCorridorInstance = GameObject.Find("666").GetComponent<Corridor>();
        m_currentCorridorInstance.m_triggers = m_currentCorridorInstance.GetComponentsInChildren<TriggerCorridor>();
        m_currentCorridorInstance.DisableTriggers();
        m_currentCorridor = m_currentCorridorInstance.GetInstanceID();
    }
    public int GetCurrentLocation()
    {
        if (m_currentCorridor == 0)
        {
            Debug.Log("There might be an error, current Corridor is uninitialized");
        }

        return m_currentCorridor;
    }
    public Corridor GetCurrentCorridor()
    {
        if (m_currentCorridorInstance == null)
        {
            Debug.Log("There might be an error, current Corridor is uninitialized");
        }

        return m_currentCorridorInstance;
    }


    public void SetCurrentLocation(int transformID, Corridor _corridor)
    {
        m_currentCorridorInstance.EnableTriggers();
        m_currentCorridor = transformID;
        m_currentCorridorInstance = _corridor;
        m_currentCorridorInstance.DisableTriggers();
    }
}
