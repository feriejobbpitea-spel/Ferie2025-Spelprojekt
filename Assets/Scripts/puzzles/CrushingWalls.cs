using UnityEngine;
using System.Collections;

public class CrushingWalls : MonoBehaviour
{
    public Transform ceiling;
    public float floorY = -3f; // Taket ska ner hit
    public float speed = 2f;
    public bool resetwalls = true; // Om väggarna ska återställas efter en tid

    private bool isActivated = false;

    private float startY; // Spara startpositionens Y

    private void Start()
    {
        startY = ceiling.position.y; // Spara initial Y-position
    }

    void Update()
    {
        if (!isActivated) return;

        // Flytta neråt mot floorY
        Vector3 targetPosition = new Vector3(ceiling.position.x, startY - floorY, ceiling.position.z);
        ceiling.position = Vector3.MoveTowards(ceiling.position, targetPosition, speed * Time.deltaTime);
    }

    public void ActivateWalls()
    {
        if (!isActivated && resetwalls)
            StartCoroutine(MoveAndReset());
    }

    IEnumerator MoveAndReset()
    {
        isActivated = true;

        // Vänta tills taket nått botten
        while (ceiling.position.y > startY - floorY) // Liten tolerans för flyttal
        {
            yield return null;
        }

        // Vänta 5 sekunder innan reset
        yield return new WaitForSeconds(5);

        // Återställ taket till startposition
        ceiling.position = new Vector3(ceiling.position.x, startY, ceiling.position.z);
        isActivated = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 targetPos = ceiling.position;
        targetPos.y = floorY;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
    }
}