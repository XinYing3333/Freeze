using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlavorUI : MonoBehaviour
{
    private Camera _cam;
    
    void Start()
    {
        _cam = Camera.main;
    }
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }

    
}
