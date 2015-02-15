using UnityEngine;
using System.Collections;

public class TurtleAnims : MonoBehaviour {


    private Animator myAnim = null;
    public float GrowingTime = 5.0f;
    float time = 0.0f;
    private Vector3 movePosition;
    private bool m_canMove = false;
    private float speedCoeff=0.1f;
    // Use this for initialization
    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("i"))
        {
            myAnim.SetBool("isUp", true);
            myAnim.SetBool("isIdle", false);
            myAnim.SetBool("isDown", false);
            myAnim.SetBool("isWalk", false);
            myAnim.SetBool("isUpIdle", false);
            myAnim.SetBool("isUpTurnHead", false);
           // StopCoroutine("nextMove");
        }
        else if (Input.GetKey("o"))
        {
            myAnim.SetBool("isWalk", true);
            myAnim.SetBool("isIdle", false);
            myAnim.SetBool("isDown", false);
            myAnim.SetBool("isUp", false);
            myAnim.SetBool("isUpIdle", false);
            myAnim.SetBool("isUpTurnHead", false);
            movePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z-5f);
          //  StartCoroutine("nextMove");
        }
        else if (Input.GetKey("j"))
        {
            myAnim.SetBool("isDown", true);
            myAnim.SetBool("isIdle", false);
            myAnim.SetBool("isWalk", false);
            myAnim.SetBool("isUp", false);
            myAnim.SetBool("isUpIdle", false);
            myAnim.SetBool("isUpTurnHead", false);
          //  StopCoroutine("nextMove");
        }
        else if (Input.GetKey("k"))
        {
            myAnim.SetBool("isIdle", true);
            myAnim.SetBool("isDown", false);
            myAnim.SetBool("isWalk", false);
            myAnim.SetBool("isUp", false);
            myAnim.SetBool("isUpIdle", false);
            myAnim.SetBool("isUpTurnHead", false);
           // StopCoroutine("nextMove");
        }
        else if (Input.GetKey("l"))
        {
            myAnim.SetBool("isIdle", false);
            myAnim.SetBool("isDown", false);
            myAnim.SetBool("isWalk", false);
            myAnim.SetBool("isUp", false);
            myAnim.SetBool("isUpIdle", true);
            myAnim.SetBool("isUpTurnHead", false);
         //   StopCoroutine("nextMove");
        }
        else if (Input.GetKey("m"))
        {
            myAnim.SetBool("isIdle", false);
            myAnim.SetBool("isDown", false);
            myAnim.SetBool("isWalk", false);
            myAnim.SetBool("isUp", false);
            myAnim.SetBool("isUpIdle", false);
            myAnim.SetBool("isUpTurnHead", true);
          //  StopCoroutine("nextMove");
        }
        if (Input.GetKeyUp("m"))
        {
            myAnim.SetBool("isUpIdle", true);
            myAnim.SetBool("isUpTurnHead", false);
         //   StopCoroutine("nextMove");
        }
        if (myAnim.GetBool("isWalk") == true && m_canMove/*myAnim.GetAnimatorTransitionInfo(0).ToString /*.animation["Walk"].time > 0.08f && myAnim.animation["Walk"].time < 0.28f*/)
        {
           //this.transform.position = Vector3.Lerp(this.transform.position, movePosition, Time.deltaTime*0.2f);
           // time += Time.deltaTime;
            this.transform.position += new Vector3(0f, 0f, - speedCoeff * Time.deltaTime);
        }
        /*if (myAnim.GetBool("isWalk") == true && myAnim.animation["Walk"].time > 0.28f && myAnim.animation["Walk"].time < 0.29f)
        {
            movePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 5f);
        }*/
    }
   /* IEnumerator nextMove()
    {
        yield return new WaitForSeconds(0.265f);
        m_canMove = true;
        yield return new WaitForSeconds(0.666f);
        m_canMove = false;
        movePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 5f);
        yield return new WaitForSeconds(1.966666666667f);
        StartCoroutine("nextMove");
    }*/
    private float Ratio(float t)
    {
        return t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
    }
    private IEnumerator MoveForward()
    {
        m_canMove = true;
        speedCoeff = 0.2f;
        yield return new WaitForSeconds(0.1f);
        speedCoeff = 0.4f;
        yield return new WaitForSeconds(0.3f);
        speedCoeff = 0.2f;
        yield return new WaitForSeconds(0.1f);
        speedCoeff = 0.1f;
        Hashtable ht = iTween.Hash("from", 0.1f, "to", 0f, "time", .166f, "onupdate", "changeMotionBlur");

        //make iTween call:
        iTween.ValueTo(gameObject, ht);
        yield return new WaitForSeconds(0.166f);
        //iTween.MoveTo(this.gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z - .3f), 3f);
        /*Hashtable options = iTween.Hash(
            "position", new Vector3(transform.position.x, transform.position.y, transform.position.z - .3f),
	        "time", 0.8f,
	        "easetype", iTween.EaseType.easeInOutCubic
	        );
        iTween.MoveTo(this.gameObject, options);*/
        yield return new WaitForSeconds(0);
        m_canMove = false;
    }
//since our ValueTo() iscalculating floats the "onupdate" callback will expect a float as well:
void changeMotionBlur(float newValue){
//apply the value of newValue:
//print(newValue);
speedCoeff = newValue;
}
}
