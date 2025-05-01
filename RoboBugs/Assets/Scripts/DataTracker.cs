using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTracker : MonoBehaviour {
	[SerializeField] private List<int> _bugsEquipped;
	[SerializeField] private float atlasPlaytime;
	[SerializeField] private bool _isAtlas;

	/// <summary>
	/// The current count of how many of each type of bug was equipped
	/// </summary>
	public List<int> BugsEquipped { get => _bugsEquipped; private set => _bugsEquipped = value; }

	/// <summary>
	/// Whether or not the player is currently playing as atlas
	/// </summary>
	public bool IsAtlas { get => _isAtlas; set => _isAtlas = value; }

	private void OnValidate ( ) {
		BugsEquipped = new List<int>( ) { 0, 0, 0 };
	}

	private void Awake ( ) {
		OnValidate( );
	}

	private void Update ( ) {
		if (IsAtlas) {
			atlasPlaytime += Time.deltaTime;
		}
	}

	public void SaveDataToFile ( ) {

	}
}
