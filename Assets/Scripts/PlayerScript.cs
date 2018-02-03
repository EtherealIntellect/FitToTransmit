using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {


	public Vector3 offset;
	public Transform playerSkills;
	Transform circleSkills;
	

	public Vector2 velocity;
    Rigidbody2D rb2D;
    bool touchedCircle = false;
    bool touchedPoint = false;
    Vector3 lastClickedPosition;

	public float range = 0.5f;
	public float smoothValue = 1;

	public GameObject solution;

	public int currentScore = 0;
	public Text ScoreText;

	// Use this for initialization
	void Start () {
		offset = new Vector3(0,0,0);
		playerSkills = transform.Find("playerSkills");
		rb2D = GetComponent<Rigidbody2D>();

		solution = GameObject.Find("solution");

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
				Debug.Log(Vector3.Distance(transform.TransformPoint(lastClickedPosition), transform.position));	
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
	    	// rb2D.MovePosition(Vector2.Lerp(rb2D.position, lastClickedPosition, Time.deltaTime*smoothValue));
	    	// v3 = Camera.main.ScreenToWorldPoint(v3);
	    	lastClickedPosition = transform.InverseTransformPoint(lastClickedPosition);
	    	rb2D.AddForce(lastClickedPosition * smoothValue, ForceMode2D.Impulse);
	    	// transform.Velocity = Vector3.Lerp(transform.position , Camera.main.ScreenToWorldPoint(v3), Time.deltaTime*smoothValue);
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
		    float dist = Vector3.Distance(transform.GetChild(0).GetChild(i).transform.position, transform.position);
		    //Debug.Log("the distance is ===== " + dist);
		    // if (dist > 1f)
		    // {
		        transform.GetChild(0).GetChild(i).transform.position = Vector3.MoveTowards(transform.position, transform.GetChild(0).GetChild(i).transform.position, Time.deltaTime * 8);
		    // }
		}


	}

	void OnCollisionEnter2D(Collision2D collision){

		// stop controlling the circle
		touchedCircle = true;
		// also stop all movement of ball so it doesnt bounce off to another ball uninentionally
		GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

		// add to the "score"
		currentScore++;
		ScoreText.text = currentScore.ToString();

		// play swap music
		Camera.main.GetComponents<AudioSource>()[0].Play();

		circleSkills = collision.transform.Find("circleSkills");

		List<Transform> playerChildren = new List<Transform>();
		List<Transform> circleChildren = new List<Transform>();
		List<Transform> toBeAppended = new List<Transform>();
		List<Transform> toBeDestroyed = new List<Transform>();
		List<Transform> finalplayer = new List<Transform>();

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

				Color circleSkillColor = skillInCircle.GetComponent<SpriteRenderer>().color;
				Color playerSkillColor = skillInPlayer.GetComponent<SpriteRenderer>().color;
				if(circleSkillColor == playerSkillColor)
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
			child.GetChild(0).GetComponent<ParticleSystem>().Play();
			Transform deathEffect = child.GetChild(0);
			deathEffect.parent = null;
			deathEffect.localScale = new Vector3(1, 1, 1);
			// child.position =  Vector3.MoveTowards(child.position, child.position + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), 2000);
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
 
 
            //  float dist = Vector3.Distance(transform.GetChild(0).GetChild(i).transform.position, transform.position);
            //  Debug.Log("the distance is ===== " + dist);
            // if (dist > 0.85f)
            // {
            //    transform.GetChild(0).GetChild(i).transform.position = Vector3.MoveTowards(transform.position, transform.GetChild(0).GetChild(i).transform.position, 0.80f);
            //}
            // }
            // child.GetComponent<SpriteRenderer>().color = circleSkills;
            // child.position = circleSkills.position + (offset += new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));
            // offset = Vector3.zero;
        }
        if (solved==true)
        {
            Debug.Log("Half solved");
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
		GameObject.Find("PersistentObject").GetComponent<GameScript>().TotalMoves += currentScore;

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void RestartLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
