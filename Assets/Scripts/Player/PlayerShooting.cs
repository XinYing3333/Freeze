using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum BulletFlavor
{
    Vanilla,   
    Chocolate, 
    Strawberry  
}
public class PlayerShooting : MonoBehaviour
{
    [Header("===== GameObjects =====")]
    [Space(10)]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform bulletSpawnPoint2;
    [SerializeField] private Transform bulletSpawnPoint3;
    
    [HideInInspector] public bool startShoot; //public bool for animator
    
    private bool _isShooting;
    private BulletPool _bulletPool; // 引用对象池
    private Bullet _bulletScript;
    
    [Header("===== Bullet Settings =====")]
    [Space(10)]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletInterval = 0.6f;
    
    public int weaponNum;
    private BulletFlavor _currentFlavor = BulletFlavor.Vanilla;
    
    private PlayerInput _playerInput;
    private InputAction _switchWeapon;
    private InputAction _switchBullet;
    private InputAction _shootAction;
    private InputAction _openStore;

    [HideInInspector] public bool isDash;
    private bool _canSwitchWeapon = true;
    private bool _canSwitchBullet = true;
    
    [Header("===== Flavor Image Settings =====")]
    [Space(10)]
    [SerializeField] private Image vanillaFlavor;
    [SerializeField] private Image strawberryFlavor;
    [SerializeField] private Image chocolateFlavor;
    
    private StoreSystem _storeSystem;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shootAction = _playerInput.actions["Shoot"];
        _switchWeapon = _playerInput.actions["SwitchWeapon"];
        _switchBullet = _playerInput.actions["SwitchBullet"];
        _openStore = _playerInput.actions["OpenStore"];
        
        weaponNum = 1;
        
        _bulletPool = GameObject.FindWithTag("BulletPool").GetComponent<BulletPool>();
        _storeSystem = FindObjectOfType<StoreSystem>();

        vanillaFlavor.rectTransform.sizeDelta = new Vector2(100, 100);

        ResourceManager.Instance.AddBullet(50);
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
        OnOpenStore();
    }

    private IEnumerator Shoot()
    {
        if (!_isShooting && ResourceManager.Instance.bulletCount > 0)
        {
            if (weaponNum == 1 && ResourceManager.Instance.bulletCount < 3)
            {
                yield break;
            }

            _isShooting = true;

            GameObject bullet = _bulletPool.GetBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, _currentFlavor);
            
            _bulletScript = bullet.GetComponent<Bullet>();
            _bulletScript.bulletFlavor = _currentFlavor;
                
            Rigidbody rb1 = bullet.GetComponent<Rigidbody>();
            rb1.velocity = bulletSpawnPoint.forward * bulletSpeed;
            
            if (weaponNum == 1)
            {
                //Shoot Point 2
                GameObject bullet2 = _bulletPool.GetBullet(bulletSpawnPoint2.position, bulletSpawnPoint2.rotation, _currentFlavor);
                
                Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
                rb2.velocity = bulletSpawnPoint2.forward * bulletSpeed;

                //Shoot Point 3
                GameObject bullet3 = _bulletPool.GetBullet(bulletSpawnPoint3.position, bulletSpawnPoint3.rotation, _currentFlavor);
                
                Rigidbody rb3 = bullet3.GetComponent<Rigidbody>();
                rb3.velocity = bulletSpawnPoint3.forward * bulletSpeed;
                
                ResourceManager.Instance.ReduceBullets(3);
            }
            else
            {
                ResourceManager.Instance.ReduceBullets(1);
            }
            
            AudioManager.Instance.PlayFX(weaponNum == 1 ? "ShootA" : "ShootB");
            
            yield return new WaitForSeconds(bulletInterval);
            _isShooting = false;
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
        _currentFlavor = (BulletFlavor)(((int)_currentFlavor + 1) % Enum.GetValues(typeof(BulletFlavor)).Length);
        //print($"Bullet Flavor : {_currentFlavor}");

        vanillaFlavor.rectTransform.sizeDelta = new Vector2(50, 50);
        strawberryFlavor.rectTransform.sizeDelta = new Vector2(50, 50);
        chocolateFlavor.rectTransform.sizeDelta = new Vector2(50, 50);

        //UI Change
        if (_currentFlavor == BulletFlavor.Vanilla)
        {
            vanillaFlavor.rectTransform.sizeDelta = new Vector2(100, 100);
        }
        else if (_currentFlavor == BulletFlavor.Strawberry)
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
    
    private void OnOpenStore()
    {
        if (_openStore.WasReleasedThisFrame()) // ButtonUp
        {
           _storeSystem.OpenStore(); //2 player cannot detect====================
        }
    }
}
