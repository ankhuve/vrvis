using UnityEngine;
using System.Collections;
using VRTK;
using VRTK.UnityEventHelper;

public class ButtonInteraction : MonoBehaviour {

	private Vector3 startPosition, offset;
	private VRTK_Button_UnityEvents buttonEvents;
	public ViewStructureChangeScript viewStructureChangeScript;
	public int productId;

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

	private void handlePush(object sender, Control3DEventArgs e)
	{
		Debug.Log("Pushed");
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
