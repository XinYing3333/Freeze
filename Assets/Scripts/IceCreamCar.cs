using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamCar : MonoBehaviour
{
    public Transform canvasTranform;
        
        
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        canvasTranform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
