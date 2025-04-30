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
	[Space]
	[SerializeField, Range(0, 50)] private int quota;

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
		if (totalBugs >= quota) {
			doorUnlockedText.gameObject.SetActive(true);
		}
	}
}
