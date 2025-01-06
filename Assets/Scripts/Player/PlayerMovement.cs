using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("===== Move Settings =====")]
    [Space(10)]
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("===== Dash Settings =====")]
    [Space(10)]
    [SerializeField] private float dashForce = 5.0f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashCooldown = 0.1f;
    
    [Header("===== Animator =====")]
    [Space(10)]
    public Animator anim;

    private PlayerInput _playerInput;
    private InputAction _movement;
    private InputAction _run;
    private InputAction _dash;
    private InputAction _look;
    private InputAction _control;

    private bool _isMove;
    private bool _isRun;
    private bool _isDashing;
    private float _lastDashTime;
    private Vector3 _rawInputMovement;
    private Vector3 _rawInputLook;

    private PlayerShooting _playerShooting;
    private int _weaponNum;
    

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _playerInput = GetComponent<PlayerInput>();
        _movement = _playerInput.actions["Movement"];
        _run = _playerInput.actions["Run"];
        _look = _playerInput.actions["Look"];
        _control = _playerInput.actions["Control"];
        _dash = _playerInput.actions["Dash"];
        
        _control.performed += ctx => ShowControlMenu();
        _dash.performed += ctx => Dash();
        
        _playerShooting = transform.GetComponent<PlayerShooting>();
    }

    void Update()
    {
        if (!GameManager.Instance.isOver)
        {
            OnMovement();
            OnLook();
            SwitchAnim();
        }
        
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isOver)
        {
            RotateMove();
        }
    }

    public void OnMovement()
    {
        Vector2 inputMovement = _movement.ReadValue<Vector2>();
        _rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
        _isMove = _rawInputMovement.sqrMagnitude > 0.01f;

        float run = _run.ReadValue<float>();
        if (run > 0)
        {
            movementSpeed = 5.0f;
            _isRun = true;
        }
        else
        {
            movementSpeed = 3.0f;
            _isRun = false;
        }
    }

    public void OnLook()
    {
        Vector2 inputLook = _look.ReadValue<Vector2>();

        float sensitivity = 1.0f; // 设置旋转灵敏度
        float lookX = inputLook.x * sensitivity;
        float lookY = inputLook.y * sensitivity;

        Vector3 lookDirection = new Vector3(lookX, 0, lookY);
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
    

    private void RotateMove()
    {
        if (_rawInputMovement == Vector3.zero)
            return;

        Vector3 newPosition = transform.position + _rawInputMovement.normalized * (movementSpeed * Time.deltaTime);
        _rb.MovePosition(newPosition);

        Quaternion targetRotation = Quaternion.LookRotation(_rawInputMovement, Vector3.up);
        _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void ShowControlMenu()
    {
        GameManager.Instance.ControlMenu();
    }
    
    private void Dash()
    {
        if (!_isDashing && Time.time >= _lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        _isDashing = true;
        _playerShooting.isDash = true;
        AudioManager.Instance.PlayFX("Rolling");
        
        Vector3 dashDirection = _rawInputMovement.normalized;
        if (dashDirection == Vector3.zero) // 防止没有移动输入时 Dash
        {
            dashDirection = transform.forward;
        }

        _rb.AddForce(dashDirection * dashForce, ForceMode.Impulse); // 使用 AddForce 添加冲力

        yield return new WaitForSeconds(dashTime); // 等待 dashTime 秒

        _playerShooting.isDash = false;
        _isDashing = false;
        _lastDashTime = Time.time;
    }
    
    private void SwitchAnim()
    {
        if (_isDashing)
        {
            StartCoroutine(DashAnim());
            return;
        }
        if (!_isMove)
        {
            anim.Play("Happy Idle");
        }
        else if (_isRun)
        {
            anim.Play("running");
        }
        else
        {
            anim.Play("moving");
        }

        _weaponNum = _playerShooting.weaponNum;

        if (_weaponNum == 0)
        {
            SetAnimatorLayerWeight("Base Layer", 1f); 
            SetAnimatorLayerWeight("ShootA", 1f); 
            SetAnimatorLayerWeight("ShootB", 0f); 
            SetAnimatorLayerWeight("Dash", 0f); 
        }
        if (_weaponNum == 1)
        {
            SetAnimatorLayerWeight("Base Layer", 1f); 
            SetAnimatorLayerWeight("ShootA", 1f); 
            SetAnimatorLayerWeight("ShootB", 1f); 
            SetAnimatorLayerWeight("Dash", 0f); 
        }

        if (_playerShooting.startShoot)
        {
            anim.SetBool("isAttack",true);
        }
        else
        {
            anim.SetBool("isAttack",false);
        }
    }
    
    private void SetAnimatorLayerWeight(string layerName, float weight)
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        if (layerIndex != -1)
        {
            anim.SetLayerWeight(layerIndex, weight);
        }
    }

    IEnumerator DashAnim()
    {
        anim.SetTrigger("isDash");
        SetAnimatorLayerWeight("Base Layer", 1f); 
        SetAnimatorLayerWeight("ShootA", 0f); 
        SetAnimatorLayerWeight("ShootB", 0f);
        yield return new WaitForSeconds(2f);
    }
}
