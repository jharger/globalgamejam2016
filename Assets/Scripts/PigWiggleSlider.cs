using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PigWiggleSlider : MonoBehaviour {
    public static PigWiggleSlider instance;
    public float lerpSpeed = 1; // how fast does the slider lerp based on inputs
    public UI_Tween tween;
    public UI_Tween helpTween;
    public Sprite rightHelp;
    public Sprite leftHelp;

    private float CurrentValue;
    Slider slider;
    Image help;

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
        help = helpTween.GetComponent<Image>();
        slider = this.GetComponent<Slider>();
    }

	// Use this for initialization
	void Start () {
        slider.value = 0;
	}
    public void ShowWithDefaultValues()
    {
        Debug.Log("Showing Slider");
        tween.Move();
        slider.value = 0f;
        CurrentValue = 0f;
        StartCoroutine(AnimateHelp());
    }

    IEnumerator AnimateHelp()
    {
        helpTween.Move();
        for (int i = 0; i < 30; i++)
        {
            if(i % 2 == 0)
            {
                help.sprite = leftHelp;
            }
            else
            {
                help.sprite = rightHelp;
            }
            yield return new WaitForSeconds(0.1f);
        }
        helpTween.MoveBack();
    }
	// Update is called once per frame
	void Update () {

        slider.value = Mathf.Lerp(slider.value, CurrentValue, lerpSpeed * Time.deltaTime);
        //detect if we drop the pig!
        if (tween.moved && Mathf.Abs(slider.value) > .8f)
        {

            HunterController.instance.ReleasePig();
            //hide the slider
            tween.MoveBack();
            GameController.instance.SetPigCaptured(false);

            
        }

	}

    //add or subtract a value to the wiggle slider
    public void AddWiggle(float wiggleValue)
    {
        CurrentValue = CurrentValue + wiggleValue;        
    }
}
