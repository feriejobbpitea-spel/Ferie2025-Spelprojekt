using UnityEngine;

public class Boss_Animations : MonoBehaviour
{
    [SerializeField] private Boss boss;
    private Animator _animator;

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (boss == null) return;

        boss.OnFly += HandleFly;
        boss.OnSlam += HandleSlam;
        boss.OnThrow += HandleThrow;
    }

    private void OnDisable()
    {
        if (boss == null) return;

        boss.OnFly -= HandleFly;
        boss.OnSlam -= HandleSlam;
        boss.OnThrow -= HandleThrow;
    }

    private void HandleFly()
    {
        _animator.SetTrigger("Fly");
    }

    private void HandleSlam()
    {
        _animator.SetTrigger("Slam");
    }

    private void HandleThrow()
    {
        _animator.SetTrigger("Throw");
    }
}
