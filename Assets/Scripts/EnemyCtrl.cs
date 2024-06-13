using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    [Header("Enemy State")]
    public EnemyType enemyType; 
    public Transform target; 
    private NavMeshAgent _navMeshAgent;
    private bool _isAttack;

    public Slider healthBar;

    [Header("Attacked State")]
    public float bulletAttack;

    public Animator anim;
    
    private Vector3 randomTarget;
    private LineRenderer lineRenderer;

    void Start()
    {
        GameObject tg = GameObject.FindWithTag("IceCreamCar");
        target = tg.transform;
            
        GameObject hp = GameObject.Find("HealthBar");
        healthBar = hp.GetComponent<Slider>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = 0;

        if (target != null)
        {
            // 为每个敌人生成一个随机目标点
            randomTarget = GenerateRandomTarget(target.position, 5f); // 5f 是半径，可以根据需要调整
            _navMeshAgent.SetDestination(randomTarget);
        }

        bulletAttack = 10f;
    }

    void Update()
    {
        if (enemyType.enemyHealth <= 0)
        {
            StartCoroutine(OnEnemyDeath());
        }

        // 确保敌人向随机目标点移动
        if (_navMeshAgent.remainingDistance < 0.5f && target != null)
        {
            randomTarget = GenerateRandomTarget(target.position, 1f);
            _navMeshAgent.SetDestination(randomTarget);
        }

        DrawPath();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            anim.SetTrigger("isHurt");
            enemyType.enemyHealth -= bulletAttack;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("IceCreamCar") && !_isAttack)
        {
            StartCoroutine(OnEnemyAttack());
        }
    }

    IEnumerator OnEnemyAttack()
    {
        if(healthBar.value > 0)
        {
            _isAttack = true;
            yield return new WaitForSeconds(enemyType.enemyAttackSpeed);
            healthBar.value -= enemyType.enemyAttack;
            _isAttack = false;
        }
    }

    IEnumerator OnEnemyDeath()
    {
// 禁用NavMeshAgent以停止敌人的移动
        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;

        // 停止所有动画，只播放死亡动画
        anim.SetTrigger("isDeath");

        // 禁用Collider防止后续碰撞
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 等待死亡动画播放完成
        yield return new WaitForSeconds(4f);

        // 删除敌人对象
        Destroy(gameObject);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (_navMeshAgent != null)
        {
            randomTarget = GenerateRandomTarget(target.position, 5f);
            _navMeshAgent.SetDestination(randomTarget);
        }
    }

    private Vector3 GenerateRandomTarget(Vector3 center, float range)
    {
        Vector3 randomPos = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }

    private void DrawPath()
    {
        if (_navMeshAgent.hasPath)
        {
            lineRenderer.positionCount = _navMeshAgent.path.corners.Length;
            lineRenderer.SetPositions(_navMeshAgent.path.corners);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
