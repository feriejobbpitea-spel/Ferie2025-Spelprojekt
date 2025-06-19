using UnityEngine;
public class BossBackHitbox : MonoBehaviour
{
    public Boss bossScript;         // Referens till boss-skriptet
    public LayerMask projectileLayer;  // Välj Projectiles-lagret i Inspector

    private void OnTriggerEnter2D(Collider2D projectileLayer)
    {
        
        
            Debug.Log("Boss hit by projectile: ");
            bossScript.TakeDamage(1);
        
    }
}