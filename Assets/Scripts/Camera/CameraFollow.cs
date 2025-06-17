using UnityEngine;

public class CameraFollow : MonoBehaviour
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
}
