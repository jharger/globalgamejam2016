using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Tween : MonoBehaviour {

	public float lerpSpeed = 5.0f;
	//public float moveTime;
	public Transform TargetMask;
	public bool debug = false;
	public bool moved = false;

	private Vector3 targetOffset;
	private RectTransform rectTransform;
	private Vector2 startPos;
	private BoxCollider maskCollider;
	// Use this for initialization
	void Start () 
	{
		targetOffset = TargetMask.InverseTransformPoint(this.transform.position);

		rectTransform = this.GetComponent<RectTransform> ();
		startPos = rectTransform.anchoredPosition;

	
		maskCollider = TargetMask.GetComponent<BoxCollider> ();
		if(maskCollider != null)
		{
			maskCollider.enabled = false;
		}
	}

	void Update()
	{
		//Vector3 moveBackPos = Target.transform.pos
		Vector3 pos = TargetMask.transform.TransformPoint (targetOffset);
		Debug.DrawLine(this.transform.position,pos,Color.blue);
	}
	
	void OnDrawGizmosSelected() {
		if (TargetMask != null && debug) 
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, TargetMask.position);
			Gizmos.DrawSphere(TargetMask.position, .1f);
			//RectTransform r = gameObject.GetComponent<RectTransform>();
			//Target.sizeDelta = new Vector2( r.sizeDelta.x, r.sizeDelta.y );
		}
	}

	public void Move()
	{
		//Debug.Log ("Moving! " + this.gameObject.name);
		if (!isActiveAndEnabled)
			return;
		moved = true;
		StopAllCoroutines ();
		//StartCoroutine (Tween (Target.transform.position));
		StartCoroutine (Tween (new Vector2(0,0)));
		//iTween.MoveTo(this.gameObject,Target.transform.position,moveTime);
	}

	public void MoveBack()
	{

		if (!isActiveAndEnabled)
			return;
		//Debug.Log ("Moving Back! " + this.gameObject.name);
		Vector3 pos = TargetMask.transform.TransformPoint (targetOffset);
		moved = false;
		StopAllCoroutines ();
		//StartCoroutine (Tween (pos));
		StartCoroutine (Tween (startPos));
		//iTween.MoveTo(this.gameObject,pos,moveTime);
	
	}

	public void MoveBackInstant()
	{
		if (!isActiveAndEnabled)
			return;
		Vector3 pos = TargetMask.transform.TransformPoint (targetOffset);
		this.transform.position = pos;
		//iTween.MoveTo(this.gameObject,pos,moveTime);
		moved = false;
	}

	//if a delay is passed in, it'll wait that amount of time before tweening
	public void ToggleMove(float delay)
	{
		if (moved) {
			Invoke("MoveBack",delay);
		}
		else
		{
			Invoke("Move",delay);
		}
	}

	IEnumerator Tween(Vector2 anchordPosition)
	{
		//if tweening in, enable the mask
		if(moved)
		{
			if(maskCollider != null)
			{
				maskCollider.enabled = true;
			}
		}
		else{
			//we need to disable the collider right away, so that other menu's can be selected, and clicking on the mask doesn't toggle anything.
			if(maskCollider != null)
			{
				maskCollider.enabled = false;
			}
		}

		//Debug.Log ("Tweening");

		while(Vector2.Distance(rectTransform.anchoredPosition,anchordPosition) > 0.1f)
		{
			rectTransform.anchoredPosition = Vector2.Lerp (rectTransform.anchoredPosition,anchordPosition,lerpSpeed*Time.smoothDeltaTime);
			yield return null;
		}
	}


}
