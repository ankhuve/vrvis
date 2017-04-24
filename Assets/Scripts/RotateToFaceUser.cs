using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RotateToFaceUser : MonoBehaviour {

	public GameObject head;
	public bool onlyRotateHorizontal;
	public List<GameObject> buttonSets;
	public ViewStructureChangeScript viewStructureChangeScript;
	public bool isNPSScoreSlider;
	private Vector3 initialRotation;

	void Start(){
		initialRotation = new Vector3(0,0,-90);
	}

	
	// Update is called once per frame
	void Update () {

		if(onlyRotateHorizontal)
		{
			if(head.transform.position.x > 0 && (Mathf.Abs(head.transform.position.z) < 0.3)) {
				// vi är vid Fetch-panelen
				EnableButtonSet(0);
			} else if((head.transform.position.x < 0 && head.transform.position.x > -0.7f) && head.transform.position.z > 0){
				EnableButtonSet(1);
			} else if(head.transform.position.x < 0 && (Mathf.Abs(head.transform.position.z) < 0.3)){
				EnableButtonSet(2);
			} else if((head.transform.position.x < 0 && head.transform.position.x > -0.7f) && head.transform.position.z < 0){
				EnableButtonSet(3);
			}

		} else if(isNPSScoreSlider){
			Vector3 lookPos = transform.position - head.transform.position;
			lookPos.x = 0;
			if(lookPos.z <= 0){
				transform.rotation = Quaternion.LookRotation(lookPos);
			} else{
				transform.localEulerAngles = initialRotation;
			}
		} else {
			// Vector3 lookPoint = head.transform.position;
			// transform.LookAt(lookPoint);
			transform.rotation = Quaternion.LookRotation(transform.position - head.transform.position);
		}

	}

	// index is the button set to be enabled, all others will be disabled
	void EnableButtonSet(int index){
		// print("Trying to instantiate " + index);
		for (int i = 0; i < transform.childCount; i++){
			if(index == i){
				foreach (Transform button in transform.GetChild(i))
				{
					button.GetChild(0).GetComponent<ButtonInteraction>().SetActive(true);
				}
				// transform.GetChild(i).gameObject.GetComponent<ConfigurableJoint>().yMotion = ConfigurableJointMotion.Limited;				
			} else{
				foreach (Transform button in transform.GetChild(i))
				{
					button.GetChild(0).GetComponent<ButtonInteraction>().SetActive(false);
				}
			}
		}
	}
}
