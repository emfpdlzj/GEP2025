using UnityEngine;

public class Mose : MonoBehaviour
{
    private void Start()
    {
        if (transform.rotation == Quaternion.Euler(0,0,180))
        {
            GetComponent<SpriteRenderer>().flipY = true;
        }
    }
}
