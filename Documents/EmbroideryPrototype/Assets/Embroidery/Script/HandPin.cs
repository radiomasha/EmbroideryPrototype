using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;


public class HandPin: MonoBehaviour
{
    [Header("Mixed Reality Settings")]
    public HandRef primaryHand;
    public HandSkeleton handSkeleton;
    public LayerMask embroiderySurfaceLayer;

    [Header("Needle Settings")]
    public GameObject needlePrefab;
    public float needlePlacementDistance = 0.1f;

    [Header("Thread Settings")]
    public Material threadMaterial;
    public Color[] threadColors = { Color.red, Color.blue, Color.green };
    public float threadThickness = 0.005f;

    [Header("Visual Feedback")]
    public GameObject pinIndicatorPrefab;
    public ParticleSystem threadPlacementEffect;

    private LineRenderer currentThreadLine;
    private Vector3 firstPinPoint;
    private Vector3 secondPinPoint;
    private bool isFirstPin = true;
    private int currentColorIndex = 0;
    private List<LineRenderer> threadLines = new List<LineRenderer>();

    void Update()
    {
        if (primaryHand.GetFingerIsPinching(HandFinger.Index))
        {
            Ray handRay = new Ray(
                primaryHand.transform.position, 
                primaryHand.transform.forward
            );

            RaycastHit hit;
            if (Physics.Raycast(handRay, out hit, 0.2f, embroiderySurfaceLayer))
            {
                TryPlacePin(hit.point);
            }
        }
    }

    void TryPlacePin(Vector3 pinPosition)
    {
        // Prevent rapid successive placements
        if (Vector3.Distance(pinPosition, isFirstPin ? firstPinPoint : secondPinPoint) < needlePlacementDistance)
            return;

        if (isFirstPin)
        {
            firstPinPoint = pinPosition;
            CreatePinVisual(firstPinPoint);
            isFirstPin = false;
        }
        else
        {
            secondPinPoint = pinPosition;
            CreatePinVisual(secondPinPoint);
            DrawThread();
            isFirstPin = true;
        }
    }

    void CreatePinVisual(Vector3 position)
    {
        GameObject pinIndicator = Instantiate(pinIndicatorPrefab, position, Quaternion.identity);
    }

    void DrawThread()
    {
        Color currentColor = threadColors[currentColorIndex];
        currentColorIndex = (currentColorIndex + 1) % threadColors.Length;

        // Create thread line
        GameObject threadObject = new GameObject("EmbroideryThread");
        currentThreadLine = threadObject.AddComponent<LineRenderer>();

        // Configure line renderer
        currentThreadLine.startWidth = threadThickness;
        currentThreadLine.endWidth = threadThickness;
        currentThreadLine.material = threadMaterial;
        currentThreadLine.startColor = currentColor;
        currentThreadLine.endColor = currentColor;

        // Set thread positions
        currentThreadLine.positionCount = 2;
        currentThreadLine.SetPosition(0, firstPinPoint);
        currentThreadLine.SetPosition(1, secondPinPoint);
        threadLines.Add(currentThreadLine);
        // Thread placement visual effect
        if (threadPlacementEffect != null)
        {
            threadPlacementEffect.transform.position = Vector3.Lerp(firstPinPoint, secondPinPoint, 0.5f);
            threadPlacementEffect.Play();
        }
    }
    
    public void ResetEmbroidery()
    {

        isFirstPin = true;
    }
}
