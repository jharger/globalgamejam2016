using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public static GameController instance;
    public float gameTime;
    public bool paused = false;



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
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!paused)
        {
            gameTime -= Time.fixedDeltaTime;
        }
	}
}
