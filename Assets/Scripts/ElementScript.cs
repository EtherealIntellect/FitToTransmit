using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		// death animation code
/*		Transform deathEffect = transform.GetChild(0);
		deathEffect.GetComponent<ParticleSystem>().Play();
		deathEffect.parent = null;
		deathEffect.localScale = new Vector3(1, 1, 1);*/
	}

}
