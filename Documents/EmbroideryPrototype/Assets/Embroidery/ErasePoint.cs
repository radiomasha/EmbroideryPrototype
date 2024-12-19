using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasePoint : MonoBehaviour
{
    [SerializeField] private EmbroideryController _embroideryController;
    //private List<GameObject> _stitches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //_stitches = _embroideryController._stithes;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        _embroideryController._stithes.RemoveAt( _embroideryController._stithes.Count - 1);
    }
    public void RemoveLastStitch()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
