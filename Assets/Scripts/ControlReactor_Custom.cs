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
            go.text = e.value.ToString() + (unitOfMeasurement != "" ? (" (" + unitOfMeasurement + ((e.value > 1) || (e.value < 1) ? "s)" : ")")) : "");
        }
    }
}