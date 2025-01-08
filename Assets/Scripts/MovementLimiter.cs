using UnityEngine;
using Cinemachine;

public class MovementLimiter : MonoBehaviour
{
    [SerializeField] private Transform player;          // 玩家对象
    [SerializeField] private CinemachineTargetGroup targetGroup; // Target Group 组件
    [SerializeField] private float maxDistance = 10f;   // 玩家到中心的最大允许距离

    private Vector3 groupCenter;

    private void Update()
    {
        if (targetGroup != null)
        {
            // 获取 TargetGroup 的中心点
            groupCenter = targetGroup.transform.position;

            // 计算玩家到中心的距离
            float distanceToCenter = Vector3.Distance(player.position, groupCenter);

            // 检测是否超过最大范围
            if (distanceToCenter > maxDistance)
            {
                LimitPlayerMovement(distanceToCenter);
            }
        }
    }

    private void LimitPlayerMovement(float distanceToCenter)
    {
        Debug.Log($"Player exceeded range by {distanceToCenter - maxDistance:F2} units");

        // 将玩家限制在最大距离内
        Vector3 direction = (player.position - groupCenter).normalized;
        player.position = groupCenter + direction * maxDistance;
    }
}