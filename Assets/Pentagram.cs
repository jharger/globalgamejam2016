using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


//Unity OnChange float event
[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
};

//custom class for onChange event, and min/max value
[System.Serializable]
public class AnimationFloat
{
    public string name;
    public FloatEvent parameter; // hook in a public float variable here
    public AnimationCurve animCurve;
    //it will be changed between these min/max values with the audio frequency.
    public float minValue = 0;
    public float maxValue = 1;
    public float animationTime;
}

public class Pentagram : MonoBehaviour {

    public static Pentagram instance;
    public State state = State.Center;
    public enum State { Center, Rise, Dissapear };

    public float moveSpeed;
    public float spinSpeed;
    public float dissapearSpeed;
    public Transform centerPoint; //center of the pentagram
    public Transform risePoint;
    public GameObject HellFire;
    public AudioClip chant;
    public List<AnimationFloat> animations;

    private AudioClip startClip;
    private AudioSource audioSource;

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
        audioSource = this.GetComponent<AudioSource>();
        startClip = audioSource.clip;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AcceptSacrifice(GameObject sacrifice)
    {

        //start playin the chant clip
        audioSource.Stop();
        audioSource.clip = chant;
        audioSource.Play();

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
        RunAnimation(animations[0]); //run the middle flame rise animation
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

        RunAnimation(animations[1]); //run the middle flame fade animation

        StartCoroutine(Dissapear(sacrifice));
       
        Debug.Log("Done Rising");
    }

    IEnumerator Dissapear(GameObject sacrifice)
    {
        HellFire.SetActive(true);
        state = State.Dissapear;
        ThirdPersonPig.instance.SwitchModels(); // switch to the frag material
        Material pigSkin = ThirdPersonPig.instance.fragModel.GetComponent<SkinnedMeshRenderer>().material;
        float influence = 0;
        while (influence < .9)
        {
            influence = Mathf.Lerp(influence, 1, dissapearSpeed * Time.deltaTime);
            //set the influence
            Debug.Log("Influence: " + influence);
            pigSkin.SetFloat("V_FR_Fragmentum", influence);
            yield return null;
        }

        Debug.Log("Done dissaperd");
        HellFire.SetActive(false);


        //stop playin the chant clip
        audioSource.Stop();
        audioSource.clip = startClip;
        audioSource.Play();

        GameController.instance.DisplayWin("Goth Wins!");
        GameController.instance.paused = true;
    }

    public void RunAnimation(AnimationFloat anim)
    {
        StartCoroutine(Run(anim));
    }

    IEnumerator Run(AnimationFloat anim)
    {
        float timer = 0;
        float value = anim.minValue;
        while (timer < anim.animationTime)
        {
            float percent = timer / anim.animationTime;
            value = anim.minValue + (anim.maxValue - anim.minValue) * anim.animCurve.Evaluate(percent);
            anim.parameter.Invoke(value);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
