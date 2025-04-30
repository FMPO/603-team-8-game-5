using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerrariumMenuManager : MonoBehaviour {
	[SerializeField] private List<StorageSlot> atlasUpgradeSlots;
	[SerializeField] private List<StorageSlot> prometheusUpgradeSlots;
	[SerializeField] private List<StorageSlot> storageSlots;
	[SerializeField] private TextMeshProUGUI quotaText;
	[SerializeField] private TextMeshProUGUI doorUnlockedText;
	[SerializeField] private GameObject doorObject;
	[Space]
	[SerializeField, Range(0, 50)] private int quota;

	private void OnValidate ( ) {
		// Find the door in the scene without having to manually drag the object each time
		doorObject = GameObject.Find("Door");
	}

	private void Awake ( ) {
		OnValidate( );
	}

	/// <summary>
	/// Add a bug of a specific type to the storage
	/// </summary>
	/// <param name="bugType">The bug type to add</param>
	public void AddBugToStorage (BugType bugType) {
		// Increment the correct storage slot bug count
		storageSlots[(int) bugType].BugCount++;

		// Update the quota text since the storage has been changed
		UpdateQuotaText( );
	}

	/// <summary>
	/// Update the quota text and unlock the door if the quota has been met
	/// </summary>
	private void UpdateQuotaText ( ) {
		// Get the total amount of bugs the player has collected
		int totalBugs = storageSlots[(int) BugType.RED].BugCount + storageSlots[(int) BugType.YELLOW].BugCount + storageSlots[(int) BugType.BLUE].BugCount;

		// Set the quota text
		quotaText.text = $"Bug Goal: {totalBugs} / {quota}";

		// If the player has reached the quota, unlock the door
		bool hasCompletedQuota = (totalBugs >= quota);
		doorUnlockedText.gameObject.SetActive(hasCompletedQuota);
		doorObject.SetActive(!hasCompletedQuota);
	}
}
