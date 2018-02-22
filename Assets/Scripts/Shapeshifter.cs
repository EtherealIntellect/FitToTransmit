using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapeshifter : MonoBehaviour {

	[SerializeField]
	float changeInterval = 4f;

	List<Object> data; //holds all possible element sprites
	int index = 0;

	// in order to prevent circle having multiple elements of the same kind, we need to keep track of elements in the same circle as this shapeshiftter
	List<SpriteRenderer> cellMates;

	// Use this for initialization
	void Start () {
		data = new List<Object>(Resources.LoadAll("Elements", typeof(Sprite)));
		InvokeRepeating("ChangeElement", changeInterval, changeInterval);
		InvokeRepeating("GlowEffect", changeInterval-0.75f, changeInterval);
	}

	void ChangeElement(){

		// check which elements are in current circle
		cellMates = new List<SpriteRenderer>(transform.parent.GetComponentsInChildren<SpriteRenderer>());
		cellMates.RemoveAt(transform.GetSiblingIndex());
		// make a new array made out of valid elements for use
		List<Object> validSprites = new List<Object>(data);
		for(int i = 0; i < cellMates.Count; i++){
			
			for(int j = 0; j < validSprites.Count; j++){
				if(cellMates[i].sprite == (validSprites[j] as Sprite)){
					validSprites.RemoveAt(j);
					Debug.Log("removed a color from valid colors");
				}

			}
			
		}
		// also remove current sprite becasue we dont want to repeat this same sprite
		validSprites.Remove(transform.GetComponent<SpriteRenderer>().sprite);
		//Load Sprite From The Resources Folder and use
		transform.GetComponent<SpriteRenderer>().sprite = validSprites[index%validSprites.Count] as Sprite;
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
