using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    
    private bool _collected =false;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_collected == true)
                return;
            _collected = true;

            _animator.SetTrigger("Collected");

            MoneyHolder moneyHolder = FindObjectOfType<MoneyHolder>();
            if (moneyHolder != null)
            {
                moneyHolder.AddMoney(coinValue);
            }
        }
    }
}
