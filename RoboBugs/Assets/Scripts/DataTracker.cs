using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		if (!player.characterSwapFlag) {
			atlasPlaytime += Time.deltaTime;
		}

		// Increment the playtime of the current loadout
		BugLoadoutTimes[CurrentLoadout] += Time.deltaTime;
	}

	/// <summary>
	/// Save all of the data collected to a file
	/// </summary>
	public void SaveDataToFile ( ) {
		// Variables that hold the data that was read from the file
		Dictionary<string, float> readLoadoutValues = new Dictionary<string, float>( );
		float readAtlasTime = 0f;
		float readPrometheusTime = 0f;
		string filePath = $"{Application.dataPath}/PlayerData-{SceneManager.GetActiveScene( ).name}.txt";

		// If the text file does not exist, then create a new one
		if (!File.Exists(filePath)) {
			File.Create(filePath);
		}

		// Read old accumulated values from the file
		using (StreamReader sr = new StreamReader(filePath)) {
			// Read the current line
			string[ ] line = sr.ReadLine( ).Split("");

			// Read values from the file
			// A > indicates a loadout, and an = indicates a character playtime
			if (line[0] == ">") {
				readLoadoutValues.Add(line[1], float.Parse(line[2].Replace("s", "")));
			} else if (line[0] == "=") {
				if (line[1] == "Atlas") {
					readAtlasTime = float.Parse(line[2].Replace("s", ""));
				} else if (line[1] == "Prometheus") {
					readPrometheusTime = float.Parse(line[2].Replace("s", ""));
				}
			}
		}

		// Add in the times from this level
		foreach (KeyValuePair<string, float> loadout in BugLoadoutTimes) {
			if (!readLoadoutValues.ContainsKey(loadout.Key)) {
				readLoadoutValues.Add(loadout.Key, loadout.Value);
			} else {
				readLoadoutValues[loadout.Key] += loadout.Value;
			}
		}

		// Clear the file so we can write to it again
		File.WriteAllText(filePath, String.Empty);

		// Write new values to the file
		using (StreamWriter sw = new StreamWriter(filePath, true)) {
			// Write all of the loadouts sorted into time order
			sw.WriteLine("Amount of time each bug loadout was used:");
			foreach (KeyValuePair<string, float> loadout in readLoadoutValues) {
				sw.WriteLine($"> {loadout.Key} {loadout.Value:0.0}s");
			}
			sw.WriteLine("E - Empty, R - Red Bug, Y - Yellow Bug, B - Blue Bug");
			sw.WriteLine("Formatted as AAA-PPP, where A is Atlas's bugs and P is Prometheus' bugs\n");

			// Write the playtime of each character
			float prometheusPlaytime = Time.timeSinceLevelLoad - atlasPlaytime;
			float toalPlaytime = readAtlasTime + readPrometheusTime + Time.timeSinceLevelLoad;
			atlasPlaytime += readAtlasTime;
			prometheusPlaytime += readPrometheusTime;
			sw.WriteLine("Amount of time each character was used:");
			sw.WriteLine($"= Prometheus {atlasPlaytime:0.0}s ({(atlasPlaytime / toalPlaytime * 100f):0.00}%)");
			sw.WriteLine($"= Atlas {prometheusPlaytime:0.0}s ({(prometheusPlaytime / toalPlaytime * 100f):0.00}%)\n");
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
