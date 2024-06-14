using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletInterval = 1f;
    [SerializeField] public bool startShoot;
    [SerializeField] private bool isShooting;
    
    public int weaponNum;

    private InputAction _switch;
    private PlayerInput _playerInput;
    private InputAction _shootAction;
    private bool _isShooting;
    private bool _canSwitchWeapon = true;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["Shoot"];
        _switch = _playerInput.actions["Switch"];
        weaponNum = 1;
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
        HandleWeaponSwitch();
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

    private void HandleWeaponSwitch()
    {
        float switchWeapon = _switch.ReadValue<float>();
        if (switchWeapon > 0 && _canSwitchWeapon)
        {
            _canSwitchWeapon = false;
            SwitchWeapon();
            StartCoroutine(ResetSwitchCooldown());
        }
    }

    private void SwitchWeapon()
    {
        if (weaponNum == 0)
        {
            weaponNum = 1;
            bulletInterval = 0.5f;
        }
        else
        {
            weaponNum = 0;
            bulletInterval = 0.1f;
        }
    }

    private IEnumerator ResetSwitchCooldown()
    {
        yield return new WaitForSeconds(0.5f); // Adjust this value if needed
        _canSwitchWeapon = true;
    }
}
