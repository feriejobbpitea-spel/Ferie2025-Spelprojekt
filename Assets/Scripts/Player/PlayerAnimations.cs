using System;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Movement Movement;
    private Animator _animator;

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
    }

    private void Update()
    {
        _animator.SetBool("Walking", Movement.IsMoving);
        _animator.SetFloat("MoveSpeed", Movement.GetMoveSpeed);
        _animator.SetBool("Grounded", Movement.IsGrounded);
    }
}
