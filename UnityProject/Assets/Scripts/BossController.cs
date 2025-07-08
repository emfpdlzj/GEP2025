using UnityEngine;

public class BossController : MonoBehaviour
{
    //체력
    public int health = 10;

    //피격시 무적시간
    private float invTime = 0;
    private float invTimeSet = 1.2f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int val)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            spriteRenderer.color = Color.gray;
            health += val;
        }
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            GetComponent<EnemyController>().KillEnemy();
            GameManager.instance.GetComponent<EnemySpawnManager>().NextWave();
        }

        if (isInvincible)
        {
            invTime += Time.deltaTime;
            if (invTime > invTimeSet)
            {
                invTime = 0;
                isInvincible = false;
                spriteRenderer.color = Color.white;
            }
        }
    }
}
