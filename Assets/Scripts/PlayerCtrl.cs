using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float turnSpeed = 10f;

    [Header("Input Settings")]
    public PlayerInput playerInput;
    private InputAction _movement;
    private Vector3 _rawInputMovement;
    private InputAction _look;
    private Vector3 _rawInputLook;

    private Animator _anim;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        _movement = playerInput.actions.FindAction("Movement");
        _look = playerInput.actions.FindAction("Look");
    }

    void Update()
    {
        OnMovement();
        OnLook();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void OnMovement()
    {
        Vector2 inputMovement = _movement.ReadValue<Vector2>();
        _rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
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

    private void Move()
    {
        if (_rawInputMovement == Vector3.zero)
            return;

        Vector3 newPosition = transform.position + _rawInputMovement.normalized * (movementSpeed * Time.deltaTime);
        _rb.MovePosition(newPosition);

        Quaternion targetRotation = Quaternion.LookRotation(_rawInputMovement, Vector3.up);
        _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}
