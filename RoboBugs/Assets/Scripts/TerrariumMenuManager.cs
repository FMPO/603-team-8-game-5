using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerrariumMenuManager : MonoBehaviour {
	[SerializeField] private List<StorageSlot> atlasHUDSlots;
	[SerializeField] private List<StorageSlot> prometheusHUDSlots;
	[SerializeField] public List<StorageSlot> AtlasUpgradeSlots;
	[SerializeField] public List<StorageSlot> PrometheusUpgradeSlots;
	[SerializeField] private List<StorageSlot> storageSlots;
	[SerializeField] private List<Sprite> _bugUISprites;
	[SerializeField] private TextMeshProUGUI quotaText;
	[SerializeField] private TextMeshProUGUI doorUnlockedText;
	[SerializeField] private GameObject doorObject;
	[Space]
	[SerializeField] private Image heldBugImage;
	[SerializeField] private BugType _heldBugType;
	[SerializeField] private StorageSlot _heldFromStorageSlot;
	[SerializeField] private StorageSlot _toStorageSlot;
	[SerializeField] private bool _isHoldingBug;
	[Space]
	[SerializeField, Range(0, 50)] private int quota;

	/// <summary>
	/// Whether or not the player is currently holding a bug
	/// </summary>
	public bool IsHoldingBug {
		get => _isHoldingBug;
		set {
			_isHoldingBug = value;

			// Turn the image on or off based on if the player is holding a bug or not
			heldBugImage.color = (_isHoldingBug ? Color.white : Color.clear);
		}
	}

	/// <summary>
	/// A list of the bug UI sprites
	/// </summary>
	public List<Sprite> BugUISprites => _bugUISprites;

	/// <summary>
	/// The current bug type that is held in the inventory
	/// </summary>
	public BugType HeldBugType {
		get => _heldBugType;
		set {
			_heldBugType = value;

			// Set the held bug image based on the type
			heldBugImage.sprite = BugUISprites[(int) _heldBugType];
		}
	}

	/// <summary>
	/// The storage slot that the current held bug was taken from
	/// </summary>
	public StorageSlot HeldFromStorageSlot { get => _heldFromStorageSlot; set => _heldFromStorageSlot = value; }

	/// <summary>
	/// The storage slot that is currently being hovered over
	/// </summary>
	public StorageSlot ToStorageSlot { get => _toStorageSlot; set => _toStorageSlot = value; }

	private void OnValidate ( ) {
		// Find the door in the scene without having to manually drag the object each time
		doorObject = GameObject.Find("Door");
	}

	private void Awake ( ) {
		OnValidate( );

		// Make sure the player is not holding a bug at the start of the game
		IsHoldingBug = false;
	}

	private void Update ( ) {
		// Have the held bug image follow the mouse
		Vector2 localPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, Camera.main, out localPosition);
		heldBugImage.rectTransform.anchoredPosition = localPosition;
	}

	/// <summary>
	/// Add a bug of a specific type to the storage
	/// </summary>
	/// <param name="bugType">The bug type to add</param>
	public void AddBugToStorage (BugType bugType) {
		// Increment the correct storage slot bug count
		storageSlots[(int) bugType].BugCount++;

		// Update the quota text since the storage has been changed
		UpdateEffects( );
	}

	/// <summary>
	/// Update effects of the terrarium based on the equipped bugs
	/// </summary>
	public void UpdateEffects ( ) {
		// Get the total amount of bugs the player has collected
		int totalBugs = storageSlots[(int) BugType.RED].BugCount + storageSlots[(int) BugType.YELLOW].BugCount + storageSlots[(int) BugType.BLUE].BugCount;

		// Set the quota text
		quotaText.text = $"Bug Goal: {totalBugs} / {quota}";

		// If the player has reached the quota, unlock the door
		bool hasCompletedQuota = (totalBugs >= quota);
		doorUnlockedText.gameObject.SetActive(hasCompletedQuota);
		doorObject.GetComponent<BoxCollider>( ).enabled = !hasCompletedQuota;
		doorObject.GetComponent<SpriteRenderer>( ).enabled = !hasCompletedQuota;

		// Update the player HUD
		// Not the cleanest code but it'll do
		atlasHUDSlots[0].BugType = AtlasUpgradeSlots[0].BugType;
		atlasHUDSlots[1].BugType = AtlasUpgradeSlots[1].BugType;
		atlasHUDSlots[2].BugType = AtlasUpgradeSlots[2].BugType;
		atlasHUDSlots[0].BugCount = AtlasUpgradeSlots[0].BugCount;
		atlasHUDSlots[1].BugCount = AtlasUpgradeSlots[1].BugCount;
		atlasHUDSlots[2].BugCount = AtlasUpgradeSlots[2].BugCount;

		prometheusHUDSlots[0].BugType = PrometheusUpgradeSlots[0].BugType;
		prometheusHUDSlots[1].BugType = PrometheusUpgradeSlots[1].BugType;
		prometheusHUDSlots[2].BugType = PrometheusUpgradeSlots[2].BugType;
		prometheusHUDSlots[0].BugCount = PrometheusUpgradeSlots[0].BugCount;
		prometheusHUDSlots[1].BugCount = PrometheusUpgradeSlots[1].BugCount;
		prometheusHUDSlots[2].BugCount = PrometheusUpgradeSlots[2].BugCount;

		// Update the data tracker to include the new layout
		FindObjectOfType<DataTracker>( ).UpdateCurrentLoadoutString( );

		/// TODO: Update other effects here (like adjusting character abilities)
	}
}
