using System.Collections;
using UnityEngine;

public class BossHealth : EnemyHealth
{
    public Animator Animator;
    //public Boss_V2 Boss;
    public string GameCompleteScene = "GameComplete"; // Name of the scene to load on death    

    private bool isDead = false;

    public override void Die()
    {
        if (isDead)
            return;
        isDead = true;

        //Boss.StopAll();
        Animator.SetTrigger("Die");
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation() 
    {
        yield return new WaitForSeconds(6);
        SceneLoader.Instance.LoadScene(GameCompleteScene);
    }
}
