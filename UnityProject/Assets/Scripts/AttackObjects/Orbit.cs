using UnityEngine;

public class Orbit : MonoBehaviour
{
    private GameObject player;
    private float radius = 4;
    private float deg = 0;
    public float rotateSpeed = 3f;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        deg += Time.deltaTime * rotateSpeed;

        if(deg <= 360)
        {
            float radian = Mathf.Rad2Deg * (deg);
            float x = radius * Mathf.Sin(radian);
            float y = radius * Mathf.Cos(radian);
            transform.position = player.transform.position + new Vector3(x, y);
        }
    }
}
