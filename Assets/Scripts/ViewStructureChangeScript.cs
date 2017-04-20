using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRTK;
using VRTK.UnityEventHelper;

public class ViewStructureChangeScript : MonoBehaviour {

	public GameObject APIObject;
	public GameObject products;
	public List<Material> opaqueMaterials;
	public List<Material> transparentMaterials;
	public Material fadedMaterial;

	public List<int> currentlyHighlighted;

	// Use this for initialization
	public void Start () {
		currentlyHighlighted = new List<int>();
		currentlyHighlighted.Add(1);
		currentlyHighlighted.Add(2);
		currentlyHighlighted.Add(6);
	}
	
	public void HighlightProduct(object sender, Control3DEventArgs e){
		VRTK.VRTK_Button btn = (VRTK.VRTK_Button) sender;
		int productToHighlight = btn.gameObject.GetComponent<ButtonInteraction>().productId;
		// print("Pusheeeed " + productToHighlight);

		btn.gameObject.GetComponent<ButtonInteraction>().handlePush(sender, e);

		if(currentlyHighlighted.Contains(productToHighlight))
		{
			currentlyHighlighted.Remove(productToHighlight);
			// print("tar bort " + productToHighlight);
		} else
		{
			currentlyHighlighted.Add(productToHighlight);
			// print("addar " + productToHighlight);
			
		}
	}
}