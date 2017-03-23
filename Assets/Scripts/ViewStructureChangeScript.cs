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

	protected List<int> currentlyHighlighted;

	// Use this for initialization
	void Start () {
		currentlyHighlighted = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void HighlightProduct(object sender, Control3DEventArgs e){
		VRTK.VRTK_Button btn = (VRTK.VRTK_Button) sender;
		int productToHighlight = btn.gameObject.GetComponent<ButtonInteraction>().productId;
		print("Pusheeeed " + productToHighlight);

		currentlyHighlighted.Add(productToHighlight);

		for (int i = 1; i <= products.transform.childCount; i++)
		{
			GameObject productContainer = GameObject.Find("product-" + (i - 1));
			// print(productContainer.transform.childCount);

			if(i == productToHighlight){
				// den här vill vi highlighta
				for(int j = 0; j < productContainer.transform.childCount; j++)
				{
					GameObject customer = productContainer.transform.GetChild(j).gameObject;
					customer.GetComponentInChildren<MeshRenderer>().material = opaqueMaterials[productToHighlight % products.transform.childCount];
				}
			} 
			else{
				for(int j = 0; j < productContainer.transform.childCount; j++)
				{
					GameObject customer = productContainer.transform.GetChild(j).gameObject;
					// print(customer.GetComponentInChildren<Renderer>().material.color);
					customer.GetComponentInChildren<MeshRenderer>().material = fadedMaterial;
				}
			}
		}
	}
}