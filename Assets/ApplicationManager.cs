using UnityEngine;
using System.Collections;

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager instance;

    void OnEnable()
    {
        instance = this;
    }
    void OnDisable()
    {
        instance = null;
    }
	
	// Update is called once per frame
	void Update () {
	
       if(Input.GetKeyDown(KeyCode.Escape))
       {
           Application.Quit();
       }
	}

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadLevel(string levelToLoad)
    {
        Application.LoadLevel(levelToLoad);
    }
}
