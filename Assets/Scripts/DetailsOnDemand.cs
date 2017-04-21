namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class DetailsOnDemand : MonoBehaviour {

		protected GameObject popupDisplay;
		public GameObject rightController;
		public GameObject leftController;
		protected CustomerData customerData;

		// Use this for initialization
		void Start () {
			popupDisplay = GameObject.Find("DetailPopup");
			rightController = GameObject.Find("RightController");
			leftController = GameObject.Find("LeftController");
			customerData = transform.parent.GetComponent<CustomerData>();
			Physics.IgnoreCollision(leftController.transform.GetChild(0).GetChild(0).GetComponent<Collider>(), GetComponent<Collider>());

			if (rightController.GetComponent<VRTK_ControllerEvents>() == null)
			{
				Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
				return;
			}
		}

		void OnTriggerStay(Collider other){
			if(other.gameObject.name == "PointCollider" && rightController.GetComponent<VRTK_ControllerEvents>().triggerClicked){
				DisplayPopup();
				rightController.GetComponent<ResetButtonScript>().customerDoD = this;
				rightController.GetComponent<ResetButtonScript>().isTouchingCustomer = true;
			} else if(other.gameObject.name == "PointCollider"){
				rightController.GetComponent<ResetButtonScript>().isTouchingCustomer = true;
			}
		}

		void OnTriggerExit(Collider other){
			if(other.gameObject.name == "PointCollider"){
				rightController.GetComponent<ResetButtonScript>().isTouchingCustomer = false;
			}
		}

		void DisplayPopup(){
			popupDisplay.transform.localScale = new Vector3(0.01656692f, 0.01656692f, 0.01656692f);
			if(customerData.productCategoryId == 1){
				popupDisplay.GetComponent<DetailPopup>().productName.text = "100 mbps";
				popupDisplay.GetComponent<DetailPopup>().productName.color = Color.cyan;
			} else if(customerData.productCategoryId == 2){
				popupDisplay.GetComponent<DetailPopup>().productName.text = "250 mbps";
				popupDisplay.GetComponent<DetailPopup>().productName.color = new Color(0.9f, 0.1f, 0.9f, 1.0f);
			} else if(customerData.productCategoryId == 6){
				popupDisplay.GetComponent<DetailPopup>().productName.text = "1000 mbps";
				popupDisplay.GetComponent<DetailPopup>().productName.color = Color.yellow;
			}
			popupDisplay.GetComponent<DetailPopup>().customerAge.text = customerData.age.ToString();
			popupDisplay.GetComponent<DetailPopup>().customerTimeAs.text = customerData.timeAsCustomerInMonths.ToString();
			popupDisplay.GetComponent<DetailPopup>().customerNPS.text = customerData.npsScore.ToString();
		}

		public void HidePopup(){
			popupDisplay.transform.localScale = Vector3.zero;
		}
	}
}