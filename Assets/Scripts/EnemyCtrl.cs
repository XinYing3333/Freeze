using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    [Header("Enemy State")]
    public EnemyType enemyType; 
    public Transform target; 
    private NavMeshAgent _navMeshAgent;
    private bool _isAttack;

    public Slider healthBar;

    [Header("Attacked State")]
    public float bulletAttack;

    void Start()
    {
        GameObject tg = GameObject.FindWithTag("IceCreamCar");
        target = tg.transform;
            
        GameObject hp = GameObject.Find("HealthBar");
        healthBar = hp.GetComponent<Slider>();
            
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (target != null)
        {
            _navMeshAgent.SetDestination(target.position);
        }

        bulletAttack = 50f;
    }

    void Update()
    {
        if (enemyType.enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            enemyType.enemyHealth -= bulletAttack;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("IceCreamCar") && !_isAttack)
        {
            StartCoroutine(OnEnemyAttack());
        }
    }

    IEnumerator OnEnemyAttack()
    {
        if(healthBar.value > 0)
        {
            _isAttack = true;
            yield return new WaitForSeconds(enemyType.enemyAttackSpeed);
            healthBar.value -= enemyType.enemyAttack;
            _isAttack = false;
        }
    }
}