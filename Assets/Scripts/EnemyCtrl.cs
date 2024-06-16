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
    public Animator anim;
    
    [Header("UI State")]
    public Slider healthBar;

    [Header("Attacked State")]
    public float bulletAttack;

    [Header("Blood Effect")]
    public GameObject bloodPrefab;  // Reference to the Blood prefab
    
    private NavMeshAgent _navMeshAgent;   
    private bool _isAttack;
    private bool _isDead;
    private Vector3 _randomTarget;
    private LineRenderer _lineRenderer;
    
    private PlayerShooting _playerShooting;

    private CapsuleCollider _col;
    private GameManager _gameManager; 
    private ButtonFX _buttonFX;


    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        _playerShooting = player.GetComponent<PlayerShooting>();
        
        GameObject tg = GameObject.FindWithTag("IceCreamCar");
        target = tg.transform;
            
        GameObject hp = GameObject.Find("HealthBar");
        healthBar = hp.GetComponent<Slider>();
        
        GameObject gm = GameObject.Find("GameManager");
        _gameManager = gm.GetComponent<GameManager>();
        
        GameObject myFX = GameObject.Find("ButtonFX");
        _buttonFX = myFX.GetComponent<ButtonFX>();
        
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _col = GetComponent<CapsuleCollider>();
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.green;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.positionCount = 0;

        if (target != null)
        {
            // 为每个敌人生成一个随机目标点
            _randomTarget = GenerateRandomTarget(target.position, 5f); // 5f 是半径，可以根据需要调整
            _navMeshAgent.SetDestination(_randomTarget);
        }

        _isDead = false;
    }

    void Update()
    {
        if (enemyType.enemyHealth <= 0 && !_isDead)
        {
            StartCoroutine(OnEnemyDeath());
        }

        // 确保敌人向随机目标点移动
        if (!_isDead && _navMeshAgent != null && _navMeshAgent.isOnNavMesh)
        {
            if (_navMeshAgent.remainingDistance < 0.5f && target != null)
            {
                _randomTarget = GenerateRandomTarget(target.position, 1f);
                _navMeshAgent.SetDestination(_randomTarget);
            }
            //DrawPath();
        }
        
        if (healthBar.value <= 0)
        {
            _gameManager.isOver = true; // GameOver!
        }
        SwitchWeapon();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (!_isDead)
            {
                PlayBloodEffect(other.transform.position);
                _buttonFX.PlayFX("ZombieHit");

                anim.SetTrigger("isHurt");
                enemyType.enemyHealth -= bulletAttack;   
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("IceCreamCar") && !_isAttack && !_isDead)
        {
            StartCoroutine(OnEnemyAttack());
        }
    }
    
    IEnumerator OnEnemyAttack()
    {
        if(healthBar.value > 0)
        {
            _isAttack = true;
            anim.SetTrigger("isAttack");
            yield return new WaitForSeconds(1f);
            healthBar.value -= enemyType.enemyAttack;
            yield return new WaitForSeconds(enemyType.enemyAttackSpeed);
            _isAttack = false;
        }
    }

    IEnumerator OnEnemyDeath()
    {
        _isDead = true;
        
        _buttonFX.PlayFX("ZombieDead");
        
        _gameManager.scoreCount += enemyType.enemyScore;
        _gameManager.enemyKill += 1;
        _gameManager.uiAnim.Play("UI_Anim");
        
        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
        }
        _col.enabled = false;

        anim.SetTrigger("isDeath");

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (_navMeshAgent != null)
        {
            _randomTarget = GenerateRandomTarget(target.position, 5f);
            _navMeshAgent.SetDestination(_randomTarget);
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
    
    private void SwitchWeapon()
    {
        switch (_playerShooting.weaponNum)
        {
            case 0:
                bulletAttack = 12f;
                break;
            case 1:
                bulletAttack = 50f;
                break;
        }
    }

    private void PlayBloodEffect(Vector3 position)
    {
        GameObject bloodEffect = Instantiate(bloodPrefab, position, Quaternion.identity);
        ParticleSystem ps = bloodEffect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Destroy(bloodEffect, ps.main.duration); // Destroy the blood effect object after the particle system duration
        }
    }
    
    private void DrawPath()
    {
        if (_navMeshAgent != null && _navMeshAgent.hasPath)
        {
            _lineRenderer.positionCount = _navMeshAgent.path.corners.Length;
            _lineRenderer.SetPositions(_navMeshAgent.path.corners);
        }
        else
        {
            _lineRenderer.positionCount = 0;
        }
    }
}
