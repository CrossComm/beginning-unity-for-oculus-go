using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManipulation : MonoBehaviour
{
    private enum HighlightState { none, highlighted, grabbed }

    private HighlightState myHighlightState;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform rayCastOrigin;
    [SerializeField] private Transform handGrabbableTarget;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float rotSpeed = 1.0f;
    [SerializeField] private BezierCurveScript curveScript;
    [SerializeField] private bool enableXAxisRotation;

    private float fadeValue = .5f;
    private bool raycastDoesHit;
    private GameObject currentGrabbedObject;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        switch (myHighlightState)
        {
            case HighlightState.none:

                if (Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out hit, Mathf.Infinity, layerMask))
                {
                    curveScript.SetCurveTarget(hit.collider.gameObject);
                    myHighlightState = HighlightState.highlighted;
                }

                if (fadeValue != .5f)
                {
                    if (fadeValue > .5f) fadeValue -= Time.deltaTime;
                    else if (fadeValue < .5f) fadeValue += Time.deltaTime;
                    else fadeValue = .5f;
                }

                break;

            case HighlightState.highlighted:

                if (!Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out hit, Mathf.Infinity, layerMask))
                {
                    curveScript.DeactivateCurveTarget();
                    myHighlightState = HighlightState.none;
                }
                else if (hit.collider.gameObject != currentGrabbedObject)
                {
                    curveScript.SetCurveTarget(hit.collider.gameObject);
                }


                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    currentGrabbedObject = hit.collider.gameObject;

                    FloatToTarget _ftt = currentGrabbedObject.GetComponent<FloatToTarget>();
                    if (_ftt != null) _ftt.SetTarget(handGrabbableTarget);
                    else Debug.Log("<color=red>No Float To Target Script Attached</color>");

                    myHighlightState = HighlightState.grabbed;

                }

                if (fadeValue != 1f)
                {
                    if (fadeValue < 1f) fadeValue += Time.deltaTime;
                    else fadeValue = 1f;
                }

                break;
            case HighlightState.grabbed:


                RotateTarget();

                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    FloatToTarget _ftt = currentGrabbedObject.GetComponent<FloatToTarget>();
                    if (_ftt != null) _ftt.SetTargetToDefault();
                    else Debug.Log("<color=red>No Float To Target Script Attached</color>");

                    currentGrabbedObject = null;

                    ResetTargetRotation();

                    myHighlightState = HighlightState.none;
                }

                if (fadeValue != 0f)
                {
                    if (fadeValue > 0f) fadeValue -= Time.deltaTime;
                    else fadeValue = 0f;
                }


                break;
        }

        curveScript.SetLineThickness(fadeValue);
    }

    void RotateTarget()
    {
        float leftAxis;
        float upAxis;

        leftAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).x;
        upAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

#if UNITY_EDITOR || UNITY_STANDALONE
        leftAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        upAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
#endif
        if (Mathf.Abs(leftAxis) > .1) handGrabbableTarget.Rotate(Vector3.up, (rotSpeed * leftAxis) * Time.deltaTime);
        if (enableXAxisRotation && Mathf.Abs(upAxis) > .25) handGrabbableTarget.Rotate(Vector3.right, (rotSpeed * upAxis) * Time.deltaTime);
    }

    void ResetTargetRotation()
    {
        handGrabbableTarget.localRotation = Quaternion.identity;
    }
}
