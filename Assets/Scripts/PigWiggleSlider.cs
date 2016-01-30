using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PigWiggleSlider : MonoBehaviour {

    public static PigWiggleSlider instance;
    public float lerpSpeed = 1; // how fast does the slider lerp based on inputs
    private float CurrentValue;
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
        CurrentValue = slider.value;
	}
    public void ShowWithDefaultValues()
    {
        Debug.Log("Showing Slider");
        slider.value = 0f;
        this.gameObject.SetActive(true);
    }
	// Update is called once per frame
	void Update () {

        slider.value = Mathf.Lerp(slider.value, CurrentValue, lerpSpeed * Time.deltaTime);
	}

    //add or subtract a value to the wiggle slider
    public void AddWiggle(float wiggleValue)
    {
        CurrentValue = CurrentValue + wiggleValue;        
    }
}
