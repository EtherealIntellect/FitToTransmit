using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBehaviour : MonoBehaviour {

	private Vector3 _startPosition;
	float widthOfVibe = 3;
	public float timeOffset;
	public float vibeFrequency = 3f;

	// Use this for initialization
	void Start () {
		_startPosition = transform.position;
		timeOffset = Random.Range(0f, 2f);
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = _startPosition + new Vector3( 0f, Mathf.Sin(timeOffset + Time.time*vibeFrequency) / widthOfVibe, 0.0f);

    }

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
		for (int i = 0; i < transform.GetChild(0).childCount; i++)
		{
		    // float dist = Vector3.Distance(transform.GetChild(0).GetChild(i).transform.position, transform.position);
		    // Vector3 direction = (transform.position - transform.GetChild(0).GetChild(i).transform.position);
		    // direction = direction.normalized;
		    //Debug.Log("the distance is ===== " + dist);
		    // if (dist > 0.85f)
		    {
		        transform.GetChild(0).GetChild(i).transform.position = Vector3.Lerp(transform.position, transform.GetChild(0).GetChild(i).transform.position, Time.deltaTime);
		        // transform.GetChild(0).GetChild(i).GetComponent<Rigidbody2D>().AddForce(direction * 2, ForceMode2D.Force);
		    }
		}
    }
}
