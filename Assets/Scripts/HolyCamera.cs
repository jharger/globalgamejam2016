using UnityEngine;
using System.Collections;

public class HolyCamera : MonoBehaviour {
    public Material mat;
    public float nearClip = 7f;

    Camera holyCam;
    RenderTexture tex;

    void Awake()
    {
        GameObject holyObject = new GameObject("Holy Camera");
        holyObject.transform.SetParent(transform, false);
        Camera mainCam = GetComponent<Camera>();
        holyCam = holyObject.AddComponent<Camera>();
        holyCam.CopyFrom(mainCam);
        holyCam.nearClipPlane = nearClip;
    }

    void Start()
    {
        tex = new RenderTexture(Screen.width, Screen.height, 0);
        holyCam.targetTexture = tex;
        mat.SetTexture("_SubTex", tex);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }
}
