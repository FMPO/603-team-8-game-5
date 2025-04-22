using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldBug : MonoBehaviour {
	[SerializeField] private Terrarium terrarium;

	private void OnValidate ( ) {
		terrarium = FindObjectOfType<Terrarium>( );
	}

	private void Awake ( ) {
		OnValidate( );
	}

	public void OnPointerUp ( ) {
		Debug.Log("UP HELD");

		//// Do nothing if there is nothing being held
		//if (!terrarium.IsHoldingBug) {
		//	return;
		//}

		//terrarium.IsHoldingBug = false;
		//terrarium.HeldFromStorageSlot.BugCount += 1;
		//terrarium.HeldFromStorageSlot = null;
	}
}
