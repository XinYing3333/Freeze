using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3.0f;
    [SerializeField] float turnSpeed = 10f;

    [Header("Input Settings")]
    public PlayerInput playerInput;
    public Animator anim;

    private InputAction _movement;
    private InputAction _run;

    private bool _isMove;
    private bool _isRun;
    private Vector3 _rawInputMovement;
    private InputAction _look;
    private Vector3 _rawInputLook;

    private PlayerShooting _playerShooting;
    private int _weaponNum;

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        _movement = playerInput.actions.FindAction("Movement");
        _run = playerInput.actions.FindAction("Run");
        _look = playerInput.actions.FindAction("Look");
        
        GameObject player = GameObject.FindWithTag("Player");
        _playerShooting = player.GetComponent<PlayerShooting>();
    }

    void Update()
    {
        OnMovement();
        OnLook();
        SwitchAnim();
    }

    void FixedUpdate()
    {
        RotateMove();
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
        // 根据输入值进行角色旋转
        float sensitivity = 1.0f; // 设置旋转灵敏度
        float lookX = inputLook.x * sensitivity;
        float lookY = inputLook.y * sensitivity;

        // 根据需要应用水平和垂直旋转
        // 这里只展示了水平旋转，你也可以添加垂直旋转
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
    
    private void SwitchAnim()
    {
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
            SetAnimatorLayerWeight(0, 1f); 
            SetAnimatorLayerWeight(1, 1f); 
            SetAnimatorLayerWeight(2, 0f); 
        }
        if (_weaponNum == 1)
        {
            SetAnimatorLayerWeight(0, 1f); 
            SetAnimatorLayerWeight(1, 1f); 
            SetAnimatorLayerWeight(2, 1f); 
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
    
    private void SetAnimatorLayerWeight(int layerIndex, float weight)
    {
        if (layerIndex < anim.layerCount)
        {
            anim.SetLayerWeight(layerIndex, weight);
        }
    }
}
