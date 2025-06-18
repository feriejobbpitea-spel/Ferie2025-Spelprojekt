using UnityEditor.Rendering.LookDev;
using UnityEngine;
using System.Collections;
public class CameraFollow : Singleton<CameraFollow>
{
    Transform target; 
    public float smoothSpeed = 0.125f;
    public Vector3 offset;        // Justera om du vill se spelaren lite till vänster/höger i bild
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform; // Hitta spelaren med taggen "Player"    
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
        }
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void TriggerShake(float duration = 0.1f, float magnitude = 0.1f)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private void OnValidate()
    {
        Vector3 pos = transform.position;
        if(GameObject.FindGameObjectWithTag("Player")) 
            pos = GameObject.FindGameObjectWithTag("Player").transform.position + offset; // Uppdatera positionen baserat på spelarens position

        transform.position = new Vector3(pos.x, pos.y, -10f); // Sätt z-positionen för att undvika problem med 2D-kameran
    }
}
