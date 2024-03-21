using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Subject
{
#region Private Fields    
    PlayerControl _inputs;
    Vector2 _move;
    Camera _camera;
    Vector3 _camForward, _camRight;
#endregion    

#region Serialize Fields

    [SerializeField] float _speed;

    [Header("Joystick")]
    [SerializeField] Joystick _joystick;
    [Header("Character Controller")]
    [SerializeField] CharacterController _controller;

    [Header("Movement")]
    [SerializeField] float _gravity = -30.0f;
    [SerializeField] float _jumpHeight = 3.0f;
    [SerializeField] Vector3 _velocity;

    [Header("Ground Detection")]
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groundRadius = 0.5f;
    [SerializeField] LayerMask _groundMaks;
    [SerializeField] bool _isGrounded;

    [Header("Respawn Transform")]

    [SerializeField] Transform _respawn;
#endregion

    public object Player { get; internal set; }

    void Awake()
    {
        _camera = Camera.main;
        _controller = GetComponent<CharacterController>();
        _inputs = new PlayerControl();
        _inputs.Player.Move.performed += context => _move = context.ReadValue<Vector2>();
        _inputs.Player.Move.canceled += ctx => _move = Vector2.zero;
        _inputs.Player.Jump.performed += ctx => Jump();
    }

    void OnEnable()
    {
        _inputs.Enable();
    }

    void OnDisable() => _inputs.Disable();

void FixedUpdate()
{
    _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundRadius, _groundMaks);
    if (_isGrounded && _velocity.y < 0.0f)
    {
        _velocity.y = -2.0f;
    }
#if !UNITY_EDITOR
    _move = _joystick.Direction;
#endif
    _camForward = _camera.transform.forward;
    _camRight = _camera.transform.right;
    _camForward.y = 0.0f;
    _camRight.y = 0.0f;
    _camForward.Normalize();
    _camRight.Normalize();
    Vector3 movement = (_camRight * _move.x + _camForward * _move.y) * _speed * Time.fixedDeltaTime;
     if (!_controller.enabled) {return;}
    _controller.Move(movement);
    _velocity.y += _gravity * Time.fixedDeltaTime;
    _controller.Move(_velocity * Time.fixedDeltaTime);
}    

void OnDrawGizmos()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(_groundCheck.position, _groundRadius);
}

void Jump()
{
    if (_isGrounded)
    {
        _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        NotifyObservers(PlayerEnums.Jump);
    }
}
    void DebugMessage(InputAction.CallbackContext context)
    {
        Debug.Log($"Move Performed {context.ReadValue<Vector2>().x}, {context.ReadValue<Vector2>().y}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag ("deathZone"))
        {
            MovePlayerPosition(_respawn.position);
            NotifyObservers(PlayerEnums.Died);
        }
    }

    public void MovePlayerPosition(Vector3 pos)
    {
        _controller.enabled = false;
        transform.position = pos;
        _controller.enabled = true;
    }
}
