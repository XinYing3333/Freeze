using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    // **敌人状态**
    [Header("Enemy State")]
    public EnemyType enemyType; 
    private bool _isDead;
    private bool _isAttack;
    private Vector3 _randomTarget;

    // **目标相关**
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    // **组件引用**
    [Header("Component References")]
    [SerializeField] private Animator anim;
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _col;
    private LineRenderer _lineRenderer;

    // **玩家交互**
    [Header("Player Interaction")]
    private PlayerShooting _playerShooting;

    // **UI 和血量**
    [Header("UI Settings")]
    [SerializeField] private Slider healthBar;

    // **攻击相关**
    [Header("Attack Settings")]
    [SerializeField] private float bulletAttack;

    // **特效**
    [Header("Effects")]
    [SerializeField] private GameObject bloodPrefab;
    private GameManager _gameManager;
    private ButtonFX _buttonFX;

    // **动画参数哈希值**
    private static readonly int IsHurt = Animator.StringToHash("isHurt");
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int IsDeath = Animator.StringToHash("isDeath");

    // **初始化组件引用**
    void Awake()
    {
        _playerShooting = GameObject.FindWithTag("Player").GetComponent<PlayerShooting>();
        target = GameObject.FindWithTag("IceCreamCar").transform;
        healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Slider>();
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _buttonFX = GameObject.FindWithTag("AudioSystem").GetComponent<ButtonFX>();
        
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _col = GetComponent<CapsuleCollider>();

        InitializeLineRenderer();
    }

    // **设置初始状态**
    void Start()
    {
        if (target != null)
        {
            _randomTarget = GenerateRandomTarget(target.position, 5f);
            _navMeshAgent.SetDestination(_randomTarget);
        }
        _isDead = false;
    }

    // **每帧逻辑**
    void Update()
    {
        if (!_isDead)
        {
            HandleMovement();
            if (enemyType.enemyHealth <= 0) StartCoroutine(OnEnemyDeath());
        }

        if (healthBar.value <= 0) _gameManager.isOver = true;

        UpdateWeaponStats();
    }

    // **触发逻辑：受到子弹攻击**
    private void OnTriggerEnter(Collider other)
    {
        if (!_isDead && other.CompareTag("Bullet"))
        {
            HandleBulletHit();
        }
    }

    // **触发逻辑：攻击冰淇淋车**
    private void OnTriggerStay(Collider other)
    {
        if (!_isDead && !_isAttack && other.CompareTag("IceCreamCar"))
        {
            StartCoroutine(OnEnemyAttack());
        }
    }

    // **攻击逻辑**
    IEnumerator OnEnemyAttack()
    {
        if (healthBar.value > 0)
        {
            _isAttack = true;
            anim.SetTrigger(IsAttack);
            yield return new WaitForSeconds(1f);
            healthBar.value -= enemyType.enemyAttack;
            yield return new WaitForSeconds(enemyType.enemyAttackSpeed);
            _isAttack = false;
        }
    }

    // **死亡逻辑**
    IEnumerator OnEnemyDeath()
    {
        _isDead = true;
        
        // 播放音效和更新得分
        _buttonFX.PlayFX("ZombieDead");
        _gameManager.scoreCount += enemyType.enemyScore;
        _gameManager.enemyKill++;
        _gameManager.uiAnim.Play("UI_Anim");

        // 停止导航
        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
        }

        _col.enabled = false;
        anim.SetTrigger(IsDeath);

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }

    // **设置目标**
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (_navMeshAgent != null)
        {
            _randomTarget = GenerateRandomTarget(target.position, 5f);
            _navMeshAgent.SetDestination(_randomTarget);
        }
    }

    // **生成随机目标点**
    private Vector3 GenerateRandomTarget(Vector3 center, float range)
    {
        Vector3 randomPos = center + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }

    // **更新武器数据**
    private void UpdateWeaponStats()
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
    
    private void HandleBulletHit()
    {
        PlayBloodEffect(transform.position);

        _buttonFX.PlayFX("ZombieHit");
        anim.SetTrigger(IsHurt);

        enemyType.enemyHealth -= bulletAttack;
        if (enemyType.enemyHealth <= 0 && !_isDead)
        {
            StartCoroutine(OnEnemyDeath());
        }
    }


    // **播放血液特效**
    private void PlayBloodEffect(Vector3 position)
    {
        GameObject bloodEffect = Instantiate(bloodPrefab, position, Quaternion.identity);
        if (bloodEffect.TryGetComponent(out ParticleSystem ps))
        {
            ps.Play();
            Destroy(bloodEffect, ps.main.duration);
        }
    }

    // **初始化路径绘制器**
    private void InitializeLineRenderer()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.green;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.positionCount = 0;
    }

    // **处理移动逻辑**
    private void HandleMovement()
    {
        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh && _navMeshAgent.remainingDistance < 0.5f && target != null)
        {
            _randomTarget = GenerateRandomTarget(target.position, 1f);
            _navMeshAgent.SetDestination(_randomTarget);
        }
    }
}
