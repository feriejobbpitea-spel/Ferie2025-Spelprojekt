using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MoneyHolder moneyHolder = FindObjectOfType<MoneyHolder>();
            if (moneyHolder != null)
            {
                moneyHolder.AddMoney(coinValue);
            }

            Destroy(gameObject); // Coin försvinner
        }
    }
}
