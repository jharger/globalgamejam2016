using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {

    public string NextLevel;

	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetKeyDown(KeyCode.Return))
        {
            ApplicationManager.instance.LoadLevel(NextLevel);
        }
	}
}
