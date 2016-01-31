using System;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        public Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_Dive = false;
        private Animator m_Animator;

        private void Start()
        {

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
                m_Jump = CrossPlatformInputManager.GetButtonDown("HunterJump");
            }
            if(!m_Dive)
            {
                m_Dive = Input.GetButtonDown("HunterDive");
        
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("HunterHorizontal");
            float v = CrossPlatformInputManager.GetAxis("HunterVertical");
            Debug.Log("Input:(" + h + "," + v + ")");
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
