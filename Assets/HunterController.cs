using UnityEngine;
using System.Collections;

/// <summary>
/// For some reason ThirdPersonUserController wasn't seeing any of our custom classes....
/// Maybe a namespace issue? -Kurt
/// </summary>
public class HunterController : MonoBehaviour {

    public static HunterController instance;
    public float PigGripPower = .1f;
    private Animator m_Animator;
    protected float m_LastTriggerAxis = 0f;
    private Transform pigTransform; // a reference to the pig, after you've grabbed it
    public Transform pigCarryPoint;


    //singleton logic
    void OnEnable()
    {
        instance = this;
    }
    void OnDisable()
    {
        instance = null;
    }

	// Use this for initialization
	void Start () {
        m_Animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void HandleInput()
    {
        float trigger = Input.GetAxis("Trigger");
        Debug.Log(string.Format("Trigger: {0}", Input.GetAxis("Trigger")));
        bool wiggleR = Input.GetKeyDown(KeyCode.E) || (trigger < -0.5f && !(m_LastTriggerAxis < -0.5f)); 
        bool wiggleL = Input.GetKeyDown(KeyCode.Q) || (trigger > 0.5f && !(m_LastTriggerAxis > 0.5f));
        m_LastTriggerAxis = trigger;
        if (wiggleR)
        {

            PigWiggleSlider.instance.AddWiggle(PigGripPower );
        }
        if (wiggleL)
        {

            PigWiggleSlider.instance.AddWiggle(-PigGripPower );
        }
    }

    //we have 2 piggyGrabTriggers attached to the hands of our hunter.
    //if they trigger the piggy, grab it
    private void OnTriggerEnter(Collider col)
    {
        //if we are diving, and collided with the piggy
        if (col.gameObject.tag == "Piggy" && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Dive"))
        {
            Debug.Log("Grabbed Piggy!");

            GameController.instance.SetPigCaptured(true);
            //parent the piggy to our right hand.
            pigTransform = col.transform;
            //make the rigidbody kinematic
            col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            col.isTrigger = true;  // make the collider a trigger

            col.transform.position = pigCarryPoint.position;
            col.transform.SetParent(pigCarryPoint);
            //start the carry animation
            m_Animator.SetBool("Carry", true);
        }

        //if we are carrying the pig, and collide witht he Pentagram!
        if (col.gameObject.tag == "Pentagram" && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Carry"))
        {
            ReleasePig();
            
        }
    }


    public void ReleasePig()
    {
        Debug.Log("Sacrifice Accepted!");
        //make the rigidbody non-kinematic
        pigTransform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        pigTransform.GetComponent<BoxCollider>().isTrigger = false;  // turn the collider back on
        //parent the piggy to our right hand.
        pigTransform.SetParent(null);
        //start the carry animation
        m_Animator.SetBool("Carry", false);

        //move the pig wiggle slider back
        PigWiggleSlider.instance.tween.MoveBack();
    }
}
