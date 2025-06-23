using System;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Movement Movement;
    private Animator _animator;
    private float _lastJumpTime;
    private float _jumpCooldown = 0.1F;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Movement.OnJump += HandleJump;
    }
    private void OnDisable()
    {
        Movement.OnJump -= HandleJump;
    }

    private void HandleJump()
    {
        _animator.SetTrigger("Jump");
        _lastJumpTime = Time.time;
    }

    private void Update()
    {
        _animator.SetBool("Walking", Movement.IsMoving);
        _animator.SetBool("HoldingWall", Movement.isGrabingwall);
        _animator.SetFloat("MoveSpeed", Movement.GetMoveSpeed);

        if (Time.time < _lastJumpTime + _jumpCooldown)
        {
            _animator.SetBool("Grounded", false);
        }
        else 
        {
            _animator.SetBool("Grounded", Movement.IsGrounded);
        }
    }
}
