using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    // 子弹预制体
    [SerializeField] private GameObject bulletPrefab;
    // 对象池容量
    [SerializeField] private int poolSize = 40;

    // 对象池列表
    private Queue<GameObject> _bulletPool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // 初始状态设置为非激活
            _bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        if (_bulletPool.Count > 0)
        {
            GameObject bullet = _bulletPool.Dequeue();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.SetActive(true); // 激活子弹
            return bullet;
        }
        else
        {
            return null;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); // 归还到对象池时禁用子弹

        // 重置子弹位置和旋转
        bullet.transform.position = Vector3.zero; // 初始位置为世界坐标原点
        bullet.transform.rotation = Quaternion.identity; // 初始旋转为无旋转

        _bulletPool.Enqueue(bullet); // 放回队列中
    }
}