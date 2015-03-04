using UnityEngine;
using System.Collections;

public class CameraLabyControl : MonoBehaviour {

    public Animator myAnim;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            myAnim.SetBool("rushThrough", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            myAnim.SetBool("rushThrough", false);
            myAnim.SetBool("rotate", true);
        }
	}
}
