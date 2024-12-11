using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckStore : MonoBehaviour
{
    public bool isInStorePoint;
    public string pointName;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInStorePoint = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInStorePoint = false;
        }
    }
    
}
