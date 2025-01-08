using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StoreSystem : MonoBehaviour
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
    
    public void OpenStore()
    {
        if (isInStorePoint) 
        {
            if (pointName == "IceCreamCar") // 冰淇淋車（補子彈）
            {
                if (ResourceManager.Instance.milkCount >= 3) // 3 Milks = 100 Bullets
                { 
                    ResourceManager.Instance.ReduceMilk(3);
                    ResourceManager.Instance.AddBullet(100);
                }
                else
                {
                    print("Milk is not enough to make ice cream!");
                }
                
                //if (resourceManager.milkCount == 6) // 3 Milks = 100 Bullets
                { 
                    //resourceManager.ReduceMilk(6);
                    //autoShootTower.SetActive(true);
                }
                //else
                { 
                    //print("Milk is not enough to active Tower!");
                }
            }
        }
    }
    
}
