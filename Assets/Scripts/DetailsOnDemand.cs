namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class DetailsOnDemand : MonoBehaviour {

		protected GameObject popupDisplay;
		public GameObject rightController;

		// Use this for initialization
		void Start () {
			popupDisplay = GameObject.Find("DetailPopup");
			rightController = GameObject.Find("RightController");

			if (rightController.GetComponent<VRTK_ControllerEvents>() == null)
			{
				Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
				return;
			}
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void OnTriggerStay(Collider other){
			if(other.gameObject.name == "PointCollider" && rightController.GetComponent<VRTK_ControllerEvents>().triggerClicked){
				DisplayPopup();
			}
		}

		void DisplayPopup(){
			popupDisplay.transform.parent = transform;
			popupDisplay.transform.localPosition = transform.localPosition;
		}
	}
}