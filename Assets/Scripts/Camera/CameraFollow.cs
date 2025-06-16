using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Dra in spelaren h�r
    public float smoothSpeed = 0.125f;
    public Vector3 offset;        // Justera om du vill se spelaren lite till v�nster/h�ger i bild

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
