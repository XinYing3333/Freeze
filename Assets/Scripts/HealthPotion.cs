using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPotion : MonoBehaviour
{
    public Slider healthBar;
    private Animator _anim;
    private ButtonFX _buttonFX;

    void Start()
    {
        GameObject hp = GameObject.Find("HealthBar");
        healthBar = hp.GetComponent<Slider>();
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
        if (healthBar.value < 100)
        {
            healthBar.value += 10;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
