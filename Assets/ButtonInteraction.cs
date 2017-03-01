using UnityEngine;
using System.Collections;

public class ButtonInteraction : MonoBehaviour {

	private Vector3 startPosition, offset;

	// Use this for initialization
	void Start () {
		// Vector3 offset = new Vector3(0, -0.5f, 0);
		Vector3 startPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	// void Update () {
	// 	if (Input.GetMouseButtonDown(0))
    //      {
    //          RaycastHit hit;
    //          Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //          if (Physics.Raycast(ray, out hit))
    //          {
    //              newPosition = hit.point;
    //              transform.position = newPosition;
    //          }
    //      }
	// }

	void OnMouseDown() {
		print("mousedown");
		// gameObject.transform.Translate(Vector3.down * 0.5f, Space.Self);
		iTween.MoveBy(gameObject, iTween.Hash("amount", Vector3.down * 0.5f, "easeType", "easeOutCubic", "time", 0.25f, "onstart", "SetToStartPosition", "onstarttarget", gameObject));
    }

	void OnMouseUp() {
		print("mouseup");
		// gameObject.transform.Translate(Vector3.up * 0.5f, Space.Self);
		iTween.MoveTo(gameObject, iTween.Hash("position", startPosition, "easeType", "easeInCubic", "time", 0.15f, "islocal", true));
    }

	void SetToStartPosition() {
		transform.localPosition = startPosition;
	}
}
