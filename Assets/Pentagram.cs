using UnityEngine;
using System.Collections;

public class Pentagram : MonoBehaviour {

    public static Pentagram instance;
    public State state = State.Center;
    public enum State { Center, Rise, Dissapear };

    public float moveSpeed;
    public float spinSpeed;
    public Transform centerPoint; //center of the pentagram
    public Transform risePoint;

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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AcceptSacrifice(GameObject sacrifice)
    {
        var rigidbody = sacrifice.GetComponent<Rigidbody>();
        if(rigidbody)
        {
            rigidbody.detectCollisions = false;
            rigidbody.isKinematic = true;
        }
        StopAllCoroutines();
        StartCoroutine(MoveToCenter(sacrifice));
    }

    //sacrifice the pig!
    //move it to the center
    //then move it up and spin it.
    //finally make it dissapear.
    IEnumerator MoveToCenter(GameObject sacrifice)
    {
        Debug.Log("start moving to Center");
        state = State.Center;
        float distance = Vector3.Distance(sacrifice.transform.position, centerPoint.position);
        while(distance > .1f)
        {
            Debug.Log("distance: " + distance);
            sacrifice.transform.position = Vector3.MoveTowards(sacrifice.transform.position, centerPoint.position, moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(sacrifice.transform.position, centerPoint.position);
            yield return null;
        }

        //when done
        StartCoroutine(Rise(sacrifice));
    }

    IEnumerator Rise(GameObject sacrifice)
    {
        Debug.Log("Rising");
        state = State.Rise;
        float distance = Vector3.Distance(sacrifice.transform.position, risePoint.position);
        float angle = 0;
        while (distance > .1f)
        {
            Debug.Log("distance: " + distance);
            sacrifice.transform.position = Vector3.MoveTowards(sacrifice.transform.position, risePoint.position, moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(sacrifice.transform.position, risePoint.position);
            sacrifice.transform.Rotate(Vector3.up, angle);
            angle += spinSpeed; // rotate by more and more
            //TODO: play pentagram effects!

            yield return null;
        }


        //TODO: move!
        state = State.Dissapear;
        Debug.Log("Done Rising");
    }

    IEnumerator Dissapear(GameObject sacrifice)
    {
        while (true)
        {
            yield return null;
        }
    }
}
