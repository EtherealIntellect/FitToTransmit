using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour {

	float totalMoves = 0;
	public float TotalMoves{ get{ return totalMoves; } set{ totalMoves = value; } }

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}
	// Use this for initialization
	void Start () {

		// Restore data from local storage so user continues session

		// assign a delegate so we know when scene is changed
		SceneManager.sceneLoaded += OnSceneLoaded;
		Debug.Log("just how many time is this thing called anyway?");
		
	}

	void OnDestroy(){
		// SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
	// Update is called once per frame
	void Update () {

	}

	// called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "Highscores"){
        	GameObject.Find("Highscore").GetComponent<Text>().text = GameObject.Find("Highscore").GetComponent<Text>().text + ": " + totalMoves;
        }
        // SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
