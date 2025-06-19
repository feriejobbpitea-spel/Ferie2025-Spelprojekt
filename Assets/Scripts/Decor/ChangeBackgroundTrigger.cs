using UnityEngine;

public class ChangeBackgroundTrigger : MonoBehaviour
{
    public Color NewBackgroundColor;
    public GameObject[] ToEnable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Camera.main.backgroundColor = NewBackgroundColor;
            foreach (var item in ToEnable)
            {
                if(item != null)
                {
                    item.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var item in ToEnable)
            {
                if(item != null)
                {
                    item.SetActive(false);
                }
            }
        }
    }
}
