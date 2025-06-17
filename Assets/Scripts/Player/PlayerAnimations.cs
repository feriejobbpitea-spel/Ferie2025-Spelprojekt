using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Movement Movement;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool("Walking", Movement.IsMoving);
        _animator.SetFloat("MoveSpeed", Movement.GetMoveSpeed);
    }
}
