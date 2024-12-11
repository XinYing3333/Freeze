using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Milk : MonoBehaviour
{
    [SerializeField] private int milkValue = 3; // 每次增加的资源数量

    
    private Animator _anim;
    private ButtonFX _buttonFX;

    void Start()
    {
        _anim = GetComponent<Animator>();
        
        GameObject myFX = GameObject.Find("ButtonFX");
        _buttonFX = myFX.GetComponent<ButtonFX>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(GetItem());
        }
    }

    IEnumerator GetItem()
    {
        _buttonFX.PlayFX("GetItem");
        _anim.SetBool("isGet",true);
        
        ResourceManager.Instance.AddMilk(milkValue);
        
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
