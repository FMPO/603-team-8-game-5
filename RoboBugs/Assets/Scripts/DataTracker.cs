using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataTracker : MonoBehaviour {
	[SerializeField] private Player player;
	[SerializeField] private TerrariumMenuManager terrariumMenuManager;

	private float atlasPlaytime;

	public string CurrentLoadout {
		get => _currentLoadout;
		set {
			_currentLoadout = value;

			// If the loadout has not been selected yet, then add it as a loadout
			if (!BugLoadoutTimes.ContainsKey(_currentLoadout)) {
				BugLoadoutTimes.Add(_currentLoadout, 0);
			}

			Debug.Log("Current Loadout Set To: " + _currentLoadout);
		}
	}
	private string _currentLoadout;

	/// <summary>
	/// The current times that the player has had each loadout equipped
	/// </summary>
	public Dictionary<string, float> BugLoadoutTimes { get; private set; }

	// It is important to call this in Start() because the terrarium menu manager gets loaded in inside the Awake() function
	private void Start ( ) {
		BugLoadoutTimes = new Dictionary<string, float>( );
		terrariumMenuManager = FindAnyObjectByType<TerrariumMenuManager>( );

		UpdateCurrentLoadoutString( );
	}

	private void Update ( ) {
		// If the player is playing as atlas, then increase the atlas playtime
		if (player.characterSwapFlag) {
			atlasPlaytime += Time.deltaTime;
		}

		// Increment the playtime of the current loadout
		BugLoadoutTimes[CurrentLoadout] += Time.deltaTime;
	}

	/// <summary>
	/// Save all of the data collected to a file
	/// </summary>
	public void SaveDataToFile ( ) {
		using (StreamWriter sw = new StreamWriter($"{Application.dataPath}/{DateTime.Now.ToString( ).Replace("/", "-").Replace(":", "-").Replace(" ", "-")}.txt", true)) {
			// Write all of the loadouts sorted into time order
			sw.WriteLine("Amount of time each bug loadout was used: ");
			foreach (KeyValuePair<string, float> loadout in BugLoadoutTimes) {
				sw.WriteLine($"> {loadout.Key} - {loadout.Value:0.0}s");
			}
			sw.WriteLine("E - Empty, R - Red Bug, Y - Yellow Bug, B - Blue Bug");
			sw.WriteLine("Formatted as AAA-PPP, where A is Atlas's bugs and P is Prometheus' bugs\n");

			// Write the playtime of each character
			float prometheusPlaytime = Time.timeSinceLevelLoad - atlasPlaytime;
			sw.WriteLine($"Atlas Playtime: {atlasPlaytime:0.0}s ({(atlasPlaytime / Time.timeSinceLevelLoad):0.00}%)");
			sw.WriteLine($"Atlas Playtime: {prometheusPlaytime:0.0}s ({(prometheusPlaytime / Time.timeSinceLevelLoad):0.00}%)\n");

			// Based on the data, write an interpretation about how the player likes to play
		}
	}

	/// <summary>
	/// Based on the current upgrade slots, get a string that represents the current loadout
	/// </summary>
	public void UpdateCurrentLoadoutString ( ) {
		string loadout = "";

		foreach (StorageSlot slot in terrariumMenuManager.AtlasUpgradeSlots) {
			// If the slot does not have a bug, then add an E
			if (!slot.HasBug) {
				loadout += "E";
				continue;
			}

			// If the slot does have a bug, then add a letter based on the type of bug
			switch (slot.BugType) {
				case BugType.RED:
					loadout += "R";
					break;
				case BugType.YELLOW:
					loadout += "Y";
					break;
				case BugType.BLUE:
					loadout += "B";
					break;
			}
		}

		loadout += "-";

		foreach (StorageSlot slot in terrariumMenuManager.PrometheusUpgradeSlots) {
			// If the slot does not have a bug, then add an E
			if (!slot.HasBug) {
				loadout += "E";
				continue;
			}

			// If the slot does have a bug, then add a letter based on the type of bug
			switch (slot.BugType) {
				case BugType.RED:
					loadout += "R";
					break;
				case BugType.YELLOW:
					loadout += "Y";
					break;
				case BugType.BLUE:
					loadout += "B";
					break;
			}
		}

		CurrentLoadout = loadout;
	}
}
