using UnityEngine;
using System.Collections;

public class TurtleAnims : MonoBehaviour {


    private Animator myAnim = null;
    private Animation myAnimation = null;
    public float GrowingTime = 5.0f;
    float time = 0.0f;
    private Vector3 movePosition;
    private bool m_canMove = false, m_canTurnHead = false;
    private float speedCoeff=0.1f;
    private int appearHashId = Animator.StringToHash("isAppear"), idleHashId = Animator.StringToHash("isIdle");
    public ParticleSystem[] m_sandEmitters, m_leafEmitters;
    public bool appearAnimated = false, flyinTurtle = false;
    private float delay;

    // Use this for initialization
    void Start()
    {
        myAnim = GetComponent<Animator>();
        m_sandEmitters = gameObject.GetComponentsInChildren<ParticleSystem>();
      //  myAnim.SetBool(appearHashId, true);
        if (appearAnimated)
        {
            myAnim.speed = 0;
        }
        else
        {
            animIdle();
            StartCoroutine("flyingTurtle");
        }
    }
    IEnumerator flyingTurtle()
    {
        delay = Random.Range(1f, 3f);
        yield return new WaitForSeconds(delay);
        animWalk();
        StartCoroutine("MoveForward");
    }
    IEnumerator appearStop() {        
        yield return new WaitForSeconds(5f);
       // transform.localPosition = new Vector3(140f, -120f, -85f);
        //Debug.Log("repositionne");
       // yield return new WaitForSeconds(2.16f);
            myAnim.SetBool("isIdle", true);
            myAnim.SetBool("isDown", false);
            myAnim.SetBool("isWalk", false);
            myAnim.SetBool("isUp", false);
            myAnim.SetBool("isUpIdle", false);
            myAnim.SetBool("isUpTurnHead", false);
    }
    // Update is called once per frame
    void Update()
    {
        if (!flyinTurtle)
        {
            if (Input.GetKey("i"))
            {
                animUp();
                // StopCoroutine("nextMove");
            }
            else if (Input.GetKey("o"))
            {
                animWalk();
                //movePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 5);
                //  StartCoroutine("nextMove");
            }
            else if (Input.GetKey("j"))
            {
                animDown();
                //  StopCoroutine("nextMove");
            }
            else if (Input.GetKey("k"))
            {
                animIdle();
                // StopCoroutine("nextMove");
            }
            else if (Input.GetKey("l"))
            {
                animUpIdle();
                //   StopCoroutine("nextMove");
            }
            else if (Input.GetKey("m"))
            {
                animUpTurnHead();
                //  StopCoroutine("nextMove");
            }
            if (Input.GetKeyUp("m"))
            {
                animUpIdle();
                //   StopCoroutine("nextMove");
            }
        }
        if (myAnim.GetBool("isWalk") == true && m_canMove/*myAnim.GetAnimatorTransitionInfo(0).ToString /*.animation["Walk"].time > 0.08f && myAnim.animation["Walk"].time < 0.28f*/)
        {
           //this.transform.position = Vector3.Lerp(this.transform.position, movePosition, Time.deltaTime*0.2f);
           // time += Time.deltaTime;
            this.transform.parent.transform.position += new Vector3(speedCoeff * Time.deltaTime, speedCoeff*0.5f * Time.deltaTime,0f);
        }
       // Debug.Log(m_canTurnHead +" - "+ myAnim.GetBool("isUp"));
        if (m_canTurnHead && (myAnim.GetBool("isUp") || myAnim.GetBool("isUpIdle")))
        {
            StartCoroutine("UpTurnHead");
        }
        /*if (myAnim.GetBool("isWalk") == true && myAnim.animation["Walk"].time > 0.28f && myAnim.animation["Walk"].time < 0.29f)
        {
            movePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 5f);
        }*/
    }
    IEnumerator nextMove()
    {
        yield return new WaitForSeconds(0.265f);
        m_canMove = true;
        yield return new WaitForSeconds(0.666f);
        m_canMove = false;
        movePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 5f);
        yield return new WaitForSeconds(1.966666666667f);
        StartCoroutine("nextMove");
    }
    private float Ratio(float t)
    {
        return t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
    }
    private IEnumerator MoveForward()
    {
        m_canMove = true;
        speedCoeff = 40f;
        yield return new WaitForSeconds(0.1f);
        speedCoeff = 100f;
        yield return new WaitForSeconds(0.3f);
        speedCoeff = 40f;
        yield return new WaitForSeconds(0.1f);
        speedCoeff = 20f;
       // Hashtable ht = iTween.Hash("from", 0.1f, "to", 0f, "time", .166f, "onupdate", "changeMotionBlur");

        //make iTween call:
      //  iTween.ValueTo(gameObject, ht);
        yield return new WaitForSeconds(0.166f);
        //iTween.MoveTo(this.gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z - .3f), 3f);
        /*Hashtable options = iTween.Hash(
            "position", new Vector3(transform.position.x, transform.position.y, transform.position.z - .3f),
	        "time", 0.8f,
	        "easetype", iTween.EaseType.easeInOutCubic
	        );
        iTween.MoveTo(this.gameObject, options);*/
        yield return new WaitForSeconds(0);
       // m_canMove = false;
    }
    //since our ValueTo() iscalculating floats the "onupdate" callback will expect a float as well:
    void changeMotionBlur(float newValue){
        //apply the value of newValue:
        //print(newValue);
        speedCoeff = newValue;
    }
    public void launchAnimation()
    {
        myAnim.speed = 1;
        StartCoroutine("appearStop");
    }
    void endAppearAnim()
    {
        Debug.Log("Yeah");
       /* myAnim.SetBool("isUp", true);
        myAnim.SetBool("isIdle", false);
        myAnim.SetBool("isDown", false);
        myAnim.SetBool("isWalk", false);
        myAnim.SetBool("isUpIdle", false);
        myAnim.SetBool("isUpTurnHead", false);*/
    }
    public void startSand()
    {
        for (int i = 0; i < m_sandEmitters.Length; i++)
        {
            if (m_sandEmitters[i].name != "LeftLeaf" && m_sandEmitters[i].name != "RightLeaf")
            {
                m_sandEmitters[i].gameObject.particleSystem.Play();
            }
        }
    }
    public void stopSand()
    {
        for (int i = 0; i < m_sandEmitters.Length; i++)
        {
            if (m_sandEmitters[i].name != "LeftLeaf" && m_sandEmitters[i].name != "RightLeaf" && m_sandEmitters[i].name != "Smoke")
            {
                m_sandEmitters[i].gameObject.particleSystem.Stop();
            }
        }
    }
    public void startLeaf()
    {
        for (int i = 0; i < m_sandEmitters.Length; i++)
        {
            if (m_sandEmitters[i].name == "LeftLeaf" || m_sandEmitters[i].name == "RightLeaf")
            {
                m_sandEmitters[i].gameObject.particleSystem.Play();
            }
        }
    }
    public void stopLeaf()
    {
        for (int i = 0; i < m_sandEmitters.Length; i++)
        {
            if (m_sandEmitters[i].name == "LeftLeaf" || m_sandEmitters[i].name == "RightLeaf" || m_sandEmitters[i].name == "Smoke")
            {
                m_sandEmitters[i].gameObject.particleSystem.Stop();
            }
        }
    }
    public void animIdle()
    {
        myAnim.SetBool("isIdle", true);
        myAnim.SetBool("isDown", false);
        myAnim.SetBool("isWalk", false);
        myAnim.SetBool("isUp", false);
        myAnim.SetBool("isUpIdle", false);
        myAnim.SetBool("isUpTurnHead", false);
    }
    public void animUp()
    {
        myAnim.SetBool("isUp", true);
        myAnim.SetBool("isIdle", false);
        myAnim.SetBool("isDown", false);
        myAnim.SetBool("isWalk", false);
        myAnim.SetBool("isUpIdle", false);
        myAnim.SetBool("isUpTurnHead", false);
        m_canTurnHead = true;
    }

    IEnumerator UpTurnHead()
    {
        m_canTurnHead = false;
        Debug.Log("UpTurnHead");
        delay = Random.Range(3f, 8f);
        yield return new WaitForSeconds(delay);
        animUpTurnHead();
        yield return new WaitForSeconds(1f);
        animUpIdle();
        yield return new WaitForSeconds(2f);
        m_canTurnHead = true;
    }
    public void animUpIdle()
    {
        myAnim.SetBool("isIdle", false);
        myAnim.SetBool("isDown", false);
        myAnim.SetBool("isWalk", false);
        myAnim.SetBool("isUp", false);
        myAnim.SetBool("isUpIdle", true);
        myAnim.SetBool("isUpTurnHead", false);
    }
    public void animUpTurnHead()
    {
        myAnim.SetBool("isIdle", false);
        myAnim.SetBool("isDown", false);
        myAnim.SetBool("isWalk", false);
        myAnim.SetBool("isUp", false);
        myAnim.SetBool("isUpIdle", false);
        myAnim.SetBool("isUpTurnHead", true);
    }
    public void animDown()
    {
        myAnim.SetBool("isDown", true);
        myAnim.SetBool("isIdle", false);
        myAnim.SetBool("isWalk", false);
        myAnim.SetBool("isUp", false);
        myAnim.SetBool("isUpIdle", false);
        myAnim.SetBool("isUpTurnHead", false);
    }
    public void animWalk()
    {
        myAnim.SetBool("isWalk", true);
        myAnim.SetBool("isIdle", false);
        myAnim.SetBool("isDown", false);
        myAnim.SetBool("isUp", false);
        myAnim.SetBool("isUpIdle", false);
        myAnim.SetBool("isUpTurnHead", false);
    }
}
