using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameController : MonoBehaviour
{

    public static GameController instance;
    public GameObject pig;
    public GameObject hunter;
    public Text timerText;
    public Text winText;
    public Text resetText;
    public string levelName;
    public double totalTime = 66.6;
    private double goalTime = 0.0;
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
        goalTime = Time.time + totalTime;
        winText.gameObject.SetActive(false);
    }

    void Update()
    {
        double guiTime;
        if (!paused)
        {
            guiTime = goalTime - Time.time;
            guiTime = totalTime - guiTime;
            guiTime = System.Math.Truncate(guiTime * 10) / 10;
            if (guiTime > totalTime)
            {
                paused = true;
                DisplayWin("Piggy Wins!");
            }

            string displayText;
            displayText = (guiTime > totalTime) ? totalTime.ToString() : guiTime.ToString();
            if (guiTime % 1 == 0) displayText = displayText + ".0"; 
            timerText.text = displayText;
        }

        if(paused)
        {
            if(Input.GetButtonDown("Submit"))
            {
                Reset();
            }
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

    void Reset()
    {
       UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    public void DisplayWin(string message)
    {
        winText.gameObject.SetActive(true);
        winText.text = message;

        resetText.gameObject.SetActive(true);
    }
}
