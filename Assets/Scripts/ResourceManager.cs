using UnityEngine;
using UnityEngine.UI;


public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private int _resourceCount;
    [SerializeField] private Text resourceText;
    


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

    public void AddResource(int amount)
    {
        _resourceCount += amount;
        UpdateResourceUI(_resourceCount);
    }
    
    public void UpdateResourceUI(int currentResource)
    {
        resourceText.text = currentResource.ToString();
    }
}