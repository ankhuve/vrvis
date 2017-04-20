// Slider|Controls3D|100090
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a horizontal or vertical slider. The direction can be freely set and auto-detection is supported.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` has a selection of sliders at various angles with different step values to demonstrate their usage.
    /// </example>
    public class VRTK_Slider_Custom : VRTK_Control
    {
        [Tooltip("An optional game object to which the wheel will be connected. If the game object moves the wheel will follow along.")]
        public GameObject connectedTo;
        [Tooltip("The axis on which the slider should move. All other axis will be frozen.")]
        public Direction direction = Direction.autodetect;
        [Tooltip("The collider to specify the minimum limit of the slider.")]
        public Collider minimumLimit;
        [Tooltip("The collider to specify the maximum limit of the slider.")]
        public Collider maximumLimit;
        [Tooltip("The minimum value of the slider.")]
        public float minimumValue = 0f;
        [Tooltip("The maximum value of the slider.")]
        public float maximumValue = 100f;
        [Tooltip("The increments in which slider values can change.")]
        public float stepSize = 0.1f;
        [Tooltip("If this is checked then when the slider is released, it will snap to the nearest value position.")]
        public bool snapToStep = false;
        [Tooltip("The amount of friction the slider will have when it is released.")]
        public float releasedFriction = 50f;
        public GameObject APIHandler;
        public DataLogger dataLogger;
        public Material fadedTextMaterial;
        public Material visibleTextMaterial;

        protected Direction finalDirection;
        protected Rigidbody sliderRigidbody;
        protected ConfigurableJoint sliderJoint;
        protected bool sliderJointCreated = false;
        protected Vector3 minimumLimitDiff;
        protected Vector3 maximumLimitDiff;
        protected Vector3 snapPosition;
        public Vector3 minPoint;
        public Vector3 maxPoint;
        protected VRTK_InteractableObject sliderInteractableObject;
        protected Highlighters.VRTK_OutlineObjectCopyHighlighter highlighter;
        protected VRTK.Examples.ControlReactor_Custom controlReactor;
        private Color fadedText = new Color(0.25f,0.25f,0.25f,1);
        private Color visibleText = new Color(1,1,1,1);


        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }
            Gizmos.DrawLine(transform.position, minimumLimit.transform.position);
            Gizmos.DrawLine(transform.position, maximumLimit.transform.position);
        }

        protected override void InitRequiredComponents()
        {
            DetectSetup();
            InitRigidbody();
            InitInteractableObject();
            InitJoint();
            highlighter = GetComponent<Highlighters.VRTK_OutlineObjectCopyHighlighter>();
            controlReactor = GetComponent<VRTK.Examples.ControlReactor_Custom>();
            visibleTextMaterial = transform.parent.FindChild("Description").GetComponent<Renderer>().material;

            sliderInteractableObject.InteractableObjectUngrabbed += (s,e) => {
                if(APIHandler.GetComponent<HttpRequest>().grabbedSliders.Count == 1){
                    transform.parent.FindChild("Description").GetComponent<MeshRenderer>().material = visibleTextMaterial;
                }
                APIHandler.GetComponent<HttpRequest>().grabbedSliders.Remove(sliderInteractableObject.gameObject);
                dataLogger.StopSliderUse();
            };

            // log clicks
            sliderInteractableObject.InteractableObjectGrabbed += (s,e) => {
                if(!APIHandler.GetComponent<HttpRequest>().grabbedSliders.Contains(sliderInteractableObject.gameObject)){
                    APIHandler.GetComponent<HttpRequest>().grabbedSliders.Add(sliderInteractableObject.gameObject);
                }
                transform.parent.FindChild("Description").GetComponent<MeshRenderer>().material = fadedTextMaterial;
                dataLogger.StartSliderUse();
                dataLogger.IncrementSliderUses();
            };
        }

        protected override bool DetectSetup()
        {
            if (sliderJointCreated)
            {
                if (connectedTo)
                {
                    sliderJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }
            }

            finalDirection = direction;

            if (direction == Direction.autodetect)
            {
                RaycastHit hitRight;
                RaycastHit hitUp;
                RaycastHit hitForward;

                int layerMask = 1 << 8;

                bool rightHasHit = Physics.Raycast(transform.position, transform.right, out hitRight, Mathf.Infinity, layerMask);
                bool upHasHit = Physics.Raycast(transform.position, transform.up, out hitUp, Mathf.Infinity, layerMask);
                bool forwardHasHit = Physics.Raycast(transform.position, transform.forward, out hitForward, Mathf.Infinity, layerMask);

                Vector3 sliderDiff = transform.localScale / 2f;
                
                //The right ray has found the min on the right, so max is on the left
                if (rightHasHit && hitRight.collider.gameObject.Equals(minimumLimit.gameObject))
                {
                    finalDirection = Direction.x;
                    minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.right, minimumLimit.transform.localScale.x, sliderDiff.x, false);
                    maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.right, maximumLimit.transform.localScale.x, sliderDiff.x, true);
                }

                //The right ray has found the max on the right, so min is on the left
                if (rightHasHit && hitRight.collider.gameObject.Equals(maximumLimit.gameObject))
                {
                    finalDirection = Direction.x;
                    minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.right, minimumLimit.transform.localScale.x, sliderDiff.x, true);
                    maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.right, maximumLimit.transform.localScale.x, sliderDiff.x, false);
                }

                // the up ray has found the min above, so max is below
                if (upHasHit && hitUp.collider.gameObject.Equals(minimumLimit.gameObject))
                {
                    finalDirection = Direction.y;
                    minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.up, minimumLimit.transform.localScale.y, sliderDiff.y, false);
                    maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.up, maximumLimit.transform.localScale.y, sliderDiff.y, true);
                }

                //the up ray has found the max above, so the min ix below
                if (upHasHit && hitUp.collider.gameObject.Equals(maximumLimit.gameObject))
                {
                    finalDirection = Direction.y;
                    minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.up, minimumLimit.transform.localScale.y, sliderDiff.y, true);
                    maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.up, maximumLimit.transform.localScale.y, sliderDiff.y, false);
                }

                //the forward ray has found the min in front, so the max is behind
                if (forwardHasHit && hitForward.collider.gameObject.Equals(minimumLimit.gameObject))
                {
                    finalDirection = Direction.z;
                    minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.forward, minimumLimit.transform.localScale.y, sliderDiff.y, false);
                    maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.forward, maximumLimit.transform.localScale.y, sliderDiff.y, true);
                }

                //the forward ray has found the max in front, so the min is behind
                if (forwardHasHit && hitForward.collider.gameObject.Equals(maximumLimit.gameObject))
                {
                    finalDirection = Direction.z;
                    minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.forward, minimumLimit.transform.localScale.z, sliderDiff.z, true);
                    maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.forward, maximumLimit.transform.localScale.z, sliderDiff.z, false);
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = minimumValue,
                controlMax = maximumValue
            };
        }

        protected override void HandleUpdate()
        {
            CalculateValue();

            if(sliderInteractableObject.IsTouched())
            {
                controlReactor.go.transform.position = new Vector3(controlReactor.go.transform.position.x, transform.position.y + 0.08f, controlReactor.go.transform.position.z);

            } else{
                controlReactor.go.transform.position = new Vector3(controlReactor.go.transform.position.x, transform.position.y - 0.01f, controlReactor.go.transform.position.z);                                
            }

            // keep the slider lit while sliding
            if(sliderInteractableObject.IsGrabbed())
            {
                highlighter.Highlight(sliderInteractableObject.touchHighlightColor);
            }

            //APIHandler.GetComponent<HttpRequest>().SetNumOfCustomersToGet();

            if (snapToStep)
            {
                SnapToValue();
            }
        }

        protected virtual Vector3 CalculateDiff(Vector3 initialPosition, Vector3 givenDirection, float scaleValue, float diffMultiplier, bool addition)
        {
            var additionDiff = givenDirection * diffMultiplier;

            var limitDiff = givenDirection * (scaleValue / 2f);
            if (addition)
            {
                limitDiff = initialPosition + limitDiff;
            }
            else
            {
                limitDiff = initialPosition - limitDiff;
            }

            var answer = initialPosition - limitDiff;

            if (addition)
            {
                answer -= additionDiff;
            }
            else
            {
                answer += additionDiff;
            }

            return answer;
        }

        protected virtual void InitRigidbody()
        {
            sliderRigidbody = GetComponent<Rigidbody>();
            if (sliderRigidbody == null)
            {
                sliderRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            sliderRigidbody.isKinematic = false;
            sliderRigidbody.useGravity = false;
            sliderRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            sliderRigidbody.drag = releasedFriction;

            if (connectedTo)
            {
                Rigidbody connectedToRigidbody = connectedTo.GetComponent<Rigidbody>();
                if (connectedToRigidbody == null)
                {
                    connectedToRigidbody = connectedTo.AddComponent<Rigidbody>();
                    connectedToRigidbody.useGravity = false;
                    connectedToRigidbody.isKinematic = true;
                }
            }
        }

        protected virtual void InitInteractableObject()
        {
            sliderInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (sliderInteractableObject == null)
            {
                sliderInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            sliderInteractableObject.isGrabbable = true;
            sliderInteractableObject.grabAttachMechanicScript = gameObject.AddComponent<GrabAttachMechanics.VRTK_TrackObjectGrabAttach>();
            sliderInteractableObject.secondaryGrabActionScript = gameObject.AddComponent<SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
            sliderInteractableObject.grabAttachMechanicScript.precisionGrab = true;
            sliderInteractableObject.stayGrabbedOnTeleport = false;
        }

        protected virtual void InitJoint()
        {
            sliderJoint = GetComponent<ConfigurableJoint>();
            if (sliderJoint == null)
            {
                sliderJoint = gameObject.AddComponent<ConfigurableJoint>();
            }

            sliderJoint.xMotion = (finalDirection == Direction.x ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
            sliderJoint.yMotion = (finalDirection == Direction.y ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
            sliderJoint.zMotion = (finalDirection == Direction.z ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);

            sliderJoint.angularXMotion = ConfigurableJointMotion.Locked;
            sliderJoint.angularYMotion = ConfigurableJointMotion.Locked;
            sliderJoint.angularZMotion = ConfigurableJointMotion.Locked;

            ToggleSpring(false);
            sliderJointCreated = true;
        }

        protected virtual void CalculateValue()
        {
            minPoint = minimumLimit.transform.localPosition - minimumLimitDiff;
            maxPoint = maximumLimit.transform.localPosition - maximumLimitDiff;

            float maxDistance = Vector3.Distance(minPoint, maxPoint);
            float currentDistance = Vector3.Distance(minPoint, transform.localPosition);

            float currentValue = Mathf.Round((minimumValue + Mathf.Clamp01(currentDistance / maxDistance) * (maximumValue - minimumValue)) / stepSize) * stepSize;

            float flatValue = currentValue - minimumValue;
            float controlRange = maximumValue - minimumValue;
            float actualPosition = (flatValue / controlRange);
            snapPosition = minPoint + ((maxPoint - minPoint) * actualPosition);

            value = currentValue;
        }

        protected virtual void ToggleSpring(bool state)
        {
            JointDrive snapDriver = new JointDrive();
            snapDriver.positionSpring = (state ? 10000f : 0f);
            snapDriver.positionDamper = (state ? 10f : 0f);
            snapDriver.maximumForce = (state ? 100f : 0f);

            sliderJoint.xDrive = snapDriver;
            sliderJoint.yDrive = snapDriver;
            sliderJoint.zDrive = snapDriver;
        }

        protected virtual void SnapToValue()
        {
            ToggleSpring(true);
            sliderJoint.targetPosition = snapPosition * -1f;
            sliderJoint.targetVelocity = Vector3.zero;
        }
    }
}