using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAdjustment : MonoBehaviour
{
    [SerializeField] private StitchRenderer stitchRenderer;
    private Color _color;
    // Start is called before the first frame update
    void Start()
    {
        _color = GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustColor();
    }

    private void AdjustColor()
    {
        Material _material = stitchRenderer._lineMaterial;
        _color = _material.color;
    }
}
