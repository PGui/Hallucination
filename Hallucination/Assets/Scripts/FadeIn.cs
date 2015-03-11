using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour {

    private float time = 0.0f;
    public float duration = 5.0f;

	// Use this for initialization
	void Start () {
        guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        //guiTexture.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (time < duration)
        {
            guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, time / duration);
            time += Time.deltaTime;
        }
	}
}
