using UnityEngine;
using System.Collections;

public class CameraLabyControl : MonoBehaviour {

    public Animator myAnim;
    public Camera secondCam;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            myAnim.SetBool("rushThrough", true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            myAnim.SetBool("rushThrough", false);
            myAnim.SetBool("rotate", true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            secondCam.camera.rect = new Rect(0,0,1,1); 
        }
	}
}
