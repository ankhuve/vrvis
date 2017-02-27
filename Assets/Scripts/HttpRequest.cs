using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
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
		StartCoroutine(GetCustomers());
        SetNumOfCustomersToGet();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SendRequest() {
        StartCoroutine(GetCustomers());
    }

	IEnumerator GetCustomers() {
        // Create a Web Form
		WWWForm form = new WWWForm();

        // Number of customers to get with the request
		form.AddField("n", numberOfCustomersToGet);

		// Create a download object
		WWW download = new WWW( "http://vrvis-api.app/api/customers", form );

		// Wait until the download is done
		yield return download;
        

		if(!string.IsNullOrEmpty(download.error)) {
			print( "Error downloading: " + download.error );
		} else {
			// show the highscores
            Customer[] results = JsonHelper.getJsonArray<Customer> (download.text);

            RemoveCustomers();

            foreach (Customer c in results)
            {
                GameObject customerObj =  (GameObject) Instantiate(customerBall, Random.insideUnitSphere * 10, Quaternion.identity);
                customerObj.transform.parent = customerContainer.transform;
                print(c.street);
            }
		}
    }

    public void SetNumOfCustomersToGet() {
        numberOfCustomersToGet = (int) numberOfCustomersSlider.value;
        sliderText.text = numberOfCustomersToGet.ToString();

        print(numberOfCustomersToGet);
    }

    public void RemoveCustomers() {
        foreach (Transform child in customerContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }


    [System.Serializable]
    public class Customer
    {
        // public string name;
        // public int lives;
        // public float health;
        public string customer_type, customer_number, first_name, last_name, email, mobile, street, city, zip_code, social_security_number, created_date, created_at, updated_at;
        public int id, ataio_id, organization_id;

        public static Customer CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Customer>(jsonString);
        }
    }
}