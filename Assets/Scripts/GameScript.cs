using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour {

	// delete this object from all other levels except the very first one when building it

	float totalMoves = 0;
	public float TotalMoves{ get{ return totalMoves; } set{ totalMoves = value; } }
	bool instantiated = false;
	public string playerId = "unidentified_player";
	public int lastLevel;

	void Awake(){
		
		DontDestroyOnLoad(transform.gameObject);

		// assign a delegate so we know when scene is changed
		if (!instantiated){
			SceneManager.sceneLoaded += OnSceneLoaded;
			
			instantiated = true;
		}

	}

	// Use this for initialization
	void Start () {

		// Restore data from local storage so user continues session



		// Debug.Log("Start of persistent object"); // this runs only once in the life of a persistent object regardless of how many levels it got through


		
	}

	void OnDestroy(){
		// SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
	// Update is called once per frame
	void Update () {

		// global shortcuts
		if(Input.GetKeyDown("escape")){
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		// manage score
		

	}

	void HandleMoveScore(int currentMoveCount){
		// player moved so adjust score appropriately
		GameObject movesCounter = GameObject.Find("Moves Counter");
		if (movesCounter.GetComponent<Slider>().value >= movesCounter.GetComponent<Slider>().maxValue){
			movesCounter.GetComponent<Slider>().maxValue++;
		}
		movesCounter.GetComponent<Slider>().value++;
	}

	// save the score the player achieved for the passed level
	public void SaveLevelScore(int score){
		PlayerPrefs.SetInt(playerId+'_'+SceneManager.GetActiveScene().name, score);
	}



	// called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    	// if this is the highscores level, show high scores
        if(SceneManager.GetActiveScene().name == "Highscores"){
        	GameObject.Find("Highscore").GetComponent<Text>().text = GameObject.Find("Highscore").GetComponent<Text>().text + ": " + totalMoves;
        }
        // SceneManager.sceneLoaded -= OnSceneLoaded;

        // if this is a normal level and not an intro or menu then initialize a few things
        if (GameObject.Find("solution")){
        	GameObject movesCounter = GameObject.Find("Moves Counter");
        	movesCounter.GetComponent<Slider>().maxValue = int.Parse(GameObject.Find("solution").transform.Find("optimalScore").GetComponent<Text>().text);
        	Transform bgFill = movesCounter.transform.Find("Background");
        	Transform tmpMoveSlot;
        	for(int i = 0; i < movesCounter.GetComponent<Slider>().maxValue-1; i++)
        	{
        		tmpMoveSlot = Instantiate(bgFill.Find("MoveSlot"));
        		tmpMoveSlot.SetParent(bgFill);
        		tmpMoveSlot.localScale = Vector3.one;
        	}
        	// Debug.Log("Start of level"); // this does indeed run at tthe start of level even if it is first level
        }
    }
}
