using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CircleBehaviour : MonoBehaviour {

	private Vector3 _startPosition;
	public float widthOfVibe = 2;
	public float timeOffset;
	public float vibeFrequency = 3f;
	bool hasCharge = false;

	public Transform[] primaryElements;
	public Transform[] secondaryElements;
	// Sprite[] primarySprites;

	// element positions in circle depending on element count
	[SerializeField]
	Vector3[] oneElements;
	[SerializeField]
	Vector3[] twoElements;
	[SerializeField]
	Vector3[] threeElements;
	[SerializeField]
	Vector3[] fourElements;
	[SerializeField]
	Vector3[] fiveElements;
	[SerializeField]
	Vector3[] sixElements;

	// Use this for initialization
	void Start () {
		_startPosition = transform.position;
		timeOffset = UnityEngine.Random.Range(0f, 2f);

		
	}

	void OnMouseUp(){
		// code for combining elements
		Transform elemnts = transform.Find("elements");
		Sprite[] primarySprites = {primaryElements[0].GetComponent<SpriteRenderer>().sprite, primaryElements[1].GetComponent<SpriteRenderer>().sprite, primaryElements[2].GetComponent<SpriteRenderer>().sprite};
		// if this circle is charged and has only two elements combine them into one
		if(hasCharge && elemnts.childCount == 2){
			if(elemnts.GetChild(0).GetComponent<SpriteRenderer>().color.a != elemnts.GetChild(1).GetComponent<SpriteRenderer>().color.a){

				int firstElementIndex = Array.IndexOf(primarySprites, elemnts.GetChild(0).GetComponent<SpriteRenderer>().sprite);
				int secondElementIndex = Array.IndexOf(primarySprites, elemnts.GetChild(1).GetComponent<SpriteRenderer>().sprite);
				if(firstElementIndex >= 0 && 
					secondElementIndex >= 0){

					Transform newElement;
					if((firstElementIndex == 0 || secondElementIndex == 0) && (secondElementIndex == 1 || firstElementIndex == 1)){
						// red and green make yellow
						newElement = Instantiate(secondaryElements[0], transform.position, Quaternion.identity) as Transform;
						newElement.SetParent(elemnts, true);
					}
					else if((firstElementIndex == 0 || secondElementIndex == 0) && (secondElementIndex == 2 || firstElementIndex == 2)){
						// red and blue make magenta
						newElement = Instantiate(secondaryElements[1],  transform.position, Quaternion.identity) as Transform;
						newElement.SetParent(elemnts, true);
					}
					else if((firstElementIndex == 1 || secondElementIndex == 1) && (secondElementIndex == 2 || firstElementIndex == 2)){
						// green and blue make cyan
						newElement = Instantiate(secondaryElements[2],  transform.position, Quaternion.identity) as Transform;
						newElement.SetParent(elemnts, true);
					}

					// destroy the primary elements
					Destroy(elemnts.GetChild(0).gameObject);
					Destroy(elemnts.GetChild(1).gameObject);

					hasCharge = false;
					// visual discharge code
			    	transform.GetComponent<Animator>().SetBool("isCharged", false);
				}
			}
		}
		else{
			// visual cue for failed transmutation
		}
	}

	
	// Update is called once per frame
	void Update () {

		transform.position = _startPosition + new Vector3( 0f, Mathf.Sin(timeOffset + Time.time*vibeFrequency) / widthOfVibe, 0.0f);

    }


    // private Vector3 velocity = Vector3.zero;
    // public float smoothTime = 0.5F;
    void FixedUpdate(){
    	//fixing the exiting circles, standard size of 0.85
/*    	for (int i = 0; i < transform.GetChild(0).childCount; i++)
    	{
    	    float dist = Vector3.Distance(transform.GetChild(0).GetChild(i).transform.position, transform.position);
    	    // if (dist > 0.85f)
    	    // {
    	        transform.GetChild(0).GetChild(i).transform.position = Vector3.MoveTowards(transform.position, transform.GetChild(0).GetChild(i).transform.position, Time.deltaTime * 8);
    	    // }
    	}*/
    	// widthOfVibe = Random.Range(1f, Mathf.Sin(Time.time)*2);
    	//restoring if any circle scapes boundary
    			//fixing the exiting circles, standard size of 0.85
		for (int i = 0; i < transform.Find("elements").childCount; i++)
		{
		    // float dist = Vector3.Distance(transform.GetChild(0).GetChild(i).transform.position, transform.position);
		    // Vector3 direction = (transform.position - transform.GetChild(0).GetChild(i).transform.position);
		    // direction = direction.normalized;
		    //Debug.Log("the distance is ===== " + dist);
		    // if (dist > 0.85f)
		    {
		        // transform.GetChild(0).GetChild(i).transform.position = Vector3.Lerp(transform.GetChild(0).GetChild(i).transform.position, transform.position + (transform.GetChild(0).GetChild(i).transform.position - transform.position), Time.deltaTime*0.01f);
		        // transform.GetChild(0).GetChild(i).transform.position = transform.position + (transform.GetChild(0).GetChild(i).transform.position - transform.position);
		        // transform.GetChild(0).GetChild(i).transform.position = Vector3.SmoothDamp(transform.GetChild(0).GetChild(i).transform.position, transform.position + (transform.GetChild(0).GetChild(i).transform.position - transform.position), ref velocity, smoothTime);
		        // transform.GetChild(0).GetChild(i).GetComponent<Rigidbody2D>().AddForce(direction * 2, ForceMode2D.Force);
		    }
		}
    }

    void ChargeUp(){
    	hasCharge = true;
    	// do some animation or other visual effects to tell player this circle is charged up
    	transform.GetComponent<Animator>().SetBool("isCharged", true);

    }


}
