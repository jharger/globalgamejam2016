using UnityEngine;
using System.Collections;

public class ColumnsLightMovert : MonoBehaviour {

    public Transform RotPivot;
    public Transform Light;

    public float rotSpeed;
    public float oscSpeed;
    public float oscMag;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        RotPivot.localEulerAngles += (new Vector3(0.0f, 1.0f * rotSpeed, 0.0f) * Time.deltaTime);

        Light.localPosition = new Vector3(0.0f, Mathf.Sin(Time.time * oscSpeed) * oscMag, 0.0f);
	}
}
