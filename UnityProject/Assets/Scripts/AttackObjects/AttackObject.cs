using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using static AttackObject;
using UnityEngine.Diagnostics;

public class AttackObject : MonoBehaviour
{
    //파티션 그룹
    private int partitionGroup = 0;

    //공격 오브젝트의 이동 방향
    private Vector3 moveDirection = Vector3.zero;

    public bool isEffectOnly = false;   // 데미지 없고 효과만 있는 경우
    public int effectType = 0;          // 공격 효과 ID                     1 : 이동속도 감소     2 : 기절
    public float effectDuration = 0;    // 공격 효과 지속시간
    public float effectValue = 0;       // 공격 효과 값 (둔화율)

    public float moveSpeed;             //이동속도 (움직이는 경우)
    public float hitBoxRadius;          //히트박스 크기
    private int partitionCheckRadius;   //주변 파티션 체크 범위

    //주변 파티션 그룹
    List<int> surroundPartitionGroup = new List<int>();

    private bool isDestroyed = false;
    public bool isHit = false;

    public bool isDestroyOnHit = false;
    private float durationTime = 0f;
    //공격 오브젝트의 지속시간 (해당 시간 경과 후 삭제)
    public float durationTimeSet = 1f; 

    private void Start()
    {
        partitionGroup = GameManager.instance.GetPartitionGroup(transform.position.x, transform.position.y);
        GameManager.instance.attackObjectGroups[partitionGroup].Add(this);

        partitionCheckRadius = (int)hitBoxRadius + 1;
        surroundPartitionGroup = Partition.GetExpandedPartitionGroups(partitionGroup, partitionCheckRadius);
    }

    private void FixedUpdate()
    {
        //이동 방향 (AttackObject의 Z각도 방향)
        float rad = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        moveDirection = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

        //이동
        transform.position += moveDirection * Time.deltaTime * moveSpeed;

        //맵 벗어나면 제거
        if(transform.position.x <= -50 || transform.position.x >= 50)
        {
            if (transform.position.y <= -50 || transform.position.y >= 50)
                DestroyAttackObject();
        }


        //파티션 그룹 업데이트
        int newParitionGroup = GameManager.instance.GetPartitionGroup(transform.position.x, transform.position.y);
        if (newParitionGroup != partitionGroup)
        {
            GameManager.instance.attackObjectGroups[partitionGroup].Remove(this);
            partitionGroup = newParitionGroup;
            GameManager.instance.attackObjectGroups[partitionGroup].Add(this);
            surroundPartitionGroup = Partition.GetExpandedPartitionGroups(partitionGroup, partitionCheckRadius);
        }

        //적과 충돌 계산
        CheckCollisionWithEnemy();


        //지속시간이 지나면 제거
        durationTime += Time.deltaTime;
        if (durationTime > durationTimeSet)
        {
            durationTime = 0f;
            DestroyAttackObject();
        }
    }

    private void CheckCollisionWithEnemy()
    {
        List<EnemyController> surroundEnemies = Partition.GetAllEnemiesInPartitionGroups(surroundPartitionGroup);

        foreach (EnemyController enemy in surroundEnemies)
        {
            if (enemy == null) continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < hitBoxRadius)
            {
                if (!isEffectOnly)
                {
                    // 공격 효과가 존재 (0이 아님)하면 효과 적용
                    if(effectType != 0)
                        enemy.TakeEffect(effectType, effectDuration, effectValue);

                    if (enemy.isBoss)
                    {
                        enemy.GetComponent<BossController>().TakeDamage(-1);
                    }
                    else
                    {
                        // 적 제거
                        enemy.KillEnemy();
                        isHit = true;
                    }
                }
                else
                {
                    enemy.TakeEffect(effectType, effectDuration, effectValue);
                }
                // 적과 닿으면 사라지는 오브젝트라면 삭제
                if (isDestroyOnHit)
                {
                    DestroyAttackObject();
                    break;
                }
            }
        }
    }

    private void DestroyAttackObject()
    {
        if (isDestroyed) return;

        //공격 오브젝트 파티션 그룹에서 삭제
        GameManager.instance.attackObjectGroups[partitionGroup].Remove(this);
        isDestroyed = true;
        Destroy(gameObject);
    }
}
