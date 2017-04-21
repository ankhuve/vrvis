using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerColliderCheck : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "PointCollider") {
			GameObject controller = other.gameObject.transform.parent.gameObject;
			controller.transform.FindChild("VRTK_ControllerCollidersContainer").GetChild(0).gameObject.SetActive(false);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.name == "PointCollider") {
			GameObject controller = other.gameObject.transform.parent.gameObject;
			controller.transform.FindChild("VRTK_ControllerCollidersContainer").GetChild(0).gameObject.SetActive(true);
		}
	}
}
