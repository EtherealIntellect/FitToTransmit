using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {


	public Vector3 offset; // distance from center of player circle used for elements position inside it
	public Transform playerSkills; // a child object of player containing all the elements
	Transform circleSkills; // the elements inside the colliding circle
	
	// collision mechanic vars
	public Vector2 velocity;
    Rigidbody2D rb2D;
    bool touchedCircle = false;
    bool touchedPoint = false;
    Vector3 lastClickedPosition;

	public float range = 0.5f;
	public float smoothValue = 1;

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
#if UNITY_EDITOR
		// instantiate a PersistentObject in case we are not testing from intro
		if(!GameObject.FindWithTag("GameController")){
			Instantiate(persistentObjectPrefab.gameObject);
		};
#endif		
	}
	// Use this for initialization
	void Start () {



		// initialization
		offset = new Vector3(0,0,0);
		playerSkills = transform.Find("playerSkills");
		rb2D = GetComponent<Rigidbody2D>();
		persistentObject = GameObject.FindWithTag("GameController").transform;
		solution = GameObject.Find("solution").transform.Find("correct_combo").gameObject;



	}

	IEnumerator OnMouseUp(){

		// go twoards released mouse point
		if((!touchedPoint) && (!touchedCircle)){
			yield return StartCoroutine("MoveTowardsPoint");

		}
		GetComponent<TargetJoint2D>().enabled = true;
		touchedPoint = false;
		touchedCircle = false;
		yield return 0 ;
	}

	IEnumerator MoveTowardsPoint(){
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
	}

	void OnMouseDrag()
    {	
    	GetComponent<TargetJoint2D>().enabled = false;
    	if(!touchedCircle){
	    	Vector3 v3 = Input.mousePosition;
	    	v3.z = 10.1f;
	    	lastClickedPosition = Camera.main.ScreenToWorldPoint(v3);
	    	lastClickedPosition = transform.InverseTransformPoint(lastClickedPosition);
	    	rb2D.AddForce(lastClickedPosition * smoothValue, ForceMode2D.Impulse);
    	}
    	else{
    		GetComponent<TargetJoint2D>().enabled = true;
    	}
    }

	// Update is called once per frame
	void Update () {

		
	}

	void FixedUpdate(){

		//fixing the exiting circles, standard size of 0.85
		for (int i = 0; i < transform.GetChild(0).childCount; i++)
		{
		    {
		        transform.GetChild(0).GetChild(i).transform.position = Vector3.Lerp(transform.position, transform.GetChild(0).GetChild(i).transform.position, Time.deltaTime);
		    }
		}


	}

	void OnCollisionEnter2D(Collision2D collision){

		// stop controlling the circle
		touchedCircle = true;
		// also stop all movement of ball so it doesnt bounce off to another ball unintentionally
		GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

		// add to the move count score
		currentScore++;
		persistentObject.gameObject.SendMessage("HandleMoveScore", currentScore);

		// play swap music
		Camera.main.GetComponents<AudioSource>()[0].Play();

		// swap code:

		circleSkills = collision.transform.Find("circleSkills");

		List<Transform> playerChildren = new List<Transform>();
		List<Transform> circleChildren = new List<Transform>();
		List<Transform> toBeAppended = new List<Transform>();
		List<Transform> toBeDestroyed = new List<Transform>();
		List<Transform> finalplayer = new List<Transform>(); // all these temporary lists seemed to be needed because of destroying and reparenting elements on the fly, before garbage collection can take place

		// get player skills
		for(int i = 0; i < playerSkills.childCount; i++)
		{
			playerChildren.Add(playerSkills.GetChild(i));
			finalplayer.Add(playerSkills.GetChild(i));
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
					// play destroy music
					Camera.main.GetComponents<AudioSource>()[2].Play();
					toBeDestroyed.Add(skillInPlayer);
					toBeDestroyed.Add(skillInCircle);
					skillInPlayer.parent = null;
					skillInCircle.parent = null;
					finalplayer.Remove(skillInPlayer);
                    finalplayer.Remove(skillInCircle);
				}

			}
		}

		// swap remaining elements
		foreach(Transform child in playerChildren){
			child.parent = circleSkills;
			child.position = circleSkills.position + (offset += new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));
			offset = Vector3.zero;
			finalplayer.Remove(child);
		}
		foreach(Transform child in circleChildren){
			child.parent = playerSkills;
			child.position = playerSkills.position + (offset += new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));
			offset = Vector3.zero;
		}
		foreach(Transform child in toBeDestroyed){
			// mind you, this code will create errors if there is more then one element of a kind in the circle or player
			child.GetChild(0).GetComponent<ParticleSystem>().Play();
			Transform deathEffect = child.GetChild(0);
			deathEffect.parent = null;
			deathEffect.localScale = new Vector3(1, 1, 1);
			Destroy(child.gameObject);
		}


		//testing for the win condition
        bool solved = true;
 
        for (int i = 0; i < solution.transform.childCount; i++)
        {
            bool found = false;
            foreach (Transform child in finalplayer)
            {
                if (solution.transform.GetChild(i).GetComponent<SpriteRenderer>().color == child.GetComponent<SpriteRenderer>().color)
                {
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
            Debug.Log(finalplayer.Count + "," + solution.transform.childCount);
            if (finalplayer.Count == solution.transform.childCount)
            {
                Debug.Log("Game over");
                Invoke("LoadNextLevel", 2.5f);
            }
        }

	}
	public void LoadNextLevel(){

		// before loading next stage add current moves score to global score
		persistentObject.GetComponent<GameScript>().TotalMoves += currentScore;
		persistentObject.GetComponent<GameScript>().SaveLevelScore(currentScore);
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
			if(scoreFill.parent.parent.GetComponent<Slider>().maxValue > int.Parse(solution.transform.parent.Find("optimalScore").GetComponent<Text>().text)){
				tmpSingleMove.GetComponent<Image>().color = Color.red;	// if player crossed the optimal moves hreshold start marking new moves with red			
			}
			Transform tmpMoveSlot = Instantiate(scoreFill.parent.parent.Find("Background").Find("MoveSlot"));
			tmpMoveSlot.SetParent(scoreFill.parent.parent.Find("Background"));
			tmpMoveSlot.localScale = Vector3.one;
		}
	}
}
