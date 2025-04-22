using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Terrarium : MonoBehaviour {
	[SerializeField] private Canvas canvas;
	[SerializeField] private TextMeshProUGUI redBugsCollectedText;
	[SerializeField] private TextMeshProUGUI yellowBugsCollectedText;
	[SerializeField] private TextMeshProUGUI blueBugsCollectedText;
	[Space]
	[SerializeField] private int _redBugsCollected;
	[SerializeField] private int _yellowBugsCollected;
	[SerializeField] private int _blueBugsCollected;

	/// <summary>
	/// The current amount of red bugs collected
	/// </summary>
	public int RedBugsCollected {
		get => _redBugsCollected;
		set {
			_redBugsCollected = value;
			redBugsCollectedText.text = _redBugsCollected.ToString();
		}
	}

	/// <summary>
	/// The current amount of yellow bugs collected
	/// </summary>
	public int YellowBugsCollected {
		get => _yellowBugsCollected;
		set {
			_yellowBugsCollected = value;
			yellowBugsCollectedText.text = _yellowBugsCollected.ToString();
		}
	}

	/// <summary>
	/// The current amount of blue bugs collected
	/// </summary>
	public int BlueBugsCollected {
		get => _blueBugsCollected;
		set {
			_blueBugsCollected = value;
			blueBugsCollectedText.text = _blueBugsCollected.ToString();
		}
	}

	private void Start ( ) {
		// Update all text objects to match the default collected bugs
		RedBugsCollected = RedBugsCollected;
		YellowBugsCollected = YellowBugsCollected;
		BlueBugsCollected = BlueBugsCollected;

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
