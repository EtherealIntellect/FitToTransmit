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

		Transform btn;
		Transform score;
		for(int i = 0; i < allScenesCount; i++){
			// we check if lvl name contains "lvl" in it because that is how we determine if its a playable level
			if(SceneUtility.GetScenePathByBuildIndex(i).Contains("lvl")){

				// some string manipulations to extract the name of the level
				int endOfName = SceneUtility.GetScenePathByBuildIndex(i).LastIndexOf(".");
				int startOfName = SceneUtility.GetScenePathByBuildIndex(i).LastIndexOf("/") + 1;
				string sceneName = SceneUtility.GetScenePathByBuildIndex(i).Substring(startOfName, endOfName-startOfName);

				// create buttons for every playable level
				btn = Instantiate(levelButton);
				btn.SetParent(canvas.GetChild(0).Find("LevelButtons"));
				btn.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
				// assign appropriate data to the created buttons
				btn.GetComponentInChildren<Text>().text = sceneName;

				// create score ui for every playable level
				score = Instantiate(levelScore);
				score.SetParent(canvas.GetChild(0).Find("LevelScores"), false);
				// GameObject movesCounter = GameObject.Find("Moves Counter");
				score.GetComponent<Slider>().maxValue = PlayerPrefs.GetInt(playerId+'_'+sceneName+"_optimalScore");

				Transform bgFill = score.transform.Find("Background");
				Transform tmpMoveSlot;
				for(int j = 0; j < score.GetComponent<Slider>().maxValue-1; j++)
				{
					tmpMoveSlot = Instantiate(bgFill.Find("MoveSlot"));
					tmpMoveSlot.SetParent(bgFill);
					tmpMoveSlot.localScale = Vector3.one;
				}

				Transform fgFill = score.transform.Find("Fill Area").Find("Fill");
				Transform tmpSingleMove;
				for(int j = 0; j < PlayerPrefs.GetInt(playerId+'_'+sceneName+"_playerScore"); j++)
				{
					if (score.GetComponent<Slider>().value >= score.GetComponent<Slider>().maxValue){
						score.GetComponent<Slider>().maxValue++;
					}
					if(score.GetComponent<Slider>().value != 0){
						tmpSingleMove = Instantiate(fgFill.Find("SingleMove"));
						tmpSingleMove.SetParent(fgFill);
						tmpSingleMove.localScale = Vector3.one;

						if(score.GetComponent<Slider>().maxValue > PlayerPrefs.GetInt(playerId+'_'+sceneName+"_optimalScore")){
							tmpSingleMove.GetComponent<Image>().color = Color.red;	// if player crossed the optimal moves threshold start marking new moves with red			
						}
					}
			
					score.GetComponent<Slider>().value++;

				}
				// btn.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
				// assign appropriate data to the created buttons
				// btn.GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(playerId+'_'+sceneName).ToString();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
