using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour, IPointerDownHandler {
	[SerializeField] private Image bugImage;
	[SerializeField] private TextMeshProUGUI bugCountText;
	[SerializeField] private Terrarium terrarium;
	[Space]
	[SerializeField] private BugType _bugType;
	[SerializeField] private int _bugCount;
	[SerializeField] private bool _hasBug;
	[SerializeField] private bool _hideBugCount;

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

		BugType = BugType;
		BugCount = BugCount;
		HasBug = HasBug;
		HideBugCount = HideBugCount;
	}

	public void OnPointerDown (PointerEventData eventData) {
		Debug.Log("HERE");
	}
}
