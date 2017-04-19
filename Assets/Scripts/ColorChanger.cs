using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour {

	public Color on;
	public Color off;
	protected bool isActive = true;
	private Color thisColor;

	void Start () {
		thisColor = GetComponent<TextMesh>().color;
		on = new Color(thisColor.r, thisColor.g, thisColor.b, 1.0f);
		off = new Color(thisColor.r, thisColor.g, thisColor.b, 0.3f);
	}

	public void ToggleActive () {
		if(isActive)
		{
			GetComponent<TextMesh>().color = off;
		} else{
			GetComponent<TextMesh>().color = on;
		}
		isActive = !isActive;
		print("togglat");
	}
}
