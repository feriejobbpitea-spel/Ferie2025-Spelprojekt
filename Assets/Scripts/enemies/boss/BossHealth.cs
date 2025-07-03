using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BossHealth : EnemyHealth
{
    public Animator Animator; // Sätt i Unity Inspector
    public string GameCompleteScene = "GameComplete"; // Scene att ladda vid död    
    public string AltEnding = "GameAltEnding"; // Scene att ladda vid död    

    private bool isDead = false;

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        BossStateController stateController = transform.parent.GetComponent<BossStateController>();
        if (stateController != null)
        {
            stateController.SetState(BossStateController.BossState.Dead);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            col.enabled = false;
        }

        if (Animator != null)
        {
            Animator.ResetTrigger("Hit");
            Animator.SetTrigger("Die");
        }

        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer(); // Sparar automatiskt bästa tid
        }

        StartCoroutine(WaitForDeathAnimation());
    }

    public bool IsDead()
    {
        return isDead;
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(3f); // Vänta på animationen

        if (InventoryManager.Instance.HasAllItemsAndPhoto())
        {
            SceneLoader.Instance.LoadScene(AltEnding);
        }
        else
        {
            SceneLoader.Instance.LoadScene(GameCompleteScene);
        }
    }
}