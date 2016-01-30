using System;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {

        public Transform pigCarryPoint;
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_Dive = false;
        private Animator m_Animator;
        private Transform pigTransform; // a reference to the pig, after you've grabbed it

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            m_Animator = this.GetComponent<Animator>();
        }


        private void Update()
        {
            bool wiggleR = Input.GetKeyDown(KeyCode.E); //TODO: add joystick buttons
            bool wiggleL = Input.GetKeyDown(KeyCode.Q);

            if(wiggleR)
            {
                
               // PigWiggleSlider.instance.AddWiggle(.1f);
            }


            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            if(!m_Dive)
            {
                m_Dive = Input.GetButtonDown("HunterDive");
        
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
                //parent the piggy to our right hand.
                pigTransform = col.transform;
                //make the rigidbody kinematic
                col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                col.isTrigger = true;  // make the collider a trigger

                col.transform.position = pigCarryPoint.position;
                col.transform.SetParent(pigCarryPoint);
                //start the carry animation
                m_Animator.SetBool("Carry",true);
            }

            //if we are carrying the pig, and collide witht he Pentagram!
            if (col.gameObject.tag == "Pentagram" && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Carry"))
            {
                Debug.Log("Sacrifice Accepted!");
                //make the rigidbody non-kinematic
                pigTransform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                pigTransform.GetComponent<BoxCollider>().isTrigger = false;  // turn the collider back on
                //parent the piggy to our right hand.
                pigTransform.SetParent(null);
                //start the carry animation
                m_Animator.SetBool("Carry", false);
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            //dive!
            if(m_Dive)
            {
                m_Animator.SetTrigger("Dive");
                Debug.Log("Diving for dat piggy");
            }

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
            m_Dive = false;
        }
    }
}
