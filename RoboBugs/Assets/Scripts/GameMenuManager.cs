using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMenuState {
	PAUSED, PLAY, TERRARIUM
}

public class GameMenuManager : Singleton<GameMenuManager> {
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject terrariumMenu;
	[SerializeField] private GameObject playerHUD;
	[SerializeField] public Terrarium terrarium;
	[Space]
	[SerializeField] private GameMenuState _gameMenuState;

	/// <summary>
	/// The current menu state of the game
	/// </summary>
	public GameMenuState GameMenuState {
		get => _gameMenuState;
		set {
			_gameMenuState = value;

			// Set the correct menu to be active based on the menu state
			pauseMenu.SetActive(_gameMenuState == GameMenuState.PAUSED);
			terrariumMenu.SetActive(_gameMenuState == GameMenuState.TERRARIUM);
			playerHUD.SetActive(_gameMenuState == GameMenuState.PLAY);

			// Set the timescale if the game is paused
			Time.timeScale = (_gameMenuState != GameMenuState.PLAY ? 0f : 1f);
		}
	}

	private void OnValidate ( ) {
		terrarium = FindObjectOfType<Terrarium>( );
	}

	protected override void Awake ( ) {
		base.Awake( );

		OnValidate( );
	}

	private void Start ( ) {
		GameMenuState = GameMenuState.PLAY;
	}

	private void Update ( ) {
		// If the player presses escape, then pause the game
		// Alternatively this also quits out of the terrarium UI
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (GameMenuState == GameMenuState.PAUSED || GameMenuState == GameMenuState.TERRARIUM) {
				GameMenuState = GameMenuState.PLAY;
			} else {
				GameMenuState = GameMenuState.PAUSED;
			}
		}

		// If the player presses E while near the terrarium, open the terrarium UI
		if (Input.GetKeyDown(KeyCode.E) && terrarium.IsPlayerNear) {
			if (GameMenuState == GameMenuState.TERRARIUM) {
				GameMenuState = GameMenuState.PLAY;
			} else {
				GameMenuState = GameMenuState.TERRARIUM;
			}
		}
	}

	/// <summary>
	/// Set the game menu state. Used for adding Unity button functionality
	/// </summary>
	/// <param name="gameMenuState">The menu state to set the game to</param>
	public void SetGameMenuState (GameMenuState gameMenuState) {
		GameMenuState = gameMenuState;
	}

	/// <summary>
	/// Go to a scene based on its build index
	/// </summary>
	/// <param name="buildIndex">The build index of the scene to go to</param>
	public void GoToScene (int buildIndex) {
		SceneManager.LoadScene(buildIndex);
	}

	/// <summary>
	/// Quit the game
	/// </summary>
	public void Quit ( ) {
		Application.Quit( );
	}
}
