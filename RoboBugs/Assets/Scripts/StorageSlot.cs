using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour {
	[SerializeField] private Image bugImage;
	[SerializeField] private TextMeshProUGUI bugCountText;
	[SerializeField] private TerrariumMenuManager terrariumMenuManager;
	[Space]
	[SerializeField] private BugType _bugType;
	[SerializeField] private int _bugCount;
	[SerializeField] private bool _hasBug;
	[SerializeField] private bool _hideBugCount;
	[SerializeField] private int _maxBugCount;
	[SerializeField] private bool keepImageOn;
	[SerializeField] public bool CanPlayerPlaceBug;
	[SerializeField] public bool CanPlayerRemoveBug;
	[SerializeField] public bool CanOverwriteBugType;

	/// <summary>
	/// The maximum bug count that this storage slot can have
	/// </summary>
	public int MaxBugCount { get => _maxBugCount; private set => _maxBugCount = value; }

	/// <summary>
	/// The bug type being stored in this storage slot
	/// </summary>
	public BugType BugType {
		get => _bugType;
		set {
			_bugType = value;
			bugImage.sprite = terrariumMenuManager.BugUISprites[(int) _bugType];
		}
	}

	/// <summary>
	/// The current bug count of this storage slot
	/// </summary>
	public int BugCount {
		get => _bugCount;
		set {
			// Check to see if this storage slot now has a bug
			if (_bugCount == 0 && value > 0) {
				HasBug = true;
			} else if (_bugCount > 0 && value == 0 && !keepImageOn) {
				HasBug = false;
			}

			_bugCount = value;
			bugCountText.text = _bugCount.ToString( );
		}
	}

	/// <summary>
	/// Whether or not this storage slot has a bug
	/// </summary>
	public bool HasBug {
		get => _hasBug;
		set {
			_hasBug = value;
			bugImage.enabled = _hasBug;
		}
	}

	/// <summary>
	/// Whether or not to hide the bug count on this storage slot
	/// </summary>
	public bool HideBugCount {
		get => _hideBugCount;
		set {
			_hideBugCount = value;
			bugCountText.enabled = !_hideBugCount;
		}
	}

	private void OnValidate ( ) {
		terrariumMenuManager = FindObjectOfType<TerrariumMenuManager>( );

		if (terrariumMenuManager != null) {
			BugType = BugType;
			BugCount = BugCount;
			HasBug = HasBug;
			HideBugCount = HideBugCount;
		}
	}

	private void Awake ( ) {
		OnValidate( );
	}

	public void OnPointerEnter () {
		terrariumMenuManager.ToStorageSlot = this;
	}

	public void OnPointerExit ( ) {
		if (terrariumMenuManager.ToStorageSlot == this) {
			terrariumMenuManager.ToStorageSlot = null;
		}
	}

	public void OnPointerDown ( ) {
		Debug.Log("OnPointerDown - " + gameObject.name);

		// If the player cannot remove a bug, then return
		if (!CanPlayerRemoveBug) {
			return;
		}

		// If there are no bugs in the storage slot, then return as well
		if (!HasBug || BugCount == 0) {
			return;
		}

		terrariumMenuManager.IsHoldingBug = true;
		terrariumMenuManager.HeldBugType = BugType;
		BugCount--;
		terrariumMenuManager.HeldFromStorageSlot = this;
	}

	public void OnPointerUp ( ) {
		Debug.Log("OnPointerUp - " + gameObject.name);

		// If a bug cannot be placed in this storage slot, then return
		bool noStorageSlot = terrariumMenuManager.ToStorageSlot == null || !terrariumMenuManager.ToStorageSlot.CanPlayerPlaceBug;

		// If there are already the max bugs in the storage slot, then return
		// Make sure that the slot cannot overwrite the bug type though because then the bug count does not matter
		bool maxBugsReached = terrariumMenuManager.ToStorageSlot.BugCount == terrariumMenuManager.ToStorageSlot.MaxBugCount && !terrariumMenuManager.ToStorageSlot.CanOverwriteBugType;

		// If the bug type does not match and it can't overwrite it, then return
		bool wrongBugType = terrariumMenuManager.ToStorageSlot.HasBug && !terrariumMenuManager.ToStorageSlot.CanOverwriteBugType && BugType != terrariumMenuManager.ToStorageSlot.BugType;
		
		if (noStorageSlot || maxBugsReached || wrongBugType) {
			terrariumMenuManager.IsHoldingBug = false;
			if (terrariumMenuManager.HeldFromStorageSlot != null) {
				terrariumMenuManager.HeldFromStorageSlot.BugCount++;
			}
			terrariumMenuManager.HeldFromStorageSlot = null;

			return;
		}

		// Complete the action and place the bug in this storage slot
		terrariumMenuManager.IsHoldingBug = false;
		terrariumMenuManager.ToStorageSlot.BugCount++;
		terrariumMenuManager.ToStorageSlot.BugType = BugType;
		terrariumMenuManager.HeldFromStorageSlot = null;

		// Update any effects since the bugs were moved
		terrariumMenuManager.UpdateEffects( );
	}
}
