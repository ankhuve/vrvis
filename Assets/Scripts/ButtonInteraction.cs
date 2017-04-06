﻿using UnityEngine;
using System.Collections;
using VRTK;
using VRTK.UnityEventHelper;

public class ButtonInteraction : MonoBehaviour {

	private Vector3 startPosition, offset;
	private VRTK_Button_UnityEvents buttonEvents;
	public ViewStructureChangeScript viewStructureChangeScript;
	public int productId;
	public Material on;
	public Material off;


	// Use this for initialization
	private void Start () {
		Vector3 startPosition = transform.localPosition;

		buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
		if (buttonEvents == null)
		{
			buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();
		}
		buttonEvents.OnPushed.AddListener(viewStructureChangeScript.HighlightProduct);
	}

	void OnEnable() {
		if(viewStructureChangeScript.currentlyHighlighted.Contains(productId))
		{
			GetComponent<Renderer>().material = on;
		} else
		{
			GetComponent<Renderer>().material = off;
		}
    }

	public void handlePush(object sender, Control3DEventArgs e)
	{
		Debug.Log("Pushed");
		if(GetComponent<Renderer>().sharedMaterial.Equals(on))
		{
			GetComponent<Renderer>().material = off;
			//GetComponent<Renderer>().material = off;
			print("släcker");
		} else{
			// VRTK_SharedMethods.TriggerHapticPulse(0, 1, 0.3f, 0.02f);
			GetComponent<Renderer>().material = on;
			//GetComponent<Renderer>().material = on;
			print("tänder");
		}
	}

	
	// public void OnMouseDown() {
	// 	print("mousedown");
	// 	// gameObject.transform.Translate(Vector3.down * 0.5f, Space.Self);
	// 	iTween.MoveBy(gameObject, iTween.Hash("amount", Vector3.down * 0.5f, "easeType", "easeOutCubic", "time", 0.25f, "onstart", "SetToStartPosition", "onstarttarget", gameObject));
    // }

	// public void OnMouseUp() {
	// 	print("mouseup");
	// 	// gameObject.transform.Translate(Vector3.up * 0.5f, Space.Self);
	// 	iTween.MoveTo(gameObject, iTween.Hash("position", startPosition, "easeType", "easeInCubic", "time", 0.15f, "islocal", true));
    // }

	void SetToStartPosition() {
		transform.localPosition = startPosition;
	}
}
