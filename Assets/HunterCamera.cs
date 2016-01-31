using UnityEngine;
using System.Collections;

public class HunterCamera : MonoBehaviour {

    public Transform target;
    public float distance;
    public float height;
    public float turnSpeed;

    private float angle = 0;
    private Vector3 desiredPos = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        PositionCamera();
        HandleInput();
	}

    void HandleInput()
    {

    }

    void PositionCamera()
    {
 
        angle += Input.GetAxis("CameraHorizontal")*turnSpeed*Time.smoothDeltaTime;
        angle = angle % 360;

        float radians = Mathf.Deg2Rad*angle;
        float x = Mathf.Cos(radians);
        float z = Mathf.Sin(radians);


        desiredPos = target.position + new Vector3(x, 0, z) * distance + Vector3.up * height;

        //Vector3 toTarget = (target.position - this.transform.position).normalized;
        this.transform.position = Vector3.Lerp(this.transform.position,desiredPos,turnSpeed);

        this.transform.LookAt(target);
    }

    void OnGizsmosDraw()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(desiredPos, 1f);
    }
}
