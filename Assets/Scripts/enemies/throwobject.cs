using UnityEngine;

public class throwobject : MonoBehaviour
{
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // F�rst�r f�rem�let efter X sekunder
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("golv"))
        {
            print(collision.gameObject);
            Destroy(gameObject); // F�rst�r vid tr�ff
        }
    }



}



    


