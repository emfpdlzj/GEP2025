using UnityEngine;

public class StickyBomb : MonoBehaviour
{
    private float bombTimer = 0;
    private float bombTimerSet = 3f;

    public GameObject bomb;
    private CardCast ccast;

    private void Start()
    {
        ccast = GameObject.Find("CardCast").GetComponent<CardCast>();

        ccast.audioSource.PlayOneShot(ccast.attackSounds[9], 1.0f);
    }
    void FixedUpdate()
    {
        bombTimer += Time.deltaTime;

        if(bombTimer >= bombTimerSet)
        {
            ccast.audioSource.Stop();
            ccast.audioSource.PlayOneShot(ccast.attackSounds[10], 1.0f);
            Instantiate(bomb, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
