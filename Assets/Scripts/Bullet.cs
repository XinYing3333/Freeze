using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private PlayerShooting _playerShooting;
    private int _weaponNum;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject player = GameObject.FindWithTag("Player");
        _playerShooting = player.GetComponent<PlayerShooting>();
        _weaponNum = _playerShooting.weaponNum;
        
        rb.velocity = transform.forward * speed;
        SwitchWeapon();
        Destroy(gameObject, lifeTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    
    private void SwitchWeapon()
    {
        switch (_weaponNum)
        {
            case 0:
                speed = 15f;
                lifeTime = 1.5f;
                break;
            case 1:
                speed = 20f;
                lifeTime = 0.3f;
                break;
        }
    }
}