using UnityEngine;

public class PortCloser : MonoBehaviour
{
    [Header("Portinställningar")]
    public Transform port;           // Det objekt som ska röra sig (porten)
    public float dropDistance = -3f; // Negativt värde = hur långt ner porten ska gå
    public float speed = 2f;         // Hur snabbt porten stängs

    [Header("Ljud")]
    public AudioSource audioSource;
    public AudioClip closingSound;

    private bool isClosing = false;
    private float startY;
    private float targetY;
    private bool hasMoved = false;

    private void Start()
    {
        if (port == null)
        {
            enabled = false;
            return;
        }

        startY = port.position.y;
        targetY = startY + dropDistance;

        if (audioSource != null && closingSound != null)
        {
            audioSource.clip = closingSound;
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        if (!isClosing) return;

        if (Mathf.Abs(port.position.y - targetY) > 0.01f)
        {
            // Spela ljud om det inte redan spelas
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();

            port.position = Vector3.MoveTowards(port.position, new Vector3(port.position.x, targetY, port.position.z), speed * Time.deltaTime);
        }
        else
        {
            // Stäng av ljudet och avsluta
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            isClosing = false;
        }
    }

    public void ClosePort()
    {
        if (hasMoved) return; // Förhindra att porten stängs igen

        isClosing = true;
        hasMoved = true;
    }

    private void OnDrawGizmos()
    {
        if (port == null) return;

        Gizmos.color = Color.cyan;
        Vector3 targetPos = port.position;
        targetPos.y = port.position.y + dropDistance;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
    }
}
