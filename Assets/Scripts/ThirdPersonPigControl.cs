using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonPig))]
    public class ThirdPersonPigControl : MonoBehaviour
    {
        private ThirdPersonPig m_Character; // A reference to the ThirdPersonCharacter on the object
        public Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        
        private void Start()
        {


            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonPig>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("PigDash");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("PigHorizontal");
            float v = CrossPlatformInputManager.GetAxis("PigVertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // we use world-relative directions in the case of no main camera
            m_Move = v*Vector3.forward + h*Vector3.right;

#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}