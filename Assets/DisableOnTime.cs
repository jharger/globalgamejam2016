using UnityEngine;
using System.Collections;

public class DisableOnTime : MonoBehaviour {

    public float disableTime = 5;
    private float disableTimer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        disableTimer += Time.deltaTime;
        if(disableTimer > disableTime)
        {
            this.gameObject.SetActive(false);
        }
	}
}
