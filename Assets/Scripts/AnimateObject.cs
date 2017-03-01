using UnityEngine;
using System.Collections;

public class AnimateObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.MoveBy(gameObject, iTween.Hash("y", .1, "time", 2.0, "easeType", "easeInOutSine", "loopType", "pingPong"));
	}

	void Update () {
		
	}
}
