using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CircleBehaviour : MonoBehaviour {

	private Vector3 _startPosition;
	public float widthOfVibe = 2;
	public float timeOffset;
	public float vibeFrequency = 3f;
	public bool hasCharge = false;

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

		// just in case I need the leve to start with a charged circle
		if(hasCharge){ChargeUp();}

		
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
			    	GetComponent<SpriteRenderer>().color = new Color32( 43, 43, 43, 255);
				}
			}
		}
		else{
			// visual cue for failed transmutation
		}
	}

	
	// Update is called once per frame
	void Update () {

		transform.position = _startPosition + new Vector3( 0f, Mathf.Sin(timeOffset + Time.time*vibeFrequency) * widthOfVibe, 0.0f);

    }

    void FixedUpdate(){
    }

    void ChargeUp(){
    	hasCharge = true;
    	// do some animation or other visual effects to tell player this circle is charged up
    	transform.GetComponent<Animator>().SetBool("isCharged", true);
    	SwapEvent();
    }

    void SwapEvent(){
    	// change the color of this core to be the mix of its elements (only if charged up) (replace channel color values of core with sum of each channel values of elements)
    	Transform elemnts = transform.Find("elements");
    	if(hasCharge){

	    	if(elemnts.childCount == 2){
	    		String firstElement = elemnts.GetChild(0).GetComponent<ElementScript>().elementType;
	    		String secondElement = elemnts.GetChild(1).GetComponent<ElementScript>().elementType;
	    		if((firstElement == "red" || secondElement == "red" ) && (firstElement == "green" || secondElement == "green" )){
	    			transform.GetComponent<Animator>().enabled = false;
	    			GetComponent<SpriteRenderer>().color = new Color32( 255, 255, 26, 255);
	    			Debug.Log(GetComponent<SpriteRenderer>().color);
	    		}
	    		else if((firstElement == "red" || secondElement == "red" ) && (firstElement == "blue" || secondElement == "blue" )){
	    			transform.GetComponent<Animator>().enabled = false;
	    			GetComponent<SpriteRenderer>().color = new Color32( 255, 26, 255, 255);
	    			Debug.Log(GetComponent<SpriteRenderer>().color);
	    		}
	    		else if((firstElement == "green" || secondElement == "green" ) && (firstElement == "blue" || secondElement == "blue" )){
	    			transform.GetComponent<Animator>().enabled = false;
	    			GetComponent<SpriteRenderer>().color = new Color32( 26, 255, 255, 255);
	    			Debug.Log(GetComponent<SpriteRenderer>().color);
	    		}
	    	}
	    	else {
	    		transform.GetComponent<Animator>().enabled = true;
	    	}
    	}
    }
}
