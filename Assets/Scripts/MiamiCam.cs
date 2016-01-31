using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MiamiCam : MonoBehaviour {
    public Transform target;
    public float distance = 10f;
    public float leadTime = 4f;
    public float maxSpeed = 7f;
    public float smoothTime = 1f;

    ThirdPersonPig pig;

    Vector3 targetPos;
    Vector3 vel = Vector3.zero;

    void Start ()
    {
        targetPos = target.position + Vector3.up * distance;
        transform.position = targetPos;

        pig = target.GetComponent<ThirdPersonPig>();
    }

    void LateUpdate () {
        targetPos = target.position + target.forward * pig.m_ForwardAmount * leadTime + Vector3.up * distance;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, smoothTime, maxSpeed, Time.deltaTime);
    }
}
