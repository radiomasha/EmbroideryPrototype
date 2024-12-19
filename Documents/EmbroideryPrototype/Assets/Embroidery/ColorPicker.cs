using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private Slider _redSlider;
    [SerializeField] private Slider _greenSlider;
    [SerializeField] private Slider _blueSlider;
    public GameObject _sphere;

    public Color _color;
 
    void Start()
    {
        _redSlider.maxValue = 255;
        _greenSlider.maxValue = 255;
        _blueSlider.maxValue = 255;
        _redSlider.onValueChanged.AddListener(UpdateColorPreview);
        _greenSlider.onValueChanged.AddListener(UpdateColorPreview);
        _blueSlider.onValueChanged.AddListener(UpdateColorPreview);
        UpdateColorPreview(0);
    }

    private void Update()
    {
        _sphere.GetComponent<Renderer>().material.color = _color;
    }

    private void UpdateColorPreview(float value)
    {
        float red = _redSlider.value / 255f;
        float green = _greenSlider.value / 255f;
        float blue = _blueSlider.value / 255f;

        Color baseColor = new Color(red, green, blue);
        _color = baseColor;
    }
}
