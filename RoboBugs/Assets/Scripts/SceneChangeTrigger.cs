using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour {
	[SerializeField] private BoxCollider2D triggerCollider;
	[SerializeField] private int loadSceneBuildIndex;

	private void OnTriggerEnter2D (Collider2D collision) {
		// If the player collides with the trigger, then load the scene with the specified build index
		if (collision.CompareTag("Player")) {
			FindAnyObjectByType<DataTracker>( ).SaveDataToFile( );

			SceneManager.LoadScene(loadSceneBuildIndex);
		}
	}
}
