using UnityEngine;
using System.Collections;

public class Generation : MonoBehaviour {


    public GameObject[] corridors;
    int nbCorridorsToGenerate = 10;
    int length;

    ArrayList corridorTriggers;
	// Use this for initialization
	void Start () 
    {
        length = corridors.Length;
	    //Place the first corridor
        GameObject.Instantiate(getRandomCorridor(), Vector3.zero, Quaternion.identity);

        corridorTriggers = new ArrayList(4);

        for (int i = 0; i < nbCorridorsToGenerate; ++i)
        {
            corridorTriggers = getCorridorTrigger(getRandomCorridor());
            

        }
	}

    // Update is called once per frame
    void Update()
    {
	
	}

    GameObject getRandomCorridor()
    {
        return corridors[Random.Range(0, length)];
    }

    bool hasUpEntrance(GameObject corridor)
    {
        foreach (Transform t in corridor.transform)
        {
            if (t.tag == "Up")
            {
                return true;
            }
        }
        return false;
    }

    ArrayList getCorridorTrigger(GameObject corridor)
    {
        corridorTriggers.Clear();
        foreach(Transform t in corridor.transform)
        {
            corridorTriggers.Add(t.gameObject);
        }
        return corridorTriggers;
        
    }

}
