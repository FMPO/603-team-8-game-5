using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Terrarium : MonoBehaviour {
	[SerializeField] public TerrariumMenuManager terrariumMenuManager;
	[SerializeField] private GameObject interactCanvas;
	[SerializeField] private bool _isPlayerNear;

	/// <summary>
	/// Whether or not the player is near enough to the terrarium
	/// </summary>
	public bool IsPlayerNear {
		get => _isPlayerNear;
		private set {
			_isPlayerNear = value;
			interactCanvas.SetActive(_isPlayerNear);
		}
	}

	private void OnValidate ( ) {
		terrariumMenuManager = FindObjectOfType<TerrariumMenuManager>( );
	}

	private void Awake ( ) {
		OnValidate( );
	}

	private void Start ( ) {
		// Make sure the interact canvas stays off at the start
		IsPlayerNear = false;
	}

	private void OnTriggerEnter2D (Collider2D collision) {
		// If colliding with the player, display the terrarium inventory
		// If colliding with a bug that is stunned, destroy the bug and add it to the terrarium
		if (collision.CompareTag("Player")) {
			IsPlayerNear = true;
		}
		
		if (collision.CompareTag("Bug")) {
			// Get a reference to the bug component
			BugMovement bugRef = collision.GetComponent<BugMovement>( );

			// Only add the bug to the storage if it is stunned
			if (bugRef.BugState == BugState.STUNNED) {
				// Add the bug to the terrarium storage
				terrariumMenuManager.AddBugToStorage(bugRef.BugType);

				// Destroy the bug
				Destroy(collision.gameObject);
			}
		}
	}

	private void OnTriggerExit2D (Collider2D collision) {
		// If stopped colliding with the player, disable the display of the terrarium inventory
		if (collision.CompareTag("Player")) {
			IsPlayerNear = false;
		}
	}
}
