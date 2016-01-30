using UnityEngine;
using System.Collections;

/// <summary>
/// For some reason ThirdPersonUserController wasn't seeing any of our custom classes....
/// Maybe a namespace issue? -Kurt
/// </summary>
public class HunterController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void HandleInput()
    {
        bool wiggleR = Input.GetKeyDown(KeyCode.E); //TODO: add joystick buttons
        bool wiggleL = Input.GetKeyDown(KeyCode.Q);

        if (wiggleR)
        {
            PigWiggleSlider.instance.AddWiggle(.1f);
        }
        if (wiggleL)
        {

            PigWiggleSlider.instance.AddWiggle(-.1f);
        }
    }
}
