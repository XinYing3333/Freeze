using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject vanillaBulletPrefab;
    [SerializeField] private GameObject chocolateBulletPrefab;
    [SerializeField] private GameObject strawberryBulletPrefab;
    [SerializeField] private int poolSize = 10;

    private Dictionary<BulletFlavor, Queue<GameObject>> _bulletPools;

    private void Start()
    {
        _bulletPools = new Dictionary<BulletFlavor, Queue<GameObject>>
        {
            { BulletFlavor.Vanilla, InitializePool(vanillaBulletPrefab) },
            { BulletFlavor.Chocolate, InitializePool(chocolateBulletPrefab) },
            { BulletFlavor.Strawberry, InitializePool(strawberryBulletPrefab) }
        };
    }

    private Queue<GameObject> InitializePool(GameObject prefab)
    {
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(prefab);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
        return pool;
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation, BulletFlavor flavor)
    {
        if (_bulletPools.TryGetValue(flavor, out Queue<GameObject> pool) && pool.Count > 0)
        {
            GameObject bullet = pool.Dequeue();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.SetActive(true);
            return bullet;
        }

        return null; // 如果池中没有可用子弹
    }

    public void ReturnBullet(GameObject bullet, BulletFlavor flavor)
    {
        bullet.SetActive(false);
        if (_bulletPools.TryGetValue(flavor, out Queue<GameObject> pool))
        {
            pool.Enqueue(bullet);
        }
    }
}