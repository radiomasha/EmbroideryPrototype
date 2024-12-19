using System;
using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EmbroideryController : MonoBehaviour
{
   public StitchRenderer stitchRenderer;
   public ColorPicker colorPicker;
   public List<GameObject> _stithes = new List<GameObject>();
   [SerializeField] private float _offset = 0.0005f;
   [SerializeField] private GameObject pointPrefab;
   [SerializeField] private GameObject _plane;
   
   private Vector3? _start;
   private Vector3? _end;
   private Color _color;
   private GameObject _startPointVisual;
   private GameObject _endPointVisual;
   //private bool _stitchCreated = false;
   private void Update()
   {
      _color = stitchRenderer._lineMaterial.color;
      if (_start != null && _end != null)
      {
         ResetPoints();
      }
   }
   
   private void CreateStitch()
   {
         Vector3 start = _start.Value;
         Vector3 end = _end.Value;
         AlignToPlane(ref start);
         AlignToPlane(ref end);
         //Color stitchColor = _color;
         GameObject newStitch = stitchRenderer.CreateStitch(start, end, colorPicker._color, _plane.GetComponent<Collider>());
         if (newStitch != null)
         {
            _stithes.Add(newStitch);
            UIDebugger.Log("Created stitch");
         }
   }

   private void ResetPoints()
   {
      if (_startPointVisual != null)
      {
         Destroy(_startPointVisual);
         _startPointVisual = null;
      }
      if (_endPointVisual != null)
      {
         Destroy(_endPointVisual);
         _endPointVisual = null;
      }
      _start = null;
      _end = null;

      UIDebugger.Log("Points reset");
   }

   private void AlignToPlane(ref Vector3 point)
   {
      Plane plane = new Plane(_plane.transform.up, _plane.transform.position);
      float distanceToPlane = plane.GetDistanceToPoint(point);
      point -= plane.normal * distanceToPlane;
     
   }

   private Vector3 GetCentralPoint()
   {
      Vector3 bottomCenter = GetComponent<BoxCollider>().bounds.center;
      bottomCenter.y = GetComponent<BoxCollider>().bounds.min.y;
      return bottomCenter;
   }

   private void OnCollisionEnter(Collision other)
   {
      UIDebugger.Log($"Collision detected with {other.gameObject.name}");
      Vector3 bottomCenter = GetCentralPoint();
      if(other.gameObject.CompareTag("Plane"))
      {
         ContactPoint contact = other.contacts[0];
         if (_start==null)
         {
            _start = contact.point;
            Vector3 start = _start.Value;
            AlignToPlane(ref start);
            _startPointVisual = Instantiate(pointPrefab, start, Quaternion.identity);
            UIDebugger.Log("start is "+ _start);
         }
         else if (_start!= null&& _end==null)
         {
            _end = contact.point;
            Vector3 end = _end.Value;
            AlignToPlane(ref end);
            UIDebugger.Log("end  is "+ _end);
           _endPointVisual = Instantiate(pointPrefab, end, Quaternion.identity);
           CreateStitch();
         }
      }
   }

   public void EraseStitch()
   {
      if (_stithes.Count > 0)
      {
         GameObject lastStitch = _stithes[_stithes.Count - 1];
         _stithes.RemoveAt( _stithes.Count - 1);
         Destroy(lastStitch);
         UIDebugger.Log("Stitch erased");
      }
   }
  
}