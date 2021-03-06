﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HttpRequest : MonoBehaviour {

    public DataLogger dataLogger;
    public GameObject customerBall;
    public GameObject highlightArea;
    public int numberOfCustomersToGet = 675;
    public Text sliderText;
    public VRTK.VRTK_Slider_Custom sliderNumCustomers;
    protected GameObject customerContainer;
    protected JSONObject customerData;

    public GameObject CustomerSprite;
    public GameObject graphContainer;
    public List<GameObject> customerElementList;
    public VRTK.VRTK_Slider_Custom ageMin;
    public VRTK.VRTK_Slider_Custom ageMax;
    public VRTK.VRTK_Slider_Custom NPSMin;
    public VRTK.VRTK_Slider_Custom NPSMax;
    public VRTK.VRTK_Slider_Custom customerLengthMin;
    public VRTK.VRTK_Slider_Custom customerLengthMax;
    public ViewStructureChangeScript viewStructureChangeScript;
    public int totalAmountOfCustomers = 0;

    private float maxAge = 0;
    private float minAge = Mathf.Infinity;
    private float maxLengthOfBeingCustomer = 0;
    private float minLengthOfBeingCustomer = Mathf.Infinity;
    private bool customersAreGenerated = false;

    //Materials

    public Material cyanMaterial;
    public Material magentaMaterial;
    public Material yellowMaterial;
    public Material fadedMaterial;

    private int numHighlightedCustomers;
    public TextMesh numCustomersText;

	// Use this for initialization
	void Start () {
        WWWForm form = new WWWForm();
        CallSegmentOrderBy();
		//StartCoroutine(GetCustomersWithProducts(form));
        // SetNumOfCustomersToGet();
	}
	
	// Update is called once per frame
	void Update () {
		//Check filters - if moved?
        if (customersAreGenerated) {
            numHighlightedCustomers = 0;
            Transform productsContainer = graphContainer.transform.Find("Products");
            foreach(Transform product in productsContainer) {

                if(viewStructureChangeScript.currentlyHighlighted.Contains(product.GetComponent<CustomerData>().productCategoryId))
                {
                    CustomerData custData = product.GetComponent<CustomerData>();
                    if (
                        //Allow age = 0 to be Random, but change to
                            custData.age < ageMin.GetValue() ||
                            custData.age > ageMax.GetValue() ||
                            custData.npsScore < NPSMin.GetValue() ||
                            custData.npsScore > NPSMax.GetValue() ||
                            custData.timeAsCustomerInMonths < customerLengthMin.GetValue() ||
                            custData.timeAsCustomerInMonths > customerLengthMax.GetValue()
                        ) {
                        //Set faded material
                        product.transform.GetChild(0).GetComponent<Renderer>().material = fadedMaterial;
                    }
                    else {
                        //set previous material
                        product.transform.GetChild(0).GetComponent<Renderer>().material = custData.productMaterial;
                        numHighlightedCustomers++;
                    }
                } else{
                    //Set faded material
                    product.transform.GetChild(0).GetComponent<Renderer>().material = fadedMaterial;
                }
            }
            numCustomersText.text = numHighlightedCustomers.ToString() + "\n/" + totalAmountOfCustomers;
        }
        setHightlightSizePos(ageMin, ageMax, NPSMin, NPSMax, customerLengthMin, customerLengthMax);
	}

    public int getNrOfHighlightedCustomers() {
        //Count customers
        int cCounter = 0;
        Transform productsContainer = graphContainer.transform.Find("Products");
        foreach(Transform product in productsContainer) {
            if (product.transform.GetChild(0).GetComponent<Renderer>().material != fadedMaterial) {
                cCounter++;
            }
        }
        return cCounter;
    }

    public void SendProductRequest() {
        WWWForm form = new WWWForm();
        StopAllCoroutines();
        RemoveCustomers();
        StartCoroutine(GetCustomersWithProducts(form));
    }

    private void setHightlightSizePos(VRTK.VRTK_Slider_Custom ageMin, VRTK.VRTK_Slider_Custom ageMax, VRTK.VRTK_Slider_Custom NPSMin, VRTK.VRTK_Slider_Custom NPSMax, VRTK.VRTK_Slider_Custom customerLengthMin, VRTK.VRTK_Slider_Custom customerLengthMax) {
        //Sets hightlight based on vals
        Transform hlTransform = highlightArea.transform;
        float xPos = customerLengthMin.gameObject.transform.localPosition.x + 0.5f;
        float yPos = NPSMin.gameObject.transform.localPosition.x + 0.5f;
        float zPos = ageMin.gameObject.transform.localPosition.x + 0.5f;
        float xScale = (customerLengthMax.gameObject.transform.localPosition.x+0.5f) - (customerLengthMin.gameObject.transform.localPosition.x+0.5f);
        float yScale = (NPSMax.gameObject.transform.localPosition.x+0.5f) - (NPSMin.gameObject.transform.localPosition.x+0.5f);
        float zScale = (ageMax.gameObject.transform.localPosition.x+0.5f) - (ageMin.gameObject.transform.localPosition.x+0.5f);

        hlTransform.localPosition = new Vector3(-((xPos*10)-5f),(yPos*10),-(zPos*10));
        hlTransform.localScale = new Vector3(-(xScale*10), (yScale*10), (zScale*10));
    }

    private void CalculateMinMaxValues(JSONObject customerData) {
        maxAge = 0;
        minAge = Mathf.Infinity;
        maxLengthOfBeingCustomer = 0;
        minLengthOfBeingCustomer = Mathf.Infinity;

        foreach (JSONObject customer in customerData.list)
            {
                //foreach (JSONObject customer in product.list)
                //{
                    if(customer.HasField("id") && customer.HasField("age")){
                        // find max values
                        if(customer["age"].f > maxAge){
                            maxAge = customer["age"].f;
                        }

                        if(customer["months_ago"].f > maxLengthOfBeingCustomer){
                            maxLengthOfBeingCustomer = customer["months_ago"].f;
                        }

                        // find min values
                        if(customer["age"].f < minAge){
                            minAge = customer["age"].f;
                        }

                        if(customer["months_ago"].f < minLengthOfBeingCustomer){
                            minLengthOfBeingCustomer = customer["months_ago"].f;
                        }
                     }
                // }
            }
            //set min age and max age for slider
            ageMin.minimumValue = minAge;
            ageMin.maximumValue = maxAge;
            ageMax.minimumValue = minAge;
            ageMax.maximumValue = maxAge;
            //Set customer length for slider
            customerLengthMin.minimumValue = minLengthOfBeingCustomer;
            customerLengthMin.maximumValue = maxLengthOfBeingCustomer;
            customerLengthMax.minimumValue = minLengthOfBeingCustomer;
            customerLengthMax.maximumValue = maxLengthOfBeingCustomer;
    }

    public void CallSegmentOrderBy () {
        StopAllCoroutines();
        WWWForm form = new WWWForm();
        form.AddField("segment","1");
        RemoveCustomers();
        StartCoroutine(GetCustomersWithProducts(form));
    }

    public void CallSegmentNotOrderBy () {
        //Second customer segment
        StopAllCoroutines();
        WWWForm form = new WWWForm();
        form.AddField("segment","0");
        RemoveCustomers();
        StartCoroutine(GetCustomersWithProducts(form));
        dataLogger.task1Finished();
    }

    IEnumerator GetCustomersWithProducts(WWWForm form) {
        //Set bool
        customersAreGenerated = false;
        // Create a Web Form
		//WWWForm form = new WWWForm();

        // Number of customers to get with the request
		form.AddField("product_id", "1,2,6");

        // Number of customers to get with the request
		form.AddField("n", numberOfCustomersToGet);
        print("Nrs to get: " + numberOfCustomersToGet);
        print("ChildCount: " + graphContainer.transform.GetChild(0).transform.childCount);

		// Create a download object
		WWW download = new WWW( "http://vrvis-api.app/api/all", form );

		// Wait until the download is done
		yield return download;


		if(!string.IsNullOrEmpty(download.error)) {
			print( "Error downloading: " + download.error );
		} else {
			// show the highscores
            customerData = new JSONObject(download.text);

            // Clear the data in the scene
            //RemoveCustomers();

            CalculateMinMaxValues(customerData);

            print("minAge: " + minAge + "\nmaxAge: " + maxAge);
            print("\nmaxLengthOfCustomer: " + maxLengthOfBeingCustomer + "\nminLengthOfCustomer: " + minLengthOfBeingCustomer);

            int customerCount = 0;
            // For every customer with that product
            foreach (JSONObject customer in customerData.list)
            {
                // Skips first object with only count data
                if(customer.HasField("id")){
                    
                    //GameObject customerObj =  (GameObject) Instantiate(customerElementList[pCount]);
                    
                    //Get object by index from object pool, will fail if over 1000 (implement guard for this)
                    GameObject customerObj = CustomerObjectPool.current.getCustomerObjByIndex(customerCount);
                    CustomerData customerData = customerObj.GetComponent<CustomerData>();

                    float xPos;
                    if(customer.HasField("age")){
                        // (maxAge - minAge) / 2

                        customerData.age = (int) customer["age"].f;
                        // xPos = customer["age"].f / 30;
                    } else{
                        // customerData.age = (int) Random.Range(minAge, maxAge);
                        customerData.age = 42;
                        //xPos = Mathf.Abs(Random.insideUnitCircle.x);
                        //print(customerObj);
                    }

                    xPos = (1 / (maxAge - minAge)) * (customerData.age - minAge);
                    if(xPos < 0){
                        print("WTF");
                    }
                    //Set age to script

                    if(customer.HasField("pivot") && customer["pivot"].HasField("product_id")) {
                        if(customer["pivot"]["product_id"].f == 1f) {
                            //set red material
                            customerObj.transform.GetChild(0).GetComponent<Renderer>().material = cyanMaterial;
                            customerData.productMaterial = cyanMaterial;
                        }
                        else if (customer["pivot"]["product_id"].f == 2f) {
                            customerObj.transform.GetChild(0).GetComponent<Renderer>().material = magentaMaterial;
                            customerData.productMaterial = magentaMaterial;
                        }
                        else if (customer["pivot"]["product_id"].f == 6f) {
                            customerObj.transform.GetChild(0).GetComponent<Renderer>().material = yellowMaterial;
                            customerData.productMaterial = yellowMaterial;
                        }
                        customerData.productCategoryId = (int) customer["pivot"]["product_id"].f;
                    }
                    
                    float zPos;
                    if(customer.HasField("months_ago")){
                        zPos = (1 / (maxLengthOfBeingCustomer - minLengthOfBeingCustomer)) * (customer["months_ago"].f - minLengthOfBeingCustomer);
                        // zPos = customer["months_ago"].f / 60;
                        customerData.timeAsCustomerInMonths = (int) customer["months_ago"].f;
                    } else{
                        zPos = Random.insideUnitCircle.y;
                    }

                    customerData.npsScore = (int)customer["nps_scores"].list[0]["score"].f;


                    customerObj.transform.rotation = Quaternion.identity;
                    customerObj.transform.localPosition = new Vector3(
                        xPos * 5,
                        customer["nps_scores"].list[0]["score"].f / 2,
                        zPos * 5);


                    //Sets saturation depending on age.

                    /*
                    Color emissiveColors = customerObj.transform.GetChild(0).GetComponent<Renderer>().material.GetColor("_EmissionColor");
                    print(emissiveColors);
                    emissiveColors.r = emissiveColors.r * (0.5f / (0.5f + xPos));
                    emissiveColors.g = emissiveColors.g * (0.5f / (0.5f + xPos));
                    emissiveColors.b = emissiveColors.b * (0.5f / (0.5f + xPos));
                    emissiveColors.a = emissiveColors.a * (0.1f / (0.1f + xPos));
                    print(emissiveColors);
                    customerObj.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", emissiveColors);
                    
                    Color colors = customerObj.transform.GetChild(0).GetComponent<Renderer>().material.GetColor("_Color");
                    print(colors);
                    colors.r = colors.r * (0.1f / (0.1f + xPos));
                    colors.g = colors.g * (0.1f / (0.1f + xPos));
                    colors.b = colors.b * (0.1f / (0.1f + xPos));
                    // colors.a = colors.a * (0.1f / (0.1f + xPos));
                    print(colors);
                    customerObj.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", colors);
                    */


                    // For every key, if ever needed
                    // foreach (var key in customer.keys)
                    // {
                    //     print(key + ": " + customer[key]);
                        
                    // }
                    customerCount++;
                }
                
            }
            //Tell app that customers have been generated
            customersAreGenerated = true;
            totalAmountOfCustomers = customerCount;
            //Bad fix for bug
            ageMin.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Free;
            ageMax.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Free;
		}
    }

    public void SetNumOfCustomersToGet() {
        numberOfCustomersToGet = (int) sliderNumCustomers.GetValue();
        // sliderText.text = numberOfCustomersToGet.ToString();

        // print(numberOfCustomersToGet);
    }

    public void updateNumCustomersFromCanvas(Slider sender) {
        print("UPDATING");
        print(sender.value.ToString());
        numberOfCustomersToGet = (int) sender.value;
        print(numberOfCustomersToGet);
    }

    public void RemoveCustomers() {
        Transform productsContainer = graphContainer.transform.Find("Products");
        // print(productsContainer);
        foreach (Transform customer in productsContainer) {
            // Kill all children >:)
            //GameObject.Destroy(product.gameObject);
            customer.GetComponent<CustomerData>().reset();
            customer.gameObject.SetActive(false);
        }
    }
}