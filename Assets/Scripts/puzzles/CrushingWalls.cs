using UnityEngine;

public class CrushingWalls : MonoBehaviour
{
    public Transform ceiling;
    public float floorY = -3f; // Y-positionen som taket ska stanna vid
    public float speed = 2f;
    private bool isActivated = false;

    private Vector2 _startYPos;

    private void Start()
    {
        _startYPos.y = ceiling.localPosition.y - floorY;
    }

    void Update()
    {
        if (!isActivated) return;

        ceiling.position = Vector2.MoveTowards(ceiling.position, new Vector2(ceiling.localPosition.x, _startYPos.y), speed * Time.deltaTime);
    }

    public void ActivateWalls()
    {
        isActivated = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 truePos = ceiling.position;
        truePos.y = floorY;
        Gizmos.DrawWireSphere(truePos, 0.5f);
        //Gizmos.DrawLine(new Vector2(ceiling.position.x - 5, floorY), new Vector2(ceiling.position.x + 5, floorY));
    }
}