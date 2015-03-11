using UnityEngine;
using System.Collections;

public class triggerCloseDoor : MonoBehaviour {

    public Transform door;

    public Transform posDoorClose;

    public float closeDuration = 2.0f;

    float time = 0f;

    bool closeDoor = false;

    Vector3 startPos;
	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {

        if (closeDoor)
        {
           door.position = Vector3.Lerp(startPos, posDoorClose.position, time / closeDuration);

            time += Time.deltaTime;
            if (time >= closeDuration)
            {
                closeDoor = false;
                Application.LoadLevel("Scene3FX");
            }

        }

        
	}

    void OnTriggerEnter(Collider coll)
    {
        startPos = door.position;
        door.GetComponent<MeshRenderer>().enabled = true;
        closeDoor = true;
    }
}
