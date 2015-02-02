using UnityEngine;
using System.Collections;

public class TriggerGrow : MonoBehaviour {

    public Transform objectToGrow;
    public GameObject cam;
    public float shakeValue = 2.0f;
    private vp_FPCamera scriptcam;
    float previousShakeValue = 1000.0f;
	// Use this for initialization
    public float GrowingTime = 5.0f;

    public GameObject AudioSourceToPlay;

    bool needGrow = false;
    public Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 endScale = new Vector3(10.0f, 10.0f, 10.0f);

    public Vector3 startPosition = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 endPosition = new Vector3(1.0f, 1.0f, 1.0f);

    float time = 0.0f;



	void Start () 
    {
        scriptcam = cam.GetComponent<vp_FPCamera>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (needGrow)
        {
            scriptcam.ShakeSpeed = shakeValue;

            objectToGrow.localScale = Vector3.Lerp(startScale, endScale, Ratio(time / GrowingTime));
            objectToGrow.position = Vector3.Lerp(startPosition, endPosition, Ratio(time / GrowingTime));
            time += Time.deltaTime;
            if (time >= GrowingTime)
            {
                needGrow = false;
                scriptcam.ShakeSpeed = previousShakeValue;
            }
        }
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            needGrow = true;
            previousShakeValue = scriptcam.ShakeSpeed;
            AudioSourceToPlay.GetComponent<AudioSource>().Play();
        }
    }

    private float Ratio(float t)
    {
        return t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
    }
}
