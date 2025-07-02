using UnityEngine;

public class AutoDestroyAfterAnimation : MonoBehaviour
{
    public float lifetime = 1.0f;

    void Start()
    {
        Destroy(gameObject, lifetime); // Eller SetActive(false) om du använder object pooling
    }
}
