using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSender : MonoBehaviour
{
    [SerializeField]
    private Renderer[] _rendererArray = null;

    private Vector3 _pos = Vector3.zero;
    private Vector3 _size = Vector3.one;
    // Update is called once per frame
    void Update()
    {
        foreach (var materiaRenderer in _rendererArray)
        {
            foreach (var material in materiaRenderer.materials)
            {
                _pos.x = transform.position.x;_pos.y = transform.position.y; _pos.z = transform.position.z;
                _size.x = transform.localScale.x;_size.y = transform.localScale.y; _size.z = transform.localScale.z; 
                
                material.SetVector("_BOXCenter", _pos);
                material.SetVector("_BOXSize", _size);
            }
        }

    }
}
