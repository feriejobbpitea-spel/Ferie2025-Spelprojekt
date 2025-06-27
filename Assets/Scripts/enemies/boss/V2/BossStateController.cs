using UnityEngine;
using System.Collections;

public class BossStateController : MonoBehaviour
{
    public enum BossState { Idle, Attacking, Vulnerable, Dead }

    private Transform player;
    public BossAttackHandler attackHandler;
    public BossVulnerabilityHandler vulnerabilityHandler;
    public GameObject bossHealthBar;
    public float agroRange = 10f;

    private BossState currentState = BossState.Idle;
    private bool hasSeenPlayer = false;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        bossHealthBar.SetActive(false);
        StartCoroutine(StateMachine());
    }

    private IEnumerator StateMachine()
    {
        while (currentState != BossState.Dead)
        {
            switch (currentState)
            {
                case BossState.Idle:
                    yield return IdleState();
                    break;
                case BossState.Attacking:
                    yield return AttackState();
                    break;
                case BossState.Vulnerable:
                    yield return VulnerableState();
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator IdleState()
    {
        if (Vector2.Distance(transform.position, player.position) <= agroRange)
        {
            bossHealthBar.SetActive(true);
            hasSeenPlayer = true;
        }

        if (hasSeenPlayer)
            currentState = BossState.Attacking;

        yield return null;
    }

    private IEnumerator AttackState()
    {
        yield return attackHandler.PerformRandomAttack();

        currentState = BossState.Idle;
    }

    private IEnumerator VulnerableState()
    {
        yield return vulnerabilityHandler.StartVulnerability();

        currentState = BossState.Idle;
    }

    public void SetState(BossState newState)
    {
        currentState = newState;
    }

    public BossState GetCurrentState()
    {
        return currentState;
    }
}
