using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private int m_currentCorridor = 0;

    public int GetCurrentLocation()
    {
        if (m_currentCorridor == 0)
        {
            Debug.Log("There might be an error, current Corridor is uninitialized");
        }

        return m_currentCorridor;
    }

    public void SetCurrentLocation(int transformID)
    {
        m_currentCorridor = transformID;
    }
}
