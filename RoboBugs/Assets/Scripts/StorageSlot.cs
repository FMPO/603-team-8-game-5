using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour {
	[SerializeField] private Image bugImage;
	[SerializeField] private TextMeshProUGUI bugCountText;
	[SerializeField] private Sprite[ ] bugUISprites;
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
			bugImage.sprite = bugUISprites[(int) _bugType];
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
		BugType = BugType;
		BugCount = BugCount;
		HasBug = HasBug;
		HideBugCount = HideBugCount;
	}
}
