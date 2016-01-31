using UnityEngine;
using System.Collections;

public class RealTimeLightFlicker : MonoBehaviour {

    AlloyAreaLight myLight;

    public float minIntensity = 1.0f;
    public float maxIntensity = 1.0f;
    public float changeSpeed = 1.0f;

    float perlinOffset = 0.0f;



	// Use this for initialization
	void Start () {
        myLight = GetComponent<AlloyAreaLight>();
        perlinOffset = Random.Range(0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
        myLight.Intensity = (Mathf.PerlinNoise(((Time.time + perlinOffset) * changeSpeed), 0.0f) * (maxIntensity - minIntensity)) + minIntensity;

	}
}
