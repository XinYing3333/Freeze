using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    [Header("===== Enemy State =====")]
    [Space(10)]
    public EnemyType enemyType;
    private bool _isDead;
    private bool _isAttack;
    private Vector3 _randomTarget;
    [SerializeField] private GameObject bloodPrefab;

    [Header("===== Flavor Settings =====")]
    [Space(10)]
    public Dictionary<BulletFlavor, int> flavorNeeds = new Dictionary<BulletFlavor, int>();
    [SerializeField] private Image vanillaImage;
    [SerializeField] private Image chocolateImage;
    [SerializeField] private Image strawberryImage;

    [SerializeField] private Text vanillaText;
    [SerializeField] private Text chocolateText;
    [SerializeField] private Text strawberryText;

    [Header("===== Target Settings =====")]
    [Space(10)]
    [SerializeField] private Transform target;

    [Header("===== Component References =====")]
    [Space(10)]
    [SerializeField] private Animator anim;
    [SerializeField] private Slider healthBar;
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _col;
    private GameManager _gameManager;


    private static readonly int IsHurt = Animator.StringToHash("isHurt");
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int IsDeath = Animator.StringToHash("isDeath");

    void Awake()
    {
        // 初始化引用
        target = GameObject.FindWithTag("IceCreamCar").transform;
        healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Slider>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _col = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        // 设置初始状态
        if (target != null)
        {
            _randomTarget = GenerateRandomTarget(target.position, 5f);
            _navMeshAgent.SetDestination(_randomTarget);
        }
        _isDead = false;

        // 初始化所有 Image 为关闭状态
        vanillaImage.gameObject.SetActive(false);
        chocolateImage.gameObject.SetActive(false);
        strawberryImage.gameObject.SetActive(false);

        GenerateFlavorNeeds();
    }

    void Update()
    {
        if (!_isDead)
        {
            HandleMovement();
            if (enemyType.enemyHealth <= 0) StartCoroutine(OnEnemyDeath());
        }

        if (healthBar.value <= 0)
        {
            _gameManager.isOver = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isDead && other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                HandleBulletHit(bullet.bulletFlavor);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isDead && !_isAttack && other.CompareTag("IceCreamCar"))
        {
            StartCoroutine(OnEnemyAttack());
        }
    }

    private IEnumerator OnEnemyAttack()
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

    private void HandleBulletHit(BulletFlavor bulletFlavor)
    {
        if (flavorNeeds.ContainsKey(bulletFlavor))
        {
            PlayBloodEffect(transform.position);
            flavorNeeds[bulletFlavor]--;
            
            // 更新需求数量显示
            switch (bulletFlavor)
            {
                case BulletFlavor.Vanilla:
                    vanillaText.text = flavorNeeds[bulletFlavor].ToString();
                    break;
                case BulletFlavor.Chocolate:
                    chocolateText.text = flavorNeeds[bulletFlavor].ToString();
                    break;
                case BulletFlavor.Strawberry:
                    strawberryText.text = flavorNeeds[bulletFlavor].ToString();
                    break;
            }

            // 如果需求完成，关闭对应的 Image
            if (flavorNeeds[bulletFlavor] <= 0)
            {
                flavorNeeds.Remove(bulletFlavor);
                switch (bulletFlavor)
                {
                    case BulletFlavor.Vanilla:
                        vanillaImage.gameObject.SetActive(false);
                        break;
                    case BulletFlavor.Chocolate:
                        chocolateImage.gameObject.SetActive(false);
                        break;
                    case BulletFlavor.Strawberry:
                        strawberryImage.gameObject.SetActive(false);
                        break;
                }
            }

            anim.SetTrigger(IsHurt);

            // 所有需求完成，敌人死亡
            if (flavorNeeds.Count == 0)
            {
                StartCoroutine(OnEnemyDeath());
            }
        }
    }

    private IEnumerator OnEnemyDeath()
    {
        _isDead = true;

        // 播放死亡动画和音效
        anim.SetTrigger(IsDeath);
        AudioManager.Instance.PlayFX("ZombieDead");

        // 更新游戏状态
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

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
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

    private void GenerateFlavorNeeds()
    {
        int flavorCount = 1; // ===== 口味數量 ======
        foreach (BulletFlavor flavor in (BulletFlavor[])System.Enum.GetValues(typeof(BulletFlavor)))
        {
            if (flavorNeeds.Count >= flavorCount) break;
            if (!flavorNeeds.ContainsKey(flavor))
            {
                int requiredCount = Random.Range(1, 3); // ===== 需求數量(max-1=實際數字) ======
                flavorNeeds[flavor] = requiredCount;

                switch (flavor)
                {
                    case BulletFlavor.Vanilla:
                        vanillaImage.gameObject.SetActive(true);
                        vanillaText.text = requiredCount.ToString();
                        break;
                    case BulletFlavor.Chocolate:
                        chocolateImage.gameObject.SetActive(true);
                        chocolateText.text = requiredCount.ToString();
                        break;
                    case BulletFlavor.Strawberry:
                        strawberryImage.gameObject.SetActive(true);
                        strawberryText.text = requiredCount.ToString();
                        break;
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh && _navMeshAgent.remainingDistance < 0.5f && target != null)
        {
            _randomTarget = GenerateRandomTarget(target.position, 1f);
            _navMeshAgent.SetDestination(_randomTarget);
        }
    }

    private Vector3 GenerateRandomTarget(Vector3 center, float range)
    {
        Vector3 randomPos = center + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }
}
