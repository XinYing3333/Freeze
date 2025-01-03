using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
    [SerializeField] private Bullet bulletScript;
    
    public int weaponNum;
    private BulletFlavor currentFlavor = BulletFlavor.Vanilla;
    
    private PlayerInput _playerInput;
    private InputAction _switchWeapon;
    private InputAction _switchBullet;
    private InputAction _shootAction;
    private InputAction _openStore;

    private bool _isShooting;
    public bool isDash;
    private bool _canSwitchWeapon = true;
    private bool _canSwitchBullet = true;
    
    [SerializeField] private Image vanillaFlavor;
    [SerializeField] private Image strawberryFlavor;
    [SerializeField] private Image chocolateFlavor;
    
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private TruckStore truckStore;
    //[SerializeField] private GameObject autoShootTower;


    void Start()
    {
        
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["Shoot"];
        _switchWeapon = _playerInput.actions["SwitchWeapon"];
        _switchBullet = _playerInput.actions["SwitchBullet"];
        _openStore = _playerInput.actions["OpenStore"];


        weaponNum = 1;
        
        bulletPool = GameObject.FindWithTag("BulletPool").GetComponent<BulletPool>();

        vanillaFlavor.rectTransform.sizeDelta = new Vector2(100, 100);

        resourceManager.AddBullet(50);
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
        OpenStore();
    }

    private IEnumerator Shoot()
    {
        if (!isShooting && resourceManager.bulletCount > 0)
        {
            if (weaponNum == 1 && resourceManager.bulletCount < 3)
            {
                yield break;
            }

            isShooting = true;

            GameObject bullet = bulletPool.GetBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, currentFlavor);
            
            bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.bulletFlavor = currentFlavor;
                
            Rigidbody rb1 = bullet.GetComponent<Rigidbody>();
            rb1.velocity = bulletSpawnPoint.forward * bulletSpeed;
            
            if (weaponNum == 1)
            {
                //Shoot Point 2
                GameObject bullet2 = bulletPool.GetBullet(bulletSpawnPoint2.position, bulletSpawnPoint2.rotation, currentFlavor);
                
                Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
                rb2.velocity = bulletSpawnPoint2.forward * bulletSpeed;

                //Shoot Point 3
                GameObject bullet3 = bulletPool.GetBullet(bulletSpawnPoint3.position, bulletSpawnPoint3.rotation, currentFlavor);
                
                Rigidbody rb3 = bullet3.GetComponent<Rigidbody>();
                rb3.velocity = bulletSpawnPoint3.forward * bulletSpeed;
                
                resourceManager.ReduceBullets(3);
            }
            else
            {
                resourceManager.ReduceBullets(1);
            }
            
            AudioManager.Instance.PlayFX(weaponNum == 1 ? "ShootA" : "ShootB");
            
            yield return new WaitForSeconds(bulletInterval);
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
        AudioManager.Instance.PlayFX("GunLoad");
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
        AudioManager.Instance.PlayFX("ButtonSelect");
        currentFlavor = (BulletFlavor)(((int)currentFlavor + 1) % Enum.GetValues(typeof(BulletFlavor)).Length);
        print($"Bullet Flavor : {currentFlavor}");

        vanillaFlavor.rectTransform.sizeDelta = new Vector2(50, 50);
        strawberryFlavor.rectTransform.sizeDelta = new Vector2(50, 50);
        chocolateFlavor.rectTransform.sizeDelta = new Vector2(50, 50);

        //UI Change
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
    
    private void OpenStore()
    {
        if (_openStore.WasReleasedThisFrame() && truckStore.isInStorePoint) // ButtonUp
        {
            if (truckStore.pointName == "IceCreamCar") // 冰淇淋車（補子彈）
            {
                if (resourceManager.milkCount >= 3) // 3 Milks = 100 Bullets
                { 
                    resourceManager.ReduceMilk(3);
                    resourceManager.AddBullet(100);
                }
                else
                {
                    print("Milk is not enough to make ice cream!");
                }
                
                //if (resourceManager.milkCount == 6) // 3 Milks = 100 Bullets
                { 
                    //resourceManager.ReduceMilk(6);
                    //autoShootTower.SetActive(true);
                }
                //else
                { 
                    //print("Milk is not enough to active Tower!");
                }
            }
        }
    }
}

public enum BulletFlavor
{
    Vanilla,   
    Chocolate, 
    Strawberry  
}

