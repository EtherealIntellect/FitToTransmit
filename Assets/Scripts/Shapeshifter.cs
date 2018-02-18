using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapeshifter : MonoBehaviour {

	[SerializeField]
	float changeInterval = 4f;

	Object[] data;
	int index = 0;

	// Use this for initialization
	void Start () {
		data = Resources.LoadAll("Elements", typeof(Sprite));
		InvokeRepeating("ChangeElement", changeInterval, changeInterval);
		InvokeRepeating("GlowEffect", changeInterval-0.5f, changeInterval);
	}

	void ChangeElement(){
		//Load Sprite From The Resources Folder and use
		
		foreach (var t in data)
        {
            Debug.Log(t.name);
        }
		transform.GetComponent<SpriteRenderer>().sprite = data[index%data.Length] as Sprite;
		index++;
		

	}

	void GlowEffect(){
		// play change animation
		transform.GetComponent<Animation>().Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
