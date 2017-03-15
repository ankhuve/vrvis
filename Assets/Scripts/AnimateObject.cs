using UnityEngine;
using System.Collections;

public class AnimateObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.MoveBy(gameObject, iTween.Hash("y", transform.localScale.y * 0.3, "time", 2.0, "easeType", "easeInOutSine", "loopType", "pingPong"));
	}

	void Update () {
		
	}
}
