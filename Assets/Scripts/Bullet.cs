using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    public BulletFlavor bulletFlavor; // 子弹口味


    private BulletPool bulletPool;
    private float timer;
    
    private PlayerShooting _playerShooting;
    private int _weaponNum;

    private Rigidbody rb;

    void OnEnable()
    {
        timer = 0;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject player = GameObject.FindWithTag("Player");
        bulletPool = player.GetComponent<BulletPool>();
        _playerShooting = player.GetComponent<PlayerShooting>();
        _weaponNum = _playerShooting.weaponNum;
        GameObject myBulletPool = GameObject.Find("BulletPool");
        bulletPool = myBulletPool.GetComponent<BulletPool>();
        
        rb.velocity = transform.forward * speed;
        SwitchWeapon();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            // 到达生存时间，返回池中
            ReturnToPool();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject, bulletFlavor);
            //Debug.Log("bullet returned");
        }
        else
        {
            Destroy(gameObject); // 如果未设置池，仍然销毁
            //Debug.Log("bullet destroy");
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