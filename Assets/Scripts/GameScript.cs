using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour {

	float totalMoves = 0;
	public float TotalMoves{ get{ return totalMoves; } set{ totalMoves = value; } }
	bool instantiated = false;

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}
	// Use this for initialization
	void Start () {

		// Restore data from local storage so user continues session

		// assign a delegate so we know when scene is changed
		if (!instantiated){
			SceneManager.sceneLoaded += OnSceneLoaded;
			
			instantiated = true;
		}
		
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

	}

	// called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "Highscores"){
        	GameObject.Find("Highscore").GetComponent<Text>().text = GameObject.Find("Highscore").GetComponent<Text>().text + ": " + totalMoves;
        	Debug.Log("just how many time is this thing called anyway?");
        }
        // SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
