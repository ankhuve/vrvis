using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HttpRequest : MonoBehaviour {

    public GameObject customerBall;
    public int numberOfCustomersToGet;
    public Text sliderText;
    public VRTK.VRTK_Slider_Custom sliderNumCustomers;
    protected GameObject customerContainer;
    protected JSONObject customerData;

    public GameObject CustomerSprite;
    public GameObject graphContainer;
    public List<GameObject> customerElementList;
    private float maxAge = 0;
    private float minAge = Mathf.Infinity;
    private float maxLengthOfBeingCustomer = 0;
    private float minLengthOfBeingCustomer = Mathf.Infinity;

    //Materials

    public Material cyanMaterial;
    public Material magentaMaterial;
    public Material yellowMaterial;

	// Use this for initialization
	void Start () {
		StartCoroutine(GetCustomersWithProducts());
        SetNumOfCustomersToGet();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SendProductRequest() {
        StopAllCoroutines();
        StartCoroutine(GetCustomersWithProducts());
    }

    private void CalculateMinMaxValues(JSONObject customerData) {
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
    }

    IEnumerator GetCustomersWithProducts() {
        // Create a Web Form
		WWWForm form = new WWWForm();

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

            print("minAge: " + minAge + "\nmaxAge: " + maxAge + "\nmaxLengthOfCustomer: " + maxLengthOfBeingCustomer + "\nminLengthOfCustomer" + minLengthOfBeingCustomer);

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
                        xPos = (1 / (maxAge - minAge)) * (customer["age"].f - minAge);
                        if(xPos < 0){
                            print(customer["age"]);
                        }
                        //Set age to script
                        customerData.age = (int) customer["age"].f;
                        // xPos = customer["age"].f / 30;
                    } else{
                        xPos = Mathf.Abs(Random.insideUnitCircle.x);
                        print("dunno: " + xPos);
                    }

                    if(customer.HasField("pivot") && customer["pivot"].HasField("product_id")) {
                        if(customer["pivot"]["product_id"].f == 1f) {
                            //set red material
                            customerObj.transform.GetChild(0).GetComponent<Renderer>().material = cyanMaterial;
                        }
                        else if (customer["pivot"]["product_id"].f == 2f) {
                            customerObj.transform.GetChild(0).GetComponent<Renderer>().material = magentaMaterial;
                        }
                        else if (customer["pivot"]["product_id"].f == 6f) {
                            customerObj.transform.GetChild(0).GetComponent<Renderer>().material = yellowMaterial;
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
                }
                customerCount++;
            }
		}
    }

    public void SetNumOfCustomersToGet() {
        numberOfCustomersToGet = (int) sliderNumCustomers.GetValue();
        sliderText.text = numberOfCustomersToGet.ToString();

        // print(numberOfCustomersToGet);
    }

    public void RemoveCustomers() {
        Transform productsContainer = graphContainer.transform.Find("Products");
        // print(productsContainer);
        foreach (Transform customer in productsContainer) {
            // Kill all children >:)
            //GameObject.Destroy(product.gameObject);
            customer.gameObject.SetActive(false);
        }
    }
}