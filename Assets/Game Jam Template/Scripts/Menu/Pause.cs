using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {


	// private ShowPanels showPanels;						//Reference to the ShowPanels script used to hide and show UI panels
	private bool isPaused;								//Boolean to check if the game is paused or not
	private StartOptions startScript;					//Reference to the StartButton script
	private Transform pausePanel;
	
	//Awake is called before Start()
	void Awake()
	{
		//Get a component reference to ShowPanels attached to this object, store in showPanels variable
		// showPanels = GetComponent<ShowPanels> ();
		//Get a component reference to StartButton attached to this object, store in startScript variable
		startScript = GetComponent<StartOptions> ();

		// get an object reference to the pause panel which is a child of this object
		pausePanel = transform.Find("PausePanel");
	}

	// Update is called once per frame
	void Update () {

		//Check if the Cancel button in Input Manager is down this frame (default is Escape key) and that game is not paused, and that we're not in main menu
		if (Input.GetButtonDown ("Cancel") && !isPaused /*&& !startScript.inMainMenu*/) 
		{
            Debug.Log("pausing");
			//Call the DoPause function to pause the game
			DoPause();
		} 
		//If the button is pressed and the game is paused and not in main menu
		else if (Input.GetButtonDown ("Cancel") && isPaused /*&& !startScript.inMainMenu*/) 
		{
			//Call the UnPause function to unpause the game
			UnPause ();
		}
	
	}


	public void DoPause()
	{
		//Set isPaused to true
		isPaused = true;
		//Set time.timescale to 0, this will cause animations and physics to stop updating
		Time.timeScale = 0;
		//call the ShowPausePanel function of the ShowPanels script
		// showPanels.ShowPausePanel ();
		pausePanel.gameObject.SetActive(true);
	}


	public void UnPause()
	{
		//Set isPaused to false
		isPaused = false;
		//Set time.timescale to 1, this will cause animations and physics to continue updating at regular speed
		Time.timeScale = 1;
		//call the HidePausePanel function of the ShowPanels script
		// showPanels.HidePausePanel ();
		pausePanel.gameObject.SetActive(false);

	}

	public void ReturnToMainMenu(){
		// play button click sound
		this.GetComponents<AudioSource>()[1].Play();

		//Set time.timescale to 1, this will cause animations and physics to continue updating at regular speed
		Time.timeScale = 1;

		// hide pause menu
		// showPanels.HidePausePanel ();
		// showPanels.ShowMenu ();
		
		// load main menu (used just to clear current level, there already is a main menu as a persistent object from the start of game)
		SceneManager.LoadScene("MainMenu");
	}


}
