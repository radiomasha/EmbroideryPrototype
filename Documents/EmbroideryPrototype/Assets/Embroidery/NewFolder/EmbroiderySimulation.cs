using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.XR.Hands;

public class EmbroiderySimulation : MonoBehaviour
{
    [SerializeField] private Color stitchColor = Color.red;
    [SerializeField] private float stitchWidth = 0.005f;
    [SerializeField] private float snapThreshold = 0.01f;
    [SerializeField] private Material _planeMaterial;
    [SerializeField] private Hand _hand;
    [System.Serializable]
    public class Stitch
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public LineRenderer LineRenderer;
    }

    private List<Vector3> placedPoints = new List<Vector3>();
    private List<Stitch> stitches = new List<Stitch>();
    private Plane embroideryPlane;
    private XRHandSubsystem handSubsystem;
    private List<XRHand> activeHands = new List<XRHand>();
    private int lastProcessedIndex = 0;
    private void Start()
    {
        SetupHandTracking();
        CreateEmbroideryPlane();
    }

    private void SetupHandTracking()
    {
        var handsSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handsSubsystems);
        if (handsSubsystems.Count > 0)
        {
            handSubsystem = handsSubsystems[0];
            handSubsystem.Start();
            UIDebugger.Log("SubsystemStarted");
        }
    }

    private void CreateEmbroideryPlane()
    {
        GameObject planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeObject.transform.position = transform.position + Vector3.forward * 0.35f; 
        planeObject.transform.rotation = Quaternion.Euler(90f, -180f, 0f); 
        planeObject.transform.localScale = Vector3.one * 0.2f;
        Material planeMaterial = _planeMaterial;
        planeObject.GetComponent<Renderer>().material = planeMaterial;
        embroideryPlane = new Plane(Vector3.back, planeObject.transform.position);
    }
    

    private void Update()
    {
        if (handSubsystem != null)
        {
            UpdateActiveHands();
            HandlePointPlacement();
            HandleStitchCreation();
        }
    }
    private void UpdateActiveHands()
    {
        if (handSubsystem == null)
        {
            Debug.Log("No hand subsystem detected");
            return;
        }
        activeHands.Clear();

        if (handSubsystem != null)
        {
            if (handSubsystem.leftHand.isTracked)
                activeHands.Add(handSubsystem.leftHand);

            if (handSubsystem.rightHand.isTracked)
            {
                activeHands.Add(handSubsystem.rightHand);
            }
            else
            {
                    Debug.Log("Right hand is NOT tracked.");
            }
        }
    }

    private void HandlePointPlacement()
    {
        foreach (var hand in activeHands)
        {
            if (IsHandPinching(hand))
            {
                Vector3 pinchPosition = GetPinchPosition(hand);
                PlacePoint(pinchPosition);
            }
        }
    }

    private Vector3 SnapToSurface(Vector3 originalPoint)
    {
        Ray ray = new Ray(originalPoint, -embroideryPlane.normal); 
        float distance;
        if (embroideryPlane.Raycast(ray, out distance))
        {
            Vector3 intersectionPoint = ray.GetPoint(distance);
            if (Vector3.Distance(originalPoint, intersectionPoint) <= snapThreshold)
            {
                return intersectionPoint;
            }
        }
        return originalPoint;
    }

    private void PlacePoint(Vector3 position)
    {
        Vector3 snappedPosition = SnapToSurface(position);
        if (placedPoints.Count == 0 || Vector3.Distance(placedPoints[placedPoints.Count - 1], snappedPosition) > 0.05f)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.position = snappedPosition;
            point.transform.localScale = Vector3.one * 0.02f;
            point.tag = "EmbroideryPoint";
            point.GetComponent<Renderer>().material.color = Color.green;

            placedPoints.Add(snappedPosition);
        }
    }

    private void HandleStitchCreation()
    {
        while (lastProcessedIndex < placedPoints.Count - 1)
        {
            CreateStitch(placedPoints[lastProcessedIndex], placedPoints[lastProcessedIndex + 1]);
            lastProcessedIndex++;
        }
    }

    private void CreateStitch(Vector3 start, Vector3 end)
    {
        Vector3 snappedStart = SnapToSurface(start);
        Vector3 snappedEnd = SnapToSurface(end);

        GameObject stitchObject = new GameObject("Stitch");
        LineRenderer lineRenderer = stitchObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, snappedStart);
        lineRenderer.SetPosition(1, snappedEnd);
        lineRenderer.startWidth = stitchWidth;
        lineRenderer.endWidth = stitchWidth;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = stitchColor;
        Stitch newStitch = new Stitch
        {
            StartPoint = snappedStart,
            EndPoint = snappedEnd,
            LineRenderer = lineRenderer
        };

        stitches.Add(newStitch);
    }

    private bool IsHandPinching(XRHand hand)
    {
        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);

        if (indexTip != null && thumbTip != null &&
            indexTip.TryGetPose(out Pose indexPose) && thumbTip.TryGetPose(out Pose thumbPose))
        {
            return Vector3.Distance(indexPose.position, thumbPose.position) < 0.02f;
        }

        return false;
    }

    private Vector3 GetPinchPosition(XRHand hand)
    {
        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);

        if (indexTip != null && thumbTip != null &&
            indexTip.TryGetPose(out Pose indexPose) && thumbTip.TryGetPose(out Pose thumbPose))
        {
            return (indexPose.position + thumbPose.position) / 2;
        }

        return Vector3.zero;
    }

    public void ResetEmbroidery()
    {
        foreach (var point in GameObject.FindGameObjectsWithTag("EmbroideryPoint"))
        {
            Destroy(point);
        }

        foreach (var stitch in stitches)
        {
            Destroy(stitch.LineRenderer.gameObject);
        }

        placedPoints.Clear();
        stitches.Clear();
        lastProcessedIndex = 0;
    }
}
