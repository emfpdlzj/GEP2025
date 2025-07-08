using UnityEngine;

public class VerticalSlash : MonoBehaviour
{
    private float timer = 0;

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if(timer >= 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
