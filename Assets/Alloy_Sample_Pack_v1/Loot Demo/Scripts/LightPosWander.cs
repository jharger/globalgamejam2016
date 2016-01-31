using UnityEngine;
using System.Collections;

public class LightPosWander : MonoBehaviour {

    public float displacement;
    public float speed;

    Vector3 startpos;



	// Use this for initialization
	void Start () {
        startpos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        float xdisp = Mathf.PerlinNoise(startpos.x, Time.time * speed);
        float ydisp = Mathf.PerlinNoise(startpos.y, Time.time * speed);
        float zdisp = Mathf.PerlinNoise(startpos.z, Time.time * speed);

        Vector3 vdisp = new Vector3(xdisp * displacement, ydisp * displacement, zdisp * displacement);

        transform.localPosition = startpos + vdisp;
	}
}
