using UnityEngine;
using System.Collections;

public class TriggerGrow : MonoBehaviour {

    public Transform objectToGrow;
	// Use this for initialization
    public float GrowingTime = 5.0f;

    bool needGrow = false;
    public Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 endScale = new Vector3(10.0f, 10.0f, 10.0f);

    public Vector3 startPosition = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 endPosition = new Vector3(1.0f, 1.0f, 1.0f);

    float time = 0.0f;

	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (needGrow)
        {
            float t = time / GrowingTime;
            float ratio = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
            objectToGrow.localScale = Vector3.Lerp(startScale, endScale, ratio);
            objectToGrow.position = Vector3.Lerp(startPosition, endPosition, ratio);
            time += Time.deltaTime;
            if (time >= GrowingTime)
            {
                needGrow = false;
            }
        }
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            needGrow = true;
        }
    }
}
