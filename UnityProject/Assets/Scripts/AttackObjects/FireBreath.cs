using System.Threading;
using UnityEngine;

public class FireBreath : MonoBehaviour
{
    private GameObject player;
    private PlayerControll playerScript;
    private CardCast ccast;
    private Vector3 playerPos;
    private bool isFlip;
    private SpriteRenderer spriteRenderer;

    //효과음 반복 주기
    public float soundFreq = 0.5f;
    private float soundTime = 0;

    private void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerControll>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        ccast = GameObject.Find("CardCast").GetComponent<CardCast>();
        soundTime = soundFreq;
    }
    void FixedUpdate()
    {
        playerPos = player.transform.position;
        isFlip = playerScript.isFliped;
        if (isFlip)
        {
            spriteRenderer.flipX = false;
            transform.position = playerPos + Vector3.left * 2f;
        }
        else
        {
            spriteRenderer.flipX= true;
            transform.position = playerPos + Vector3.right * 2f;
        }

        soundTime += Time.deltaTime;
        if(soundTime >= soundFreq)
        {
            soundTime = 0;
            ccast.audioSource.PlayOneShot(ccast.attackSounds[4], 1.0f);
        }
    }
}
