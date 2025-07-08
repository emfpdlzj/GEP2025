using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float originalSpeed;
    Vector3 moveDirection = Vector3.zero;

    private bool isSlow = false;                //이동속도 저하 디버프 여부
    private float slowTime = 0;                 //이동속도 저하 시간 계산변수
    private float slowTimeSet = 0;
    
    private int batchId;
    public int BatchID
    {
        get { return batchId; }
        set { batchId = value; }
    }
    public int partitionGroup = 0;

    public int damage = 2;

    //보스몬스터 여부
    public bool isBoss = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //기본 이동속도 저장
        originalSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        // 디버프 관리
        if (isSlow)
        {
            slowTime += Time.deltaTime;
            if (slowTime >= slowTimeSet)
            {
                isSlow = false;
                moveSpeed = originalSpeed;
                slowTime = 0;
            }
        }

        //플레이어 위치로 이동
        moveDirection = GameManager.instance.player.transform.position - transform.position;
        moveDirection.Normalize();
        transform.position += moveDirection * Time.deltaTime * moveSpeed;

        
        //이동 방향에 따른 이미지 Flip
        if (moveDirection.x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        //파티션 그룹 최신화
        int newPartitionGroup = GameManager.instance.GetPartitionGroup(transform.position.x, transform.position.y);
        if(newPartitionGroup != partitionGroup)
        {
            GameManager.instance.enemyGroups[partitionGroup].Remove(this);
            partitionGroup = newPartitionGroup;
            GameManager.instance.enemyGroups[partitionGroup].Add(this);
        }
    }

    //Batch별로 실행할 로직 (1초당 한번)
    public void RunLogic()
    {
        PushOtherEnemy();
    }


    //주변 다른 적과 거리 유지

    private void PushOtherEnemy()
    {

        List<EnemyController> currAreaEnemies = GameManager.instance.enemyGroups[partitionGroup].ToList(); // ONLY enemies in the same spatial group

        // 같은 파티션 내 다른 적이 0.2 거리 안에 있으면 밀어냄
        foreach (EnemyController enemy in currAreaEnemies)
        {
            if (enemy == null) continue;
            if (enemy == this) continue;

            float distance = Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y);
            if (distance < 0.2f)
            {
                Vector3 direction = transform.position - enemy.transform.position;
                direction.Normalize();
                enemy.transform.position -= direction * Time.deltaTime * moveSpeed * 5;
            }
        }
    }


    public void KillEnemy()
    {
        GameManager.instance.UpdateBatchOnDeath("enemy", batchId);
        GameManager.instance.enemyGroups[partitionGroup].Remove(this);
        Destroy(gameObject);
    }

    
    public void TakeEffect(int type, float dur, float val)
    {
        switch (type)
        {
            case 0:
                break;
            //이동속도 * val
            case 1:
                slowTimeSet = dur;
                slowTime = 0;
                moveSpeed = originalSpeed * val;
                Debug.Log(moveSpeed);
                isSlow = true;
                break;
            case 2:
                slowTimeSet = dur;
                slowTime = 0;
                moveSpeed = 0;
                isSlow = true;
                break;
        }
    }
}
