using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    
    public BulletFlavor bulletFlavor; // 子弹口味

    private BulletPool _bulletPool;
    private float _timer;
    
    private PlayerShooting _playerShooting;
    private int _weaponNum;

    private Rigidbody rb;

    private void OnEnable()
    {
        _timer = 0;
    }
    
    private void Start()
    {
        GetInfo();
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= lifeTime) 
        {
            ReturnToPool();
        }
    }

    private void GetInfo()
    {
        rb = GetComponent<Rigidbody>();

        _playerShooting = GameObject.FindWithTag("Player").GetComponent<PlayerShooting>();
        _bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>(); //=========
        
        _weaponNum = _playerShooting.weaponNum;
        rb.velocity = transform.forward * speed;
        SwitchWeapon();
    }
    
    private void OnTriggerEnter(Collider other) //碰撞邏輯
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (_bulletPool != null)
        {
            _bulletPool.ReturnBullet(gameObject, bulletFlavor);
        }
        else
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