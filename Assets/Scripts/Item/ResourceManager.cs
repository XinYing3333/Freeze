using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public int milkCount;
    [SerializeField] private Text milkText;
    public int bulletCount;
    [SerializeField] private Text bulletText;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMilk(int amount)
    {
        milkCount += amount;
        milkText.text = milkCount.ToString();
    }
    
    public void ReduceMilk(int amount)
    {
        milkCount -= amount;
        milkText.text = milkCount.ToString();
    }
    
    public void AddBullet(int amount)
    {
        bulletCount += amount;
        bulletText.text = bulletCount.ToString();
    }

    public void ReduceBullets(int amount)
    {
        bulletCount -= amount;
        bulletText.text = bulletCount.ToString();
    }
}