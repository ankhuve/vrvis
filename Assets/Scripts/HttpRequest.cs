using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HttpRequest : MonoBehaviour {

    public GameObject customerBall;
    public int numberOfCustomersToGet;
    public Text sliderText;
    protected Slider numberOfCustomersSlider;
    protected GameObject customerContainer;


	// Use this for initialization
	void Start () {
        numberOfCustomersSlider = GameObject.Find ("SliderNumCustomers").GetComponent <Slider> ();
        customerContainer = GameObject.Find("CustomerContainer");
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
		form.AddField("product_id", "1,2");

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

            // For every product
            foreach (JSONObject product in myJson.list)
            {
                // For every customer with that product
                foreach (JSONObject customer in product.list)
                {
                    GameObject customerObj =  (GameObject) Instantiate(customerBall, new Vector3(
                        Random.insideUnitCircle.x * 10, 
                        customer["nps_scores"].list[0]["score"].f,
                        Random.insideUnitCircle.y * 10),
                        Quaternion.identity
                    );
                    customerObj.transform.parent = customerContainer.transform;

                    print(customer["nps_scores"].list[0]["score"].f);

                    // For every key, if needed for some time
                    // foreach (var key in customer.keys)
                    // {
                    //     print(key + ": " + customer[key]);
                        
                    // }
                }
            }

		}
    }

    public void SetNumOfCustomersToGet() {
        numberOfCustomersToGet = (int) numberOfCustomersSlider.value;
        sliderText.text = numberOfCustomersToGet.ToString();

        // print(numberOfCustomersToGet);
    }

    public void RemoveCustomers() {
        foreach (Transform child in customerContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}