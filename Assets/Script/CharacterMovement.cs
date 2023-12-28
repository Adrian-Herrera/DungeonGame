using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField]
    private float _maximumSpeed = 5f;
    [SerializeField]
    private float _jumpHeight = 2f;
    [SerializeField]
    private float _dashTime;
    [SerializeField]
    private float _dashSpeed;
    [SerializeField]
    private float _groundDistance = 0.2f;
    [SerializeField]
    private float _rotationSpeed = 720;
    [SerializeField]
    private bool _isGrounded = true;
    [SerializeField]
    private LayerMask Ground;
    [SerializeField]
    private Transform _groundChecker;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Vector3 Drag;

    private float Gravity = -9.81f;
    [SerializeField]
    private float ySpeed;
    private bool _isJumping;
    private Vector3 _velocity;
    private CharacterController _controller;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Jump();
        Attack();


        // _velocity.x /= 1 + Drag.x * Time.deltaTime;
        // _velocity.y /= 1 + Drag.y * Time.deltaTime;
        // _velocity.z /= 1 + Drag.z * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
        float speed = inputMagnitude * _maximumSpeed;
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();
        _velocity = movementDirection * speed;

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }
    }
    private void Jump()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, _groundDistance, Ground, QueryTriggerInteraction.Ignore);
        animator.SetBool("IsGrounded", _isGrounded);
        if (_isGrounded && ySpeed < 0)
        {
            ySpeed = 0f;
            // animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            _isJumping = false;
        }
        // else
        // {
        //     if ((_isJumping && ySpeed < 0) || ySpeed < -2)
        //     {
        //         animator.SetBool("IsFalling", true);
        //     }
        // }
        ySpeed += Gravity * Time.deltaTime;
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            ySpeed = Mathf.Sqrt(_jumpHeight * -2f * Gravity);
            animator.SetTrigger("Jump");
        }
        _velocity.y = ySpeed;
    }
    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
            // _velocity += Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
            // StartCoroutine(DashCoroutine());
        }
    }
    private void Dash()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Dash");
            // _velocity += Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
            // StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time; // need to remember this to know how long to dash
        while (Time.time < startTime + _dashTime)
        {
            _controller.Move(transform.forward * _dashSpeed * Time.deltaTime);
            // or controller.Move(...), dunno about that script
            yield return null; // this will make Unity stop here and continue next frame
        }
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.DrawSphere(_groundChecker.position, _groundDistance);
        if (_isGrounded)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {

            Gizmos.color = Color.red;
        }
    }
}
