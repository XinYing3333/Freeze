using System.Collections;
using UnityEngine;

public class AutoShootTower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool bulletPool; // 引用对象池
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Settings")]
    [SerializeField] private float bulletSpeed = 20f; // 子弹速度
    [SerializeField] private BulletFlavor currentFlavor = BulletFlavor.Vanilla; // 当前口味
    [SerializeField] private float shootInterval = 0.6f; // 射击间隔时间
    [SerializeField] private float disableDelay = 10f; // 自禁时间
    
    
    private void Start()
    {
        // 开始射击循环
        InvokeRepeating(nameof(AutoShoot), 0f, shootInterval);

        // 10秒后禁用对象
        Invoke(nameof(DisableTower), disableDelay);
    }

    private void AutoShoot()
    {
        // 从对象池获取子弹
        GameObject bullet = bulletPool.GetBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, currentFlavor);

        if (bullet != null)
        {
            // 设置子弹的口味
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.bulletFlavor = currentFlavor;

            // 设置子弹速度
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }
        }
    }

    private void DisableTower()
    {
        // 停止射击
        CancelInvoke(nameof(AutoShoot));
        // 禁用塔
        gameObject.SetActive(false);
    }
}