using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour {
	[SerializeField] private Image bugImage;
	[SerializeField] private TextMeshProUGUI bugCountText;
	[SerializeField] private Terrarium terrarium;
	[Space]
	[SerializeField] private BugType _bugType;
	[SerializeField] private int _bugCount;
	[SerializeField] private bool _hasBug;
	[SerializeField] private bool _hideBugCount;
	[SerializeField] private bool canPlayerPlaceBug;
	[SerializeField] private bool canPlayerRemoveBug;
	[SerializeField] private bool canOverwriteBugType;

	/// <summary>
	/// The bug type being stored in this storage slot
	/// </summary>
	public BugType BugType {
		get => _bugType;
		set {
			_bugType = value;
			bugImage.sprite = terrarium.BugUISprites[(int) _bugType];
		}
	}

	/// <summary>
	/// The current bug count of this storage slot
	/// </summary>
	public int BugCount {
		get => _bugCount;
		set {
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
		terrarium = FindObjectOfType<Terrarium>( );

		if (terrarium != null) {
			BugType = BugType;
			BugCount = BugCount;
			HasBug = HasBug;
			HideBugCount = HideBugCount;
		}
	}

	private void Awake ( ) {
		OnValidate( );
	}

	public void OnPointerEnter ( ) {
		Debug.Log("On Pointe rEnter");
		//terrarium.SelectedStorageSlot = this;
	}

	public void OnPointerExit ( ) {
		Debug.Log("On Pointe Exit");
		//if (terrarium.SelectedStorageSlot == this) {
		//	terrarium.SelectedStorageSlot = null;
		//}
	}

	public void OnPointerDown ( ) {
		Debug.Log("On Pointe Down");
		//// If the player cannot remove bugs from this storage slot, then return
		//// If there are no bugs to remove, then return
		//if (!canPlayerRemoveBug || BugCount <= 0) {
		//	return;
		//}

		//// Set the held bug type
		//terrarium.IsHoldingBug = true;
		//terrarium.HeldBugType = BugType;
		//terrarium.HeldFromStorageSlot = this;
		//BugCount -= 1;
	}

	public void OnPointerUp ( ) {
		Debug.Log("On Pointe Up");
		//// If the player cannot place bugs from this storage slot, then return
		//if (!canPlayerPlaceBug || (!canOverwriteBugType && terrarium.HeldBugType != BugType)) {
		//	terrarium.IsHoldingBug = false;
		//	terrarium.HeldFromStorageSlot.BugCount += 1;
		//	terrarium.HeldFromStorageSlot = null;
		//}

		//terrarium.IsHoldingBug = false;
		//BugType = terrarium.HeldBugType;
		//terrarium.HeldFromStorageSlot = null;
		//HasBug = true;
		//BugCount += 1;
	}
}
