using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// For some reason ThirdPersonUserController wasn't seeing any of our custom classes....
/// Maybe a namespace issue? -Kurt
/// </summary>
public class HunterController : MonoBehaviour {

    public static HunterController instance;
    public float PigGripPower = .1f;
    public Transform pigCarryPoint;
    public float pickupTime;
    public List<AudioClip> grunts;
    public List<AudioClip> quotes;

    private Animator m_Animator;
    private bool m_Dive = false;
    private Transform pigTransform; // a reference to the pig, after you've grabbed it
    private float pickupTimer = 0;


    protected float m_LastTriggerAxis = 0f;


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
        GameController.instance.hunter = this.gameObject;
        m_Animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void FixedUpdate()
    {
        if (!GameController.instance.m_PigCaptured)
        {
            pickupTimer -= Time.fixedDeltaTime;
        }

        //dive!
        if (m_Dive)
        {
            int clipIndex = Random.Range(0,grunts.Count-1);
            AudioManager.instance.PlayClip(grunts[clipIndex]);
            m_Animator.SetTrigger("Dive");
           // Debug.Log("Diving for dat piggy");
        }

        m_Dive = false;
    }

    void HandleInput()
    {
        float trigger = Input.GetAxis("HunterWiggle");
        //Debug.Log(string.Format("Trigger: {0}", Input.GetAxis("HunterWiggle")));
        bool wiggleR = Input.GetKeyDown(KeyCode.E) || (trigger > 0.5f && !(m_LastTriggerAxis > 0.5f));
        bool wiggleL = Input.GetKeyDown(KeyCode.Q) || (trigger < -0.5f && !(m_LastTriggerAxis < -0.5f));
        bool emote = Input.GetButtonDown("Emote");

        m_LastTriggerAxis = trigger;
        if (wiggleR)
        {

            PigWiggleSlider.instance.AddWiggle(PigGripPower );
        }
        if (wiggleL)
        {

            PigWiggleSlider.instance.AddWiggle(-PigGripPower );
        }

        if (!m_Dive)
        {
            m_Dive = Input.GetButtonDown("HunterDive");

        }

        if(emote)
        {
            int clipIndex = Random.Range(0, quotes.Count - 1);
            AudioManager.instance.PlayClip(quotes[clipIndex]);
        }

    }

    //we have 2 piggyGrabTriggers attached to the hands of our hunter.
    //if they trigger the piggy, grab it
    private void OnTriggerEnter(Collider col)
    {
        //if we are diving, and collided with the piggy
        if (col.gameObject.tag == "Piggy" && pickupTimer < 0 && !ThirdPersonPig.instance.isDashing)
        {
            GrabPig(col);


        }

        //if we are carrying the pig, and collide witht he Pentagram!
        if (col.gameObject.tag == "Pentagram" && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Carry"))
        {
            ReleasePig();
            Pentagram.instance.AcceptSacrifice(GameController.instance.pig);
           // ThirdPersonPig.instance.enabled = false; //disable piggy controls
        }
    }

    private void GrabPig(Collider col)
    {
        Debug.Log("Grabbed Piggy!");
        pickupTimer = pickupTime; //reset the pickup timer

        GameController.instance.SetPigCaptured(true);
        //parent the piggy to our right hand.
        pigTransform = col.transform;
        //make the rigidbody kinematic
        col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        col.isTrigger = true;  // make the collider a trigger

        col.transform.position = pigCarryPoint.position;
        col.transform.rotation = pigCarryPoint.rotation;
        col.transform.SetParent(pigCarryPoint);
        //start the carry animation
        m_Animator.SetBool("Carry", true);

        //start the wiggle animation
        ThirdPersonPig.instance.SetWiggle(true);

    }


    public void ReleasePig()
    {
        //end the wiggle animation
        ThirdPersonPig.instance.SetWiggle(false);
        GameController.instance.SetPigCaptured(false);

        //make the rigidbody non-kinematic
        pigTransform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        pigTransform.GetComponent<CapsuleCollider>().isTrigger = false;  // turn the collider back on

        pigTransform.rotation = this.transform.rotation;

        //parent the piggy to our right hand.
        pigTransform.SetParent(null);
        //start the carry animation
        m_Animator.SetBool("Carry", false);

        //move the pig wiggle slider back
        PigWiggleSlider.instance.tween.MoveBack();
    }
}
