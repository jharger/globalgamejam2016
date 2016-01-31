using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;
    public List<AudioSource> audioSources;

    int index = 0;

    void OnEnable()
    {
        instance = this;
    }

    void OnDisable()
    {
        instance = null;
    }
    
    public void PlayClip(AudioClip clip)
    {
        audioSources[index].clip = clip;
        audioSources[index].Play();

        index++;
        if(index >= audioSources.Count)
        {
            index = 0;
        }
    }
}
