using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuScript : MonoBehaviour {

	public Transform levelButton;
	Transform persistentObject;

	// Use this for initialization
	void Start () {

		persistentObject = GameObject.Find("PersistentObject").transform;
		string playerId = persistentObject.GetComponent<GameScript>().playerId;
		// on load of LevelMenu level show all available levels as buttons, as well as acompanying info about level score
		int allScenesNumber = SceneManager.sceneCountInBuildSettings;		
		
		Transform bt;
		for(int i = 0; i < allScenesNumber; i++){
			if(SceneManager.GetSceneAt(i).name.StartsWith("lvl")){ //cant be done this way because SceneManager functions work only for loaded scenes
				bt = Instantiate(levelButton);
				bt.GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(playerId+'_'+SceneManager.GetSceneAt(i).name).ToString();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
