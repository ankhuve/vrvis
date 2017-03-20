using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HttpRequest : MonoBehaviour {

    public GameObject customerBall;
    public int numberOfCustomersToGet;
    public Text sliderText;
    protected Slider numberOfCustomersSlider;
    protected GameObject customerContainer;

    public GameObject CustomerSprite;
    public GameObject graphContainer;
    public List<GameObject> customerElementList;


	// Use this for initialization
	void Start () {
        numberOfCustomersSlider = GameObject.Find ("SliderNumCustomers").GetComponent <Slider> ();
		StartCoroutine(GetCustomersWithProducts());
        SetNumOfCustomersToGet();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SendProductRequest() {
        StartCoroutine(GetCustomersWithProducts());
    }

    IEnumerator GetCustomersWithProducts() {
        // Create a Web Form
		WWWForm form = new WWWForm();

        // Number of customers to get with the request
		form.AddField("product_id", "1,2,6");

        // Number of customers to get with the request
		form.AddField("n", numberOfCustomersToGet);

		// Create a download object
		WWW download = new WWW( "http://vrvis-api.app/api/all", form );

		// Wait until the download is done
		yield return download;


		if(!string.IsNullOrEmpty(download.error)) {
			print( "Error downloading: " + download.error );
		} else {
			// show the highscores
            JSONObject myJson = new JSONObject(download.text);

            // Clear the data in the scene
            RemoveCustomers();

            int pCount = 0;
            // For every product
            foreach (JSONObject product in myJson.list)
            {
                GameObject customerObjContainer =  new GameObject("product-" + pCount);
                customerObjContainer.transform.parent = graphContainer.transform.FindChild("Products");
                customerObjContainer.transform.localPosition = new Vector3(0, 0, 0);
                print(product.list[0]["product_name"]);

                // For every customer with that product
                foreach (JSONObject customer in product.list)
                {
                    GameObject customerObj =  (GameObject) Instantiate(customerElementList[pCount]);
                    
                    // print(customer["nps_scores"].list[0]["score"].f);

                    // för användning av Antons graf
                    // customerObj.transform.position = customerObjContainer.transform.position + new Vector3(
                    //     (customer["nps_scores"].list[0]["score"].f / 50f) * 5.77f, 
                    //     Random.Range(0f, 8.5f/5f),
                    //     0f);

                    customerObj.transform.parent = customerObjContainer.transform;

                    customerObj.transform.rotation = Quaternion.identity;

                    customerObj.transform.localPosition = new Vector3(
                        Random.insideUnitCircle.x, 
                        customer["nps_scores"].list[0]["score"].f / 10,
                        Random.insideUnitCircle.y);

                    // For every key, if ever needed
                    // foreach (var key in customer.keys)
                    // {
                    //     print(key + ": " + customer[key]);
                        
                    // }
                }

                // customerObjContainer.transform.eulerAngles = new Vector3(0, pCount * 72, 0);
                pCount++;
            }

		}
    }

    public void SetNumOfCustomersToGet() {
        numberOfCustomersToGet = (int) numberOfCustomersSlider.value;
        sliderText.text = numberOfCustomersToGet.ToString();

        // print(numberOfCustomersToGet);
    }

    public void RemoveCustomers() {
        Transform productsContainer = graphContainer.transform.Find("Products");
        print(productsContainer);
        foreach (Transform product in productsContainer) {
            // Kill all children >:)
            GameObject.Destroy(product.gameObject);
        }
    }
}