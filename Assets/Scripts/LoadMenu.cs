using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour {

	public float loadPause = 9f;

	// Use this for initialization
	void Start () {
		Invoke("LoadNextLevel", loadPause);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LoadNextLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
