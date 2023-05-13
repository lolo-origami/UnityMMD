using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraXTrans : MonoBehaviour
{
    [SerializeField]
    private Transform _charaTrans = null;

    // Update is called once per frame
    void Update()
    {
        if (!_charaTrans != null)
        {
            var tmpPos = gameObject.transform.localPosition;
            tmpPos.x = _charaTrans.transform.localPosition.x;
            gameObject.transform.localPosition = tmpPos;
        }
        
    }
}
