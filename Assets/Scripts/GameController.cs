using UnityEngine;
using System.Collections;


public class GameController : MonoBehaviour
{

    public static GameController instance;
    public GameObject pig;
    public GameObject hunter;
    public float gameTime;
    public bool paused = false;

    public bool m_PigCaptured = false;




    //singleton logic;
    void OnEnable()
    {
        instance = this;
    }

    void OnDisable()
    {
        instance = null;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!paused)
        {
            gameTime -= Time.fixedDeltaTime;
        }
    }

    public void SetPigCaptured(bool bcaptured)
    {
        m_PigCaptured = bcaptured;

        if (bcaptured) // Pig was captured reset all variables.
        {
            PigWiggleSlider.instance.ShowWithDefaultValues();
        }
        else //Pig was relased Hide slider.
        {
        }
    }

}
