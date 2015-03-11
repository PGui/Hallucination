using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public int m_currentCorridor = -1;
    public Corridor m_currentCorridorInstance;
    public int m_creationLimit = 5;
    public int m_creationCounter = 0;
    public bool m_victoryState = false;
    public int[] m_victoryPattern;
    public int m_nbCorridorVictory, m_corridorCollectionCounter;
    private string m_lineMap = "";
    public Text m_mapTextArea;
    private Color m_beginFadeColor = new Color(1, 1, 1, 1), m_endFadeColor = new Color(1, 1, 1, 0);

    private float duration = 1; // This will be your time in seconds.
    private float smoothness = 0.02f; // This will determine the smoothness of the lerp. Smaller values are smoother. Really it's the time between updates.
    private Transform m_endRoom;

    void Start()
    {
        m_currentCorridorInstance = GameObject.Find("666").GetComponent<Corridor>();
        m_endRoom = GameObject.Find("EndRoom").GetComponent<Transform>();
        m_currentCorridorInstance.m_triggers = m_currentCorridorInstance.GetComponentsInChildren<TriggerCorridor>();
        m_currentCorridorInstance.DisableTriggers();
        m_currentCorridor = m_currentCorridorInstance.GetInstanceID();
        m_victoryPattern = new int[m_nbCorridorVictory];
        for (int i = 0; i < m_nbCorridorVictory; i++)
        {
            int rand10 = (int)Random.Range(0, 4);
            int rand1 = 0;
            switch (rand10)
            {
                case 0:
                    rand1 = (int)Random.Range(0, 2);
                    break;
                case 1:
                    rand1 = (int)Random.Range(0, 2);
                    break;
                case 2:
                    rand1 = (int)Random.Range(0, 3);
                    break;
                case 3:
                    rand1 = 0;
                    break;
                case 4:
                    rand1 = 0;
                    break;
            }
            m_victoryPattern[i] = (rand10*10)+rand1;
        }
    }
    void Update()
    {
        if (Input.GetKeyUp("y"))
        {
            //Debug.Log(ObjectPool.instance._matrix.Count);
            foreach (ObjectPool.GridElement _t in ObjectPool.instance._matrix)
            {
                //Debug.Log(_t.mToString());
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            displayMap();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (m_corridorCollectionCounter == m_nbCorridorVictory)
        {
            TeleportToEnd();
        }
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
    public void displayMap()
    {
        m_lineMap = "| ";
        if (m_corridorCollectionCounter == m_nbCorridorVictory)
        {
            m_lineMap += "WINNER ";
        }
        else
        {
            for (int i = 0; i < m_nbCorridorVictory; i++)
            {
                if (i == m_corridorCollectionCounter)
                {
                    m_lineMap += "->";
                }
                if (m_victoryPattern[i] == 0 || m_victoryPattern[i] == 1) m_lineMap += "0";

                m_lineMap += m_victoryPattern[i] + " ";
            }
        }
        m_lineMap += "|\n\n";

        for (int j = ObjectPool.instance.matrixMaxY; j >= ObjectPool.instance.matrixMinY; j--)
        {
            m_lineMap += "| ";
            for (int i = ObjectPool.instance.matrixMinX; i <= ObjectPool.instance.matrixMaxX; i++)
            {
                m_lineMap += findTypeAtPos(i, j) + " ";
            }
            m_lineMap += "|\n";
        }
        m_mapTextArea.text = m_lineMap;
        if (m_victoryState) m_mapTextArea.text = "YOU FOUND THE ANSWER. TELEPORTING TO THE REWARD ROOM...";
        StartCoroutine("LerpColor", false);
       /* if (m_mapTextArea.color == m_endFadeColor)
        {
            m_mapTextArea.color = m_beginFadeColor;
        }
        else
            m_mapTextArea.color = m_endFadeColor;*/
    }
    public string findTypeAtPos(int x, int y)
    {
        for (int i = 0; i < ObjectPool.instance._matrix.Count; i++)
        {
            if (x == ObjectPool.instance._matrix[i].m_x && y == ObjectPool.instance._matrix[i].m_y)
            {
                if (ObjectPool.instance._matrix[i].m_type == 0 || ObjectPool.instance._matrix[i].m_type == 1)
                    return "0"+ObjectPool.instance._matrix[i].m_type.ToString();
                else 
                    return ObjectPool.instance._matrix[i].m_type.ToString();
            }
        }
        return "--";
    }
    IEnumerator LerpColor(bool finishGame = false)
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.

        while (progress < 1)
        {
            m_mapTextArea.color = Color.Lerp(m_beginFadeColor, m_endFadeColor, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
        if(finishGame)
        {
            yield return new WaitForSeconds(1f);
            this.transform.position = m_endRoom.position + new Vector3(0f, 5f, 3f);
        }
    }
    public void TeleportToEnd ()
    {
        m_corridorCollectionCounter++;
        m_victoryState = true;
        m_mapTextArea.text = "YOU FOUND THE ANSWER. TELEPORTING TO THE REWARD ROOM...";
        StartCoroutine("LerpColor", true);
    }
}
