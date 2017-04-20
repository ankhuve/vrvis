namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class ResetButtonScript : MonoBehaviour {
		public List<GameObject> sliders = new List<GameObject>();
		public List<GameObject> buttons = new List<GameObject>();
		public List<Vector3> startPositions = new List<Vector3>();
		public ViewStructureChangeScript vsChangeScript;
		public List<ColorChanger> colorChangers;
		public DataLogger dl;
		

		// Use this for initialization
		void Start () {
			if (GetComponent<VRTK_ControllerEvents>() == null)
			{
				Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
				return;
			}
			GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
			GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

			foreach (GameObject slider in sliders)
			{
				startPositions.Add(slider.transform.localPosition);
			}
		}

		private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
		{
			ResetSliders();
			ResetButtons();
			dl.IncrementResetClicks();
		}

		private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
		{
			print(e.controllerIndex + " TOUCHPAD " + " released "+ e);
		}

		void ResetSliders(){
			int idx = 0;
			foreach (GameObject slider in sliders)
			{
				slider.transform.localPosition = startPositions[idx];
				idx++;
			}
		}

		void ResetButtons(){
			vsChangeScript.Start();
			foreach (ColorChanger cc in colorChangers)
			{
				cc.SetActive();
			}
		}
	}
}