using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementScript : MonoBehaviour {

	public string elementType = "yellow";
	// keep a copy of the executing script
    private Coroutine coroutine;
    bool animationRunning = false;

	public void SwapAnimation(Transform destination, Vector3 offset, float approxAnimTime){
		offset.x *= destination.localScale.x;
		offset.y *= destination.localScale.y;
		if(animationRunning)
		{
			StopCoroutine(coroutine);
		}
		coroutine = StartCoroutine(SwapCoroutine(destination, offset, approxAnimTime));
		animationRunning = true;
	}

	IEnumerator SwapCoroutine(Transform destination, Vector3 offset, float approxAnimTime){
		while(Vector3.Distance(transform.position, destination.position + (offset)) > 0.01f){
			transform.position = Vector3.Lerp(transform.position, destination.position + offset, Time.deltaTime / approxAnimTime);
			yield return 0;
		}
		animationRunning = false;
		yield return 0;
	}

	public void DestroyAnimation(float approxAnimTime){
		StartCoroutine(DestroyCoroutine(approxAnimTime));
	}

	// lower this element's alpha until it is completely invisible, then destroy it
	IEnumerator DestroyCoroutine(float approxAnimTime){
		float passedTime = 0f;
		Color tmpColor;
		while(passedTime < approxAnimTime){
			tmpColor = transform.GetComponent<SpriteRenderer>().color;
			tmpColor.a -= (Time.deltaTime / approxAnimTime);
			transform.GetComponent<SpriteRenderer>().color = tmpColor; 
			passedTime += Time.deltaTime;
			yield return 0;
		}
		Destroy(transform.gameObject);
		yield return 0;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		// death animation code. to be uncommented when it will have particle coomponent
/*		Transform deathEffect = transform.GetChild(0);
		deathEffect.GetComponent<ParticleSystem>().Play();
		deathEffect.parent = null;
		deathEffect.localScale = new Vector3(1, 1, 1);*/
	}

}
