using UnityEngine;
using System.Collections;

public class TriggerWind : MonoBehaviour {


    GameObject player;
    GameObject particles;
    GameObject camera;
	// Use this for initialization
	void Start () 
    {
        //get the player
        player = GameObject.FindGameObjectWithTag("Player");
        
        //Get its particles and put it to false
        foreach(Transform t in player.transform)
        {
            if(t.name == "Dust Storm")
            {
                particles = t.gameObject;
                break;
            }
        }
        particles.SetActive(false);

        //Get It'sCamera to shake it
        foreach (Transform t in player.transform)
        {
            if (t.name == "FPSCamera")
            {
                camera = t.gameObject;
                //Debug.Log("Cam");
                break;
            }
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            particles.SetActive(true);
            camera.GetComponent<vp_FPCamera>().ShakeSpeed = 0.5f;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            //particles.SetActive(false);
            //camera.GetComponent<vp_FPCamera>().ShakeSpeed = 1.0f; /**/
        }
    }
}
