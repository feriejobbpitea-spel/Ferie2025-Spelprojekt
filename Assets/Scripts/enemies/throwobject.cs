using UnityEditor;
using UnityEngine;

public class throwobject : MonoBehaviour
{
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

   
   
}





