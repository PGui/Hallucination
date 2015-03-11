using UnityEngine;
using System.Collections;

public class MovingAnimals : MonoBehaviour {
    public string axe = "Z";
    public float speed = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (axe == "Z")
        {
            transform.Translate(0, 0, Time.deltaTime * speed); // move forward
            transform.Rotate(0, Time.deltaTime * 15f, 0); // turn a little
        }
        else if (axe == "X")
        {
            transform.Translate(Time.deltaTime * speed, 0, 0); // move forward
            transform.Rotate(0, Time.deltaTime * 40f, 0); // turn a little
        }
        else if (axe == "turtle")
        {
            transform.eulerAngles += new Vector3(0f, Time.deltaTime * speed, 0f);
        }

	}
    private IEnumerator MoveForward()
    {
        speed = 20f;
        yield return new WaitForSeconds(0.05f);
        speed = 50f;
        yield return new WaitForSeconds(0.3f);
        speed = 20f;
        yield return new WaitForSeconds(0.1f);
        speed = 10f;
        yield return new WaitForSeconds(0.166f);
    }
}
