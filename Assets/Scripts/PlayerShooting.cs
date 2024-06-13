using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletInterval = 0.1f;
    [SerializeField] private bool startShoot;
    [SerializeField] private bool isShooting;

    private int _weaponNum;
    private InputAction _switch;
    
    private PlayerInput _playerInput;
    private InputAction _shootAction;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["Shoot"];
        _switch = _playerInput.actions["Switch"];
        startShoot = false;
    }

    void Update()
    {
        float shoot = _shootAction.ReadValue<float>();
        
        if (shoot > 0)
        {
            startShoot = true;
        }
        else
        {
            startShoot = false;
        }

        if (startShoot)
        {
            StartCoroutine(Shoot());
        }
        SwitchWeapon();
    }
    
    IEnumerator Shoot()
    {
        if (!isShooting)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }

            isShooting = true;
            yield return new WaitForSeconds(bulletInterval);//射擊間隔
            isShooting = false;

        }
    }
    
    private void SwitchWeapon()
    {
        float switchWeapon = _switch.ReadValue<float>();
        if (switchWeapon > 0)
        {
            if (_weaponNum == 0)
            {
                _weaponNum = 1;
                Debug.Log("Switch to Weapon 1");
                bulletInterval = 1f;
            }
            else
            {
                _weaponNum = 0;
                Debug.Log("Switch to Weapon 0");
                bulletInterval = 0.1f;
            }
        }
    }
}