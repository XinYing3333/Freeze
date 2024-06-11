using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamCar : MonoBehaviour
{
    public Transform canvasTranform;
    public Transform aimPoint;
        
        
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        canvasTranform.LookAt(transform.position + aimPoint.transform.forward);
    }
}
