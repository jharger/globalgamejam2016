using UnityEngine;
using System.Collections;

public class MudPuddle : MonoBehaviour {
    public ParticleSystem particles;
    public Texture2D mudPig;
    public float speedMultiplier = 1.5f;

    Texture savedTex;

    void Start()
    {
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Piggy"))
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
            particles.transform.position = other.transform.position;
            SkinnedMeshRenderer ren = other.GetComponentInChildren<SkinnedMeshRenderer>();
            savedTex = ren.material.mainTexture;
            ren.material.mainTexture = mudPig;
            ThirdPersonPig pig = other.GetComponent<ThirdPersonPig>();
            pig.m_MoveSpeedMultiplier = speedMultiplier;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Piggy"))
        {
            particles.transform.position = other.transform.position;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Piggy"))
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = false;
            SkinnedMeshRenderer ren = other.GetComponentInChildren<SkinnedMeshRenderer>();
            ren.material.mainTexture = savedTex;
            ThirdPersonPig pig = other.GetComponent<ThirdPersonPig>();
            pig.m_MoveSpeedMultiplier = 1f;
        }
    }
}
