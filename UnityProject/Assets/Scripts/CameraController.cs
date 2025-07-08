using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        Vector3 playerPos = player.transform.position;
        float halfCameraSizeY = Camera.main.orthographicSize;
        float halfCameraSizeX = halfCameraSizeY * Screen.width / Screen.height;
        Vector3 cameraPos = new Vector3(Mathf.Clamp(playerPos.x, -50.0f + halfCameraSizeX, 50.0f - halfCameraSizeX), Mathf.Clamp(playerPos.y, -50.0f + halfCameraSizeY, 50.0f - halfCameraSizeY), transform.position.z);
        transform.position = cameraPos;
    }
}
