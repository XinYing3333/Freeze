using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform bulletSpawnPoint2;
    [SerializeField] private Transform bulletSpawnPoint3;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletInterval = 0.6f;
    [SerializeField] public bool startShoot;
    [SerializeField] private bool isShooting;
    
    public int weaponNum;

    private InputAction _switch;
    private PlayerInput _playerInput;
    private InputAction _shootAction;
    private bool _isShooting;
    public bool isDash;
    private bool _canSwitchWeapon = true;
    private ButtonFX _buttonFX;


    void Start()
    {
        GameObject myFX = GameObject.Find("ButtonFX");
        _buttonFX = myFX.GetComponent<ButtonFX>();
        
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

        if (startShoot && !isDash)
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
            if (weaponNum == 1)
            {
                Instantiate(bulletPrefab, bulletSpawnPoint2.position, bulletSpawnPoint2.rotation);
                Instantiate(bulletPrefab, bulletSpawnPoint3.position, bulletSpawnPoint3.rotation);
            }
            
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }

            isShooting = true;

            _buttonFX.PlayFX(weaponNum == 1 ? "ShootA" : "ShootB");
            
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
        _buttonFX.PlayFX("GunLoad");
        if (weaponNum == 0)
        {
            weaponNum = 1;
            bulletInterval = 0.6f;
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
