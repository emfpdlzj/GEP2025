using UnityEngine;

public class BloodLord : MonoBehaviour
{
    // 시전시 체력 10 감소
    // 발사체가 하나 맞을 떄 마다 2씩 회복


    public GameObject projectile;
    private GameObject player;
    private float timeToReady = 0;

    private CardCast ccast;
    void Start()
    {
        player = GameObject.Find("Player");
        player.GetComponent<PlayerControll>().ModifyHealth(-10);

        ccast = GameObject.Find("CardCast").GetComponent<CardCast>();
        ccast.audioSource.PlayOneShot(ccast.attackSounds[5], 1.0f);
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position;
        timeToReady += Time.deltaTime;

        if(timeToReady > 1f)
        {
            ccast.audioSource.PlayOneShot(ccast.attackSounds[6], 1.0f);
            for (int i = 1; i < 18; i++)
            {
                Instantiate(projectile, player.transform.position, Quaternion.Euler(0, 0, i * 20));
            }
            Destroy(gameObject);
        }
    }
}
