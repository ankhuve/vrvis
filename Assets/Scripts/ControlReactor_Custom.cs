namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEventHelper;

    public class ControlReactor_Custom : MonoBehaviour
    {
        public TextMesh go;
        public string unitOfMeasurement;

        private VRTK_Control_UnityEvents controlEvents;

        private void Start()
        {
            controlEvents = GetComponent<VRTK_Control_UnityEvents>();
            if (controlEvents == null)
            {
                controlEvents = gameObject.AddComponent<VRTK_Control_UnityEvents>();
            }

            controlEvents.OnValueChanged.AddListener(HandleChange);
        }

        private void HandleChange(object sender, Control3DEventArgs e)
        {
            if (unitOfMeasurement == "month" && e.value >= 24) {
                float newVal = e.value/12f;
                newVal = Mathf.Round(newVal*2)/2;
                string myCoolNewString = newVal.ToString("0.#");
                int maxLength = 3;
                if (myCoolNewString.Length == 1) {
                    myCoolNewString += " ";
                }
                for (int i = 0; i < maxLength - newVal.ToString().Length; i++) {
                    myCoolNewString += " ";
                }

                go.text = myCoolNewString + " (years)";


                // if (newVal - Mathf.Round(newVal) != 0) {
                //     if (newVal.ToString().Length == 2) {
                //         go.text = newVal.ToString("0.#") + "   (years 10)";
                //     }
                //     else{
                //         go.text = newVal.ToString("0.#") + "(years 1)";
                //     }
                // }
                // else {
                //     go.text = newVal.ToString("0.#") + " (years)";
                // }
            }
            else {
                go.text = e.value.ToString() + (unitOfMeasurement != "" ? (" (" + unitOfMeasurement + ((e.value > 1) || (e.value < 1) ? "s)" : ")")) : "");
            }
        }
    }
}