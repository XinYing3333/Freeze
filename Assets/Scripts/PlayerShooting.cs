using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private BulletPool bulletPool; // 引用对象池
    
    public int weaponNum;
    private BulletFlavor currentFlavor = BulletFlavor.Vanilla;
    
    private PlayerInput _playerInput;
    private InputAction _switchWeapon;
    private InputAction _switchBullet;
    private InputAction _shootAction;
    private bool _isShooting;
    public bool isDash;
    private bool _canSwitchWeapon = true;
    private bool _canSwitchBullet = true;
    private ButtonFX _buttonFX;
    
    [SerializeField] private Image vanillaFlavor;
    [SerializeField] private Image strawberryFlavor;
    [SerializeField] private Image chocolateFlavor;


    void Start()
    {
        GameObject myFX = GameObject.Find("ButtonFX");
        _buttonFX = myFX.GetComponent<ButtonFX>();
        
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["Shoot"];
        _switchWeapon = _playerInput.actions["SwitchWeapon"];
        _switchBullet = _playerInput.actions["SwitchBullet"];

        weaponNum = 1;
        
        GameObject myBulletPool = GameObject.Find("BulletPool");
        bulletPool = myBulletPool.GetComponent<BulletPool>();

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
        HandleBulletSwitch();
    }

    IEnumerator Shoot()
    {
        if (!isShooting)
        {
            // 主发射点子弹
            GameObject bullet1 = bulletPool.GetBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            
            Rigidbody rb1 = bullet1.GetComponent<Rigidbody>();
            rb1.velocity = bulletSpawnPoint.forward * bulletSpeed;
            
            // 如果是武器 1，额外发射两颗子弹
            if (weaponNum == 1)
            {
                GameObject bullet2 = bulletPool.GetBullet(bulletSpawnPoint2.position, bulletSpawnPoint2.rotation);
                
                Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
                rb2.velocity = bulletSpawnPoint2.forward * bulletSpeed;

                GameObject bullet3 = bulletPool.GetBullet(bulletSpawnPoint3.position, bulletSpawnPoint3.rotation);
                
                Rigidbody rb3 = bullet3.GetComponent<Rigidbody>();
                rb3.velocity = bulletSpawnPoint3.forward * bulletSpeed;
            }

            isShooting = true;

            _buttonFX.PlayFX(weaponNum == 1 ? "ShootA" : "ShootB");

            yield return new WaitForSeconds(bulletInterval); // 射击间隔
            isShooting = false;
        }
    }


    private void HandleWeaponSwitch()
    {
        float switchWeapon = _switchWeapon.ReadValue<float>();
        if (switchWeapon > 0 && _canSwitchWeapon)
        {
            _canSwitchWeapon = false;
            SwitchWeapon();
            StartCoroutine(ResetSwitchCooldown());
        }
    }
    
    private void HandleBulletSwitch()
    {
        float switchBullet = _switchBullet.ReadValue<float>();
        if (switchBullet > 0 && _canSwitchBullet)
        {
            _canSwitchBullet = false;
            SwitchBullet();
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
    
    private void SwitchBullet()
    {
        _buttonFX.PlayFX("ButtonSelect");
        currentFlavor = (BulletFlavor)(((int)currentFlavor + 1) % Enum.GetValues(typeof(BulletFlavor)).Length);
        Debug.Log($"Bullet Flavor : {currentFlavor}");

        vanillaFlavor.rectTransform.sizeDelta = new Vector2(50, 50);
        strawberryFlavor.rectTransform.sizeDelta = new Vector2(50, 50);
        chocolateFlavor.rectTransform.sizeDelta = new Vector2(50, 50);

        if (currentFlavor == BulletFlavor.Vanilla)
        {
            vanillaFlavor.rectTransform.sizeDelta = new Vector2(100, 100);
        }
        else if (currentFlavor == BulletFlavor.Strawberry)
        {
            strawberryFlavor.rectTransform.sizeDelta = new Vector2(100, 100);
        }
        else
        {
            chocolateFlavor.rectTransform.sizeDelta = new Vector2(100, 100);
        }
    }

    private IEnumerator ResetSwitchCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        if (!_canSwitchWeapon)
        {
            _canSwitchWeapon = true;
        }
        if (!_canSwitchBullet)
        {
            _canSwitchBullet = true;
        }
    }
}

public enum BulletFlavor
{
    Vanilla,   
    Chocolate, 
    Strawberry  
}

