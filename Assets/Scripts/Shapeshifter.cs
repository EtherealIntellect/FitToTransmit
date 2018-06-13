using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapeshifter : MonoBehaviour {

	[SerializeField]
	float changeInterval = 4f;

	// 0 == yellow, 1 == red, 2 == magenta, 3 == green, 4 == blue, 5 == cyan
	[SerializeField]
	Texture2D[] elementSprites;

	List<Object> data; //holds all possible element sprites
	int index = 0;

	// in order to prevent circle having multiple elements of the same kind, we need to keep track of elements in the same circle as this shapeshiftter
	List<SpriteRenderer> cellMates;

	// Use this for initialization
	void Start () {
		data = new List<Object>(Resources.LoadAll("Elements", typeof(Sprite)));
		InvokeRepeating("ChangeElement", 0.75f, changeInterval);
		InvokeRepeating("GlowEffect", 0f, changeInterval);
	}

	void ChangeElement(){

		// get the elements that are in current circle
		cellMates = new List<SpriteRenderer>(transform.parent.GetComponentsInChildren<SpriteRenderer>());
		if(cellMates.Count > 1){
			// if it is not alon in the cire remove it from the list of elements not suitable for shapeshifting to (duplicates)
			cellMates.RemoveAt(transform.GetSiblingIndex());
		}
		// make a new array made out of all  elements for use
		List<Object> validSprites = new List<Object>(data);
		// make a list of valid elements for use
		for(int i = 0; i < cellMates.Count; i++){
			
			for(int j = 0; j < validSprites.Count; j++){
				if(cellMates[i].sprite == (validSprites[j] as Sprite)){
					validSprites.RemoveAt(j);
				}

			}
			
		}
		//Load Sprite From The Resources Folder and use
		transform.GetComponent<SpriteRenderer>().sprite = validSprites[ index % validSprites.Count ] as Sprite;
		// and here we finaly assign the new element type according to the new texture. could have gone the opposite way and decide type first and assign texure after but oh well.
		if(transform.GetComponent<SpriteRenderer>().sprite.texture == elementSprites[0]){
			transform.GetComponent<ElementScript>().elementType = "yellow";
		}
		else if(transform.GetComponent<SpriteRenderer>().sprite.texture == elementSprites[1]){
			transform.GetComponent<ElementScript>().elementType = "red";
		}
		else if(transform.GetComponent<SpriteRenderer>().sprite.texture == elementSprites[2]){
			transform.GetComponent<ElementScript>().elementType = "magenta";
		}
		else if(transform.GetComponent<SpriteRenderer>().sprite.texture == elementSprites[3]){
			transform.GetComponent<ElementScript>().elementType = "green";
		}
		else if(transform.GetComponent<SpriteRenderer>().sprite.texture == elementSprites[4]){
			transform.GetComponent<ElementScript>().elementType = "blue";
		}
		else if(transform.GetComponent<SpriteRenderer>().sprite.texture == elementSprites[5]){
			transform.GetComponent<ElementScript>().elementType = "cyan";
		}
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
