using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightCalibration : MonoBehaviour {

	private const float HEIGHT_MULTIPLIER = 0.6875f;
	public GameObject head;
	public GameObject sliders;
	public GameObject buttons;

	// Use this for initialization
	void Start () {
		head = GameObject.Find("Camera (eye)");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")){
			foreach (Transform child in transform.GetChild(0))
			{
				Destroy(child.gameObject);
			}

            transform.position = new Vector3(transform.position.x, head.transform.localPosition.y * HEIGHT_MULTIPLIER, transform.position.z);
			GameObject goSliders = Instantiate(sliders);
			goSliders.transform.parent = transform.GetChild(0);
			goSliders.transform.localPosition = sliders.transform.localPosition;
			goSliders.transform.localRotation = sliders.transform.localRotation;
			goSliders.transform.localScale = sliders.transform.localScale;

			GameObject goButtons = Instantiate(buttons);
			goButtons.transform.parent = transform.GetChild(0);
			goButtons.transform.localPosition = buttons.transform.localPosition;
			goButtons.transform.localRotation = buttons.transform.localRotation;
			goButtons.transform.localScale = buttons.transform.localScale;
		}
	}


}
