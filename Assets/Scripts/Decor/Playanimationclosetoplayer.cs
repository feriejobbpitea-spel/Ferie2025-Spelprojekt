using UnityEngine;

public class Playanimationclosetoplayer : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component


        void Start()
        {

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                animator.SetTrigger("Enter");
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                animator.SetTrigger("Exit");
            }
        }
}
