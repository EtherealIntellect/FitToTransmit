using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuScript : MonoBehaviour {

	public Transform levelButton;
	public Transform levelScore;
	Transform persistentObject;

	// Use this for initialization
	void Start () {

		// on load of LevelMenu level show all available levels as buttons, as well as acompanying info about level score
		// persistentObject = GameObject.FindWithTag("GameController").transform;
		Transform canvas = GameObject.Find("Canvas").transform;
		// string playerId = persistentObject.GetComponent<GameScript>().playerId;
		string playerId = "unidentified_player";
		
		int allScenesCount = SceneManager.sceneCountInBuildSettings;		

		Transform bt;
		for(int i = 0; i < allScenesCount; i++){
			// we check if lvl name contains "lvl" in it because that is how we determine if its a playable level
			if(SceneUtility.GetScenePathByBuildIndex(i).Contains("lvl")){

				// some string manipulations to extract the name of the level
				int endOfName = SceneUtility.GetScenePathByBuildIndex(i).LastIndexOf(".");
				int startOfName = SceneUtility.GetScenePathByBuildIndex(i).IndexOf("lvl");
				string sceneName = SceneUtility.GetScenePathByBuildIndex(i).Substring(startOfName, endOfName-startOfName);

				// create buttons for every playable level
				bt = Instantiate(levelButton);
				bt.SetParent(canvas.GetChild(0).Find("LevelButtons"));
				bt.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
				// assign appropriate data to the created buttons
				bt.GetComponentInChildren<Text>().text = sceneName;

				// create score ui for every playable level
				bt = Instantiate(levelButton);
				bt.SetParent(canvas.GetChild(0).Find("LevelScores"));
				bt.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
				// assign appropriate data to the created buttons
				bt.GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(playerId+'_'+sceneName).ToString();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
