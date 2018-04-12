using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {


	public Vector3 offset; // distance from center of player circle used for elements position inside it
	public Transform playerElements; // a child object of player containing all the elements
	Transform circleSkills; // the elements inside the colliding circle
	
	// collision mechanic vars
	public Vector2 velocity;
    Rigidbody2D rb2D;
    bool touchedCircle = false;
    bool touchedPoint = false; // for moving the player circle to the lastt position of the mouse.
    Vector3 lastClickedPosition;

	public float range = 0.5f;
	public float smoothValue = 1;
	public float nextLevelDelay = 2f;

	// elements swap animation time
	float approxAnimTime = 0.1f;
	float timePassed = 0f;
	float animationTime = 0.3f;
	bool stillSwapping = false;

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

	// destruction animation prefab
	[SerializeField]
	Transform destructionEffect;
	Transform graveyard; // destroyed elements are sent here before actual destruction. I was haing problems with popping elements, I think it was something to do with rigidbodys
	// duration of destruction effect
	[SerializeField]
	float destructionEffect_duration = 0.6f;

	// the combination of elements players needs to obtain to pass level
	GameObject solution;

	public int currentScore = 0;
	Text ScoreText; // where current number of moves is printed out

	// reference to the PersistentObject that manages game state
	Transform persistentObject;
	// just for editor debugging
	[SerializeField]
	Transform persistentObjectPrefab;

	void Awake(){
// #if UNITY_EDITOR || UNITY_STANDALONE
		// instantiate a PersistentObject in case we are not starting from intro (where there already is a PersistentObject)
		if(!GameObject.FindWithTag("GameController")){
			Instantiate(persistentObjectPrefab.gameObject);
		};
// #endif		
	}
	// Use this for initialization
	void Start () {
		// initialization
		offset = new Vector3(0,0,0);
		playerElements = transform.Find("elements");
		rb2D = GetComponent<Rigidbody2D>();
		persistentObject = GameObject.FindWithTag("GameController").transform;
		solution = GameObject.Find("solution").transform.Find("correct_combo").gameObject;
		graveyard = GameObject.Find("Graveyard").transform;

		// place all elements in correct patterns in their respective circles before game starts
		GameObject[] passiveCores = GameObject.FindGameObjectsWithTag("PassiveCore");
		foreach(GameObject core in passiveCores){
			List<Transform> childElements = new List<Transform>();
			foreach(Transform element in core.transform.Find("elements")){
				childElements.Add(element);
			}

			SwapAnimation(childElements, core.transform);
		}
		// do the same for the active core (player)
		List<Transform> _childElements = new List<Transform>();
		foreach(Transform element in transform.Find("elements")){
			_childElements.Add(element);
		}
		SwapAnimation(_childElements, transform);
	}

	IEnumerator OnMouseUp(){
		// go twoards released mouse point
		if((!touchedPoint) && (touchedCircle)){
			// yield return StartCoroutine("MoveTowardsPoint");
		}
		GetComponent<TargetJoint2D>().enabled = true;
		touchedPoint = false;
		touchedCircle = false;
		yield return 0 ;
	}

	//  moves the player circle to the last position of mouse after release of mouse button (deprecated at the moment)
/*	IEnumerator MoveTowardsPoint(){
		Vector3 v3 = Input.mousePosition;
		v3.z = 10.1f;
		Vector3 wrldPoint = Camera.main.ScreenToWorldPoint(v3);
		lastClickedPosition = transform.InverseTransformPoint(wrldPoint);
		while(Vector3.Distance(wrldPoint, transform.position) > 1f){

			if(!touchedCircle){
				rb2D.AddForce(lastClickedPosition * smoothValue, ForceMode2D.Impulse);
			}
			else{
				break;
			}
			
		    yield return 0;
		}

		yield return 0;
	}*/

	void OnMouseDown(){


    	GetComponent<TargetJoint2D>().enabled = false;
	}

	// move player circle towards mouse cursor
	void OnMouseDrag()
    {	
    	if(!touchedCircle && !stillSwapping){
	    	Vector3 v3 = Input.mousePosition;
	    	v3.z = 10.1f;
	    	lastClickedPosition = Camera.main.ScreenToWorldPoint(v3);
	    	lastClickedPosition = transform.InverseTransformPoint(lastClickedPosition);
	    	rb2D.AddForce(lastClickedPosition * smoothValue, ForceMode2D.Impulse);
    	}
    	else{
			// GetComponent<TargetJoint2D>().enabled = true;
    	}
    }

	// Update is called once per frame
	void Update () {
		if(stillSwapping){
			if(timePassed < animationTime){
				timePassed += Time.deltaTime;
			}
			else{
				stillSwapping = false;
				timePassed = 0f;
			}
		}

	}

	void FixedUpdate(){

		//fixing the exiting circles, standard size of 0.85
		for (int i = 0; i < transform.GetChild(0).childCount; i++)
		{
		    {
		        // transform.GetChild(0).GetChild(i).transform.position = Vector3.Lerp(transform.position, transform.GetChild(0).GetChild(i).transform.position, Time.deltaTime);
		        // transform.GetChild(0).GetChild(i).transform.position = transform.position + (transform.GetChild(0).GetChild(i).transform.position - transform.position);
		    }
		}


	}

	void OnCollisionEnter2D(Collision2D collision){
		if(!touchedCircle){


			// stop controlling the circle
			touchedCircle = true;
			// also stop all movement of ball so it doesnt bounce off to another ball unintentionally
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			stillSwapping = true;
			// circle goes back to origin
			GetComponent<TargetJoint2D>().enabled = true;

			// add to the move count score
			currentScore++;
			persistentObject.gameObject.SendMessage("HandleMoveScore", currentScore);

			// play swap music
			Camera.main.GetComponents<AudioSource>()[0].Play();

			// swap code:

			circleSkills = collision.transform.Find("elements");

			List<Transform> playerChildren = new List<Transform>();
			List<Transform> circleChildren = new List<Transform>();
			// List<Transform> toBeAppended = new List<Transform>(); //never used
			List<Transform> toBeDestroyed = new List<Transform>();
			List<Transform> finalplayer = new List<Transform>(); // all these temporary lists seemed to be needed because of destroying and reparenting elements on the fly, before garbage collection can take place

			// get player skills
			for(int i = 0; i < playerElements.childCount; i++)
			{
				playerChildren.Add(playerElements.GetChild(i));
				finalplayer.Add(playerElements.GetChild(i));
			}
			// get circle skills
			for(int i = 0; i <  circleSkills.childCount; i++)
			{
				circleChildren.Add( circleSkills.GetChild(i));
				finalplayer.Add(circleSkills.GetChild(i));
			}

			// delete same elements
			foreach(Transform skillInPlayer in playerChildren){

				foreach(Transform skillInCircle in circleChildren){

					Sprite circleSkillSprite = skillInCircle.GetComponent<SpriteRenderer>().sprite;
					Sprite playerSkillSprite = skillInPlayer.GetComponent<SpriteRenderer>().sprite;
					
					if(circleSkillSprite == playerSkillSprite)
					{
						// this check makes sure we can have more than one element of the same color in a circle
						if(!toBeDestroyed.Contains(skillInPlayer) && !toBeDestroyed.Contains(skillInCircle)){
							// play destroy music
							Camera.main.GetComponents<AudioSource>()[2].Play();
							toBeDestroyed.Add(skillInPlayer);
							toBeDestroyed.Add(skillInCircle);
							finalplayer.Remove(skillInPlayer);
		                    finalplayer.Remove(skillInCircle);
	                	}
	                	else{
							// Debug.Log("Has one extra element of the same color");
	                	}
					}

				}
			}

			// destroy same-colored elements
			foreach(Transform child in toBeDestroyed){ 
				circleChildren.Remove(child); // I don't know how it finds the transform objects in a list of transforms, but it works for now.
				playerChildren.Remove(child); // maybe I should look into it
				// remove element from core but don't destroy it yet so it can be used for destruction animation
				Vector3 prevPos = child.position;
				// child.SetParent(null, true);
				child.parent = graveyard;
				// child.position = prevPos;
				// Debug.Log(child.position.x);
				// passive core gains a charge now
				collision.gameObject.SendMessage("ChargeUp");
			}
			DestructionAnim(toBeDestroyed);

			// swap remaining elements
			foreach(Transform child in playerChildren){
				finalplayer.Remove(child);
				child.parent = circleSkills;
				// child.position = circleSkills.position + (offset + new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));
				offset = Vector3.zero;
			}
			SwapAnimation(playerChildren, circleSkills.parent);

			foreach(Transform child in circleChildren){
				/*child.parent = null;
				finalplayer.Remove(child);*/
				// child.SendMessage("SwapAnimation", playerElements);
				child.parent = playerElements;
				// finalplayer.Remove(child);
				// child.position = playerElements.position + (offset + new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));
				offset = Vector3.zero;
			}
			SwapAnimation(circleChildren, playerElements.parent);




			//testing for the win condition
	        bool solved = true;
	 
	        for (int i = 0; i < solution.transform.childCount; i++)
	        {
	            bool found = false;
	            foreach (Transform child in finalplayer)
	            {
	                if (solution.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite == child.GetComponent<SpriteRenderer>().sprite)
	                {
	                	// Debug.Log(finalplayer.Count);	
	                    found = true;

	                }
	            }
	            if (found == false)
	            { 
	            solved = false;
	            }
	 
	        }
	        if (solved==true)
	        {
	            // Debug.Log("Half solved");
	            // Debug.Log(finalplayer.Count + "," + solution.transform.childCount);
	            if (finalplayer.Count == solution.transform.childCount)
	            {
	                Debug.Log("Game over");
	                // lock control over ball
	                GetComponent<CircleCollider2D>().enabled = false;;
	                // load next level afer a while
	                Invoke("LoadNextLevel", nextLevelDelay);
	            }
	        }
		}
	}
	
	public void LoadNextLevel(){

		// before loading next stage add current moves score to global score
		persistentObject.GetComponent<GameScript>().TotalMoves += currentScore;
		persistentObject.GetComponent<GameScript>().SaveLevelScore(currentScore, int.Parse(solution.transform.parent.Find("optimalScore").GetComponent<Text>().text));
		persistentObject.GetComponent<GameScript>().lastLevel = SceneManager.GetActiveScene().buildIndex;

		Resources.UnloadUnusedAssets();
		SceneManager.LoadScene("Next Level");
	}

	public void RestartLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void OnUpdateScoreBar(){
		if(GameObject.Find("Moves Counter").GetComponent<Slider>().value != 1)
		{
			Transform scoreFill = GameObject.Find("Fill").transform;
			Transform tmpSingleMove = Instantiate(scoreFill.Find("SingleMove"));
			tmpSingleMove.SetParent(scoreFill);
			tmpSingleMove.localScale = Vector3.one;
			if(scoreFill.parent.parent.GetComponent<Slider>().maxValue > int.Parse(GameObject.Find("solution").transform.Find("optimalScore").GetComponent<Text>().text)){
				tmpSingleMove.GetComponent<Image>().color = Color.red;	// if player crossed the optimal moves threshold start marking new moves with red	
				Transform tmpMoveSlot = Instantiate(scoreFill.parent.parent.Find("Background").Find("MoveSlot"));
				tmpMoveSlot.SetParent(scoreFill.parent.parent.Find("Background"));
				tmpMoveSlot.localScale = Vector3.one;		
			}

		}
	}

	IEnumerator KillDestructionEffect(Transform effect, float effectDuration){
		float passedTime = 0;
		float alpha = 1.0f;
		Color tmpColor = effect.GetComponentInChildren<LineRenderer>().colorGradient.colorKeys[0].color;
		while(passedTime < effectDuration){
			alpha -= (Time.deltaTime / effectDuration);
			foreach(LineRenderer line in effect.GetComponentsInChildren<LineRenderer>()){
		        Gradient gradient = new Gradient();
		        gradient.SetKeys(
		            new GradientColorKey[] { new GradientColorKey(tmpColor, 0.0f), new GradientColorKey(tmpColor, 1.0f) },
		            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
		            );
		        line.colorGradient = gradient;		
			}
			yield return 0;
		}
		// yield return new WaitForSeconds(effectDuration);
		Destroy(effect.gameObject);
		yield return 0;
	}


	void DestructionAnim(List<Transform> destroyedElements){
		Transform tmpEffect;
		for(int i = 0; i < destroyedElements.Count/2; i++){
			// create destruction effect between both destroyed elements
			tmpEffect = Instantiate(destructionEffect, Vector3.zero, Quaternion.identity);
			tmpEffect.Find("LightningStart").position = destroyedElements[i*2].position;
			tmpEffect.Find("LightningEnd").position = destroyedElements[i*2 + 1].position;
			// start destroy animation of elements as well
			destroyedElements[i*2].GetComponent<ElementScript>().DestroyAnimation(destructionEffect_duration);
			destroyedElements[i*2 + 1].GetComponent<ElementScript>().DestroyAnimation(destructionEffect_duration);
			// StartCoroutine("DestructionEffect_FadeOut", 0.8);
			// adjust the destruction effect color according to destroyed element type
			switch(destroyedElements[i*2].GetComponent<ElementScript>().elementType){
				case "yellow":
					// yellow
					Debug.Log("yellow");
					foreach(LineRenderer line in tmpEffect.GetComponentsInChildren<LineRenderer>()){
						// A simple 2 color gradient with a fixed alpha of 1.0f.
				        float alpha = 1.0f;
				        Gradient gradient = new Gradient();
				        gradient.SetKeys(
				            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.yellow, 1.0f) },
				            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
				            );
				        line.colorGradient = gradient;
					}
					break;
				case "red":
					// red
					Debug.Log("red");
					foreach(LineRenderer line in tmpEffect.GetComponentsInChildren<LineRenderer>()){
						// A simple 2 color gradient with a fixed alpha of 1.0f.
				        float alpha = 1.0f;
				        Gradient gradient = new Gradient();
				        gradient.SetKeys(
				            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
				            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
				            );
				        line.colorGradient = gradient;
					}
					break;
				case "magenta":
					// magenta
					Debug.Log("magenta");
					foreach(LineRenderer line in tmpEffect.GetComponentsInChildren<LineRenderer>()){
						// A simple 2 color gradient with a fixed alpha of 1.0f.
				        float alpha = 1.0f;
				        Gradient gradient = new Gradient();
				        gradient.SetKeys(
				            new GradientColorKey[] { new GradientColorKey(Color.magenta, 0.0f), new GradientColorKey(Color.magenta, 1.0f) },
				            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
				            );
				        line.colorGradient = gradient;
					}
					break;
				case "green":
					// green
					Debug.Log("green");
					foreach(LineRenderer line in tmpEffect.GetComponentsInChildren<LineRenderer>()){
						// A simple 2 color gradient with a fixed alpha of 1.0f.
				        float alpha = 1.0f;
				        Gradient gradient = new Gradient();
				        gradient.SetKeys(
				            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
				            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
				            );
				        line.colorGradient = gradient;
					}
					break;
				case "blue":
					// blue
					Debug.Log("blue");
					foreach(LineRenderer line in tmpEffect.GetComponentsInChildren<LineRenderer>()){
						// A simple 2 color gradient with a fixed alpha of 1.0f.
				        float alpha = 1.0f;
				        Gradient gradient = new Gradient();
				        gradient.SetKeys(
				            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
				            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
				            );
				        line.colorGradient = gradient;
					}
					break;
				case "cyan":
					// cyan
					Debug.Log("cyan");
					foreach(LineRenderer line in tmpEffect.GetComponentsInChildren<LineRenderer>()){
						// A simple 2 color gradient with a fixed alpha of 1.0f.
				        float alpha = 1.0f;
				        Gradient gradient = new Gradient();
				        gradient.SetKeys(
				            new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.cyan, 1.0f) },
				            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
				            );
				        line.colorGradient = gradient;
					}
					break;
				default: 
					break;

			}
			// kill lightning after some time
			StartCoroutine(KillDestructionEffect(tmpEffect, destructionEffect_duration));
		}
	}


    void SwapAnimation(List<Transform> circleChildren, Transform destination){
    	
    	// int elementsCount = destroyedCount == 0? circleChildren.Count : circleChildren.Count - destroyedCount / 2;
    	switch(circleChildren.Count){
    		case 1:
    			{
    			int i = 0;
				// Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
				Vector3 offset = oneElements[i];
				circleChildren[0].parent = destination.Find("elements");
				circleChildren[0].GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
				// child.SendMessage("SwapAnimation", destination.position + offset);
    			}
				
    			break;
    		case 2:
    			{
    				int i = 0;
					foreach(Transform child in circleChildren){
						// Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
						Vector3 offset = twoElements[i];
						child.parent = destination.Find("elements");
						child.GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
						i++;
					}
    			}
    			break;
			case 3:
				{
    				int i = 0;
					foreach(Transform child in circleChildren){
						// Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
						Vector3 offset = threeElements[i];
						child.parent = destination.Find("elements");
						child.GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
						i++;
					}
    			}
    			break;
    		case 4:
				{
    				int i = 0;
					foreach(Transform child in circleChildren){
						// Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
						Vector3 offset = fourElements[i];
						child.parent = destination.Find("elements");
						child.GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
						i++;
					}
    			}
    			break;
    		case 5:
				{
    				int i = 0;
					foreach(Transform child in circleChildren){
						// Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
						Vector3 offset = fiveElements[i];
						child.parent = destination.Find("elements");
						child.GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
						i++;
					}
    			}
    			break;	    			
    		case 6:
				{
    				int i = 0;
					foreach(Transform child in circleChildren){
						// Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
						Vector3 offset = sixElements[i];
						child.parent = destination.Find("elements");
						child.GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
						i++;
					}
    			}
    			break;	
			default:
				foreach(Transform child in circleChildren){
					Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
					child.parent = destination.Find("elements");
					child.GetComponent<ElementScript>().SwapAnimation(destination, offset, approxAnimTime);
					// child.SendMessage("SwapAnimation", destination.position + offset);
				}
    			break;
    	}
    }
}
