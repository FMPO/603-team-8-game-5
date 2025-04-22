using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Terrarium : MonoBehaviour {
	[SerializeField] private Canvas canvas;
	[SerializeField] private List<StorageSlot> storageSlots;

	/// <summary>
	/// The current amount of red bugs collected
	/// </summary>
	public int RedBugsCollected {
		get => storageSlots[(int) BugType.RED].BugCount;
		set => storageSlots[(int) BugType.RED].BugCount = value;
	}

	/// <summary>
	/// The current amount of yellow bugs collected
	/// </summary>
	public int YellowBugsCollected {
		get => storageSlots[(int) BugType.YELLOW].BugCount;
		set => storageSlots[(int) BugType.YELLOW].BugCount = value;
	}

	/// <summary>
	/// The current amount of blue bugs collected
	/// </summary>
	public int BlueBugsCollected {
		get => storageSlots[(int) BugType.BLUE].BugCount;
		set => storageSlots[(int) BugType.BLUE].BugCount = value;
	}

	private void Start ( ) {
		// Disable the canvas when the game starts
		canvas.gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D (Collider2D collision) {
		// If colliding with the player, display the terrarium inventory
		// If colliding with a bug that is stunned, destroy the bug and add it to the terrarium
		if (collision.CompareTag("Player")) {
			canvas.gameObject.SetActive(true);
		} else if (collision.CompareTag("Bug")) {
			// Get a reference to the bug component
			BugMovement bugRef = collision.GetComponent<BugMovement>( );

			// Add to the counter based on the bug type
			if (bugRef.BugState == BugState.STUNNED) {
				switch (bugRef.BugType) {
					case BugType.RED:
						RedBugsCollected++;
						break;
					case BugType.YELLOW:
						YellowBugsCollected++;
						break;
					case BugType.BLUE:
						BlueBugsCollected++;
						break;
				}

				// Destroy the bug
				Destroy(collision.gameObject);
			}
		}
	}

	private void OnTriggerExit2D (Collider2D collision) {
		// If stopped colliding with the player, disable the display of the terrarium inventory
		if (collision.CompareTag("Player")) {
			canvas.gameObject.SetActive(false);
		}
	}
}
