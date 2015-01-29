using UnityEngine;
using System.Collections;

public class TriggerAberrationChrom : MonoBehaviour {
   // sfgfgfdg
    public GameObject Camera;
    public float multipleSin = 30.0f;
    public float multipleTime = 10.0f;
    Vignetting scriptAberrationChromatique;

    float time = 0.0f;

    bool needEffect = false;

	// Use this for initialization
	void Start () {
        scriptAberrationChromatique = Camera.GetComponent<Vignetting>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (needEffect)
        {
            float chromValue = Mathf.Sin(time * multipleTime) * multipleSin;

            scriptAberrationChromatique.chromaticAberration = chromValue;

            time += Time.deltaTime;
        }
	}

    void OnTriggerStay(Collider coll)
    {
        

       
    }

    void OnTriggerEnter(Collider coll)
    {
        needEffect = true;
    }

    void OnTriggerExit(Collider coll)
    {
        float chromValue = Mathf.Sin(time * multipleSin);
        time = 0.0f;
    }
}
