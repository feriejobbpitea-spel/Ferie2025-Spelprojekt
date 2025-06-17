using UnityEngine;
using System.Collections;
public class boss : MonoBehaviour
{
    public float jumpForce = 10f;
public float slamDelay = 1.5f;
public GameObject earthWavePrefab;
public Transform groundCheck;

private Rigidbody2D rb;
private bool isGrounded;
private bool isAttacking;

void Start()
{
    rb = GetComponent<Rigidbody2D>();
}

void Update()
{
    if (!isAttacking && Input.GetKeyDown(KeyCode.Space)) // Byt ut triggern till vad du vill
    {
        StartCoroutine(SlamAttack());
    }
}

private IEnumerator SlamAttack()
{
    isAttacking = true;

    // Steg 1: Hoppa upp
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    // Vänta i luften
    yield return new WaitForSeconds(slamDelay);

    // Steg 2: Slå ner snabbt
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, -jumpForce * 2);

    // Vänta tills den landar
    yield return new WaitUntil(() => IsGrounded());

    // Steg 3: Skapa jordvågen
    Instantiate(earthWavePrefab, groundCheck.position, Quaternion.identity);

    isAttacking = false;
}

private bool IsGrounded()
{
    RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f);
    return hit.collider != null;
}
}
   