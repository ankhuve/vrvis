using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerData : MonoBehaviour {

	public int productCategoryId;
	public int age;
	public string name;
	public int npsScore;
	public float timeAsCustomerInMonths;
	public Material productMaterial;
	public Material fadedMaterial;

	public void reset() {
		productCategoryId = -1;
		age = -1;
		name = null;
		npsScore = -1;
		timeAsCustomerInMonths = -1f;
		productMaterial = null;
		fadedMaterial = null;
	}

}
