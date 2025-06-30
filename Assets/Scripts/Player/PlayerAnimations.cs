using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Movement Movement;
    private Animator _animator;
    private float _lastJumpTime;
    private float _jumpCooldown = 0.1f;

    private GameObject _lastWeapon;

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

        bool isAttacking = _animator.GetBool("isAttacking");

        var currentWeapon = InventoryManager.Instance?.currentWeapon;

        bool isHoldingMelee = currentWeapon != null &&
            currentWeapon.name.Contains("MeleAttack", System.StringComparison.OrdinalIgnoreCase);

        // 🔁 Kolla om vapnet ändrats
        if (_lastWeapon != currentWeapon)
        {
            _lastWeapon = currentWeapon;

            // Om man INTE längre håller melee → stäng av MeleeLayer
            if (!isHoldingMelee)
            {
                int meleeLayerIndex = _animator.GetLayerIndex("MeleeLayer");
                if (meleeLayerIndex >= 0)
                {
                    _animator.SetLayerWeight(meleeLayerIndex, 0f);
                    isAttacking = false; // Stäng av attack-animationer också
                    _animator.SetBool("isAttacking", false);
                }
            }
        }

        // Vanlig Mele-lager (ex: idle med svärd när ej attackerar)
        bool isMeleLayerActive = isHoldingMelee && !isAttacking;
        _animator.SetBool("IsMeleActive", isMeleLayerActive);

        int meleLayerIndex = _animator.GetLayerIndex("Mele");
        if (meleLayerIndex >= 0)
        {
            _animator.SetLayerWeight(meleLayerIndex, isMeleLayerActive ? 1f : 0f);
        }

        // MeleeLayer = attack-animationer, aktiv bara när attackerar
        int meleeLayerAttack = _animator.GetLayerIndex("MeleeLayer");
        if (meleeLayerAttack >= 0 && isHoldingMelee)
        {
            _animator.SetLayerWeight(meleeLayerAttack, isAttacking ? 1f : 0f);
        }
    }
}
