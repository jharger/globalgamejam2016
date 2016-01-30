using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PigWiggleSlider : MonoBehaviour {

    public static PigWiggleSlider instance;
    public float lerpSpeed = 1; // how fast does the slider lerp based on inputs
    Slider slider;

    //singleton logic
    void OnEnable()
    {
        instance = this;
    }
    void OnDisable()
    {
        instance = null;
    }

    void Awake()
    {
        slider = this.GetComponent<Slider>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //add or subtract a value to the wiggle slider
    public void AddWiggle(float wiggleValue)
    {
        float desiredValue = slider.value + wiggleValue;
        slider.value = Mathf.Lerp(slider.value,desiredValue,lerpSpeed);
    }
}
