using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Diagnostics;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;  
public class PlayerControll : MonoBehaviour
{
    //플레이어 스탯
    [SerializeField] private float health = 100f; // ✅ 누락된 선언 추가
    [SerializeField] private float maxhealth = 100f;
    public float Health => health;
    public float MaxHealth => maxhealth; // 필요시 따로 관리
    public float moveSpeed = 3f;
    private float basicMoveSpeed = 3f;  //기본 이동 속도

    //애니메이션 변수
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool isFliped;

    //Partition Group
    private int partitionGroup = -1;
    [HideInInspector]
    public int PartitionGroup { get { return partitionGroup; } }

    //피격 관련 변수
    private int takeDamageEachFrame = 0;
    private int takeDamageEachFrameSet = 10;
    private float hitBoxRadius = 0.5f;              //히트박스 크기 (플레이어 기준 반지름)
    [HideInInspector]
    public bool isInvincible = false;               //무적 여부 (고속 이동 등 카드에 사용)
    [HideInInspector]
    public bool isStar = false;                     //무적 시간중 적과 충돌 시 적 처치 여부
    [HideInInspector]
    public bool isOnMed = false;                    //명상 카드 시전중인지 확인
    [HideInInspector]
    public bool isMedInv = false;                   //명상 무적 확인

    //코루틴 변수 (중복 사용 방지)
    private Coroutine setMoveSpeed;
    private Coroutine hitAnamation;

    [HideInInspector]
    public GameObject trail;                        //이동속도 증가 이펙트

    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //파티션 그룹 초기화
        partitionGroup = GameManager.instance.GetPartitionGroup(transform.position.x, transform.position.y);

        //기본 이동 속도 확인
        basicMoveSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        PlayerMove();



        //플레이어의 현재 파티션 그룹 계산
        partitionGroup = GameManager.instance.GetPartitionGroup(transform.position.x, transform.position.y);

        //무적 중 적 처치 상태가 아니라면, 10프레임마다 충돌 계산
        if (isStar)
            CheckCollision();
        else
        {
            takeDamageEachFrame++;
            if (takeDamageEachFrame > takeDamageEachFrameSet)
            {
                CheckCollision();
                takeDamageEachFrame = 0;
            }
        }

        //임시
        //if (Input.GetKeyDown(KeyCode.Z))
            //GameObject.Find("CardCast").GetComponent<CardCast>().CastCardWithID(10);
    }

    private void PlayerMove()
    {
        //플레이어 이동
        Vector3 moveDirection = Vector3.zero;
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(inputHorizontal, inputVertical, 0);

        Vector3 playerPos = transform.position + moveDirection * Time.deltaTime * moveSpeed;
        //플레이어가 100x100 파티션을 벗어나지 않도록 범위 제한
        transform.position = new Vector3(Mathf.Clamp(playerPos.x, -49.5f, 49.5f), Mathf.Clamp(playerPos.y, -49.5f, 49.5f), 0);

        //플레이어 이동시 애니메이터 파라미터 설정
        if (inputHorizontal != 0 || inputVertical != 0)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
        //플레이어 이동 방향에 따른 이미지 Flip
        if (inputHorizontal != 0)
        {
            if (inputHorizontal < 0)
            {
                isFliped = true;
                spriteRenderer.flipX = true;
            }
            else
            {
                isFliped = false;
                spriteRenderer.flipX = false;
            }
        }
    }

    private void CheckCollision()
    {
        // 1칸 내 파티션 그룹 리스트
        List<int> surroundingPartitionGroups = Partition.GetExpandedPartitionGroups(partitionGroup);
        // 1칸 내 파티션 그룹의 적 오브젝트 리스트
        List<EnemyController> surroundingEnemies = Partition.GetAllEnemiesInPartitionGroups(surroundingPartitionGroups);

        //surroundingEnemies의 적 오브젝트들과 거리 비교
        foreach (EnemyController enemy in surroundingEnemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < hitBoxRadius)
            {
                //무적 상태 처리
                if (isInvincible || isMedInv)
                {
                    //무적 상태에서 적 처치 여부 확인
                    if (isStar)
                    {
                        if (!enemy.isBoss)
                            enemy.KillEnemy();
                        continue;   //적 처치 시 모든 적을 대상으로 해야하기 때문에 continue로 다음 적 검색 (break시 다른 오브젝트들과의 충돌 처리를 못함)
                    }
                    break;
                }
                // 적과의 거리가 히트박스거리보다 가까우면 데미지 받음 (데미지 감소 처리)
                if (isOnMed)
                    ModifyHealth(-(enemy.damage * 0.5f));
                else
                    ModifyHealth(-enemy.damage);
                // 피격시 이동속도 감소
                ModifyMoveSpeed(-1f, 1f);
                HitAnimation();
                break;
            }
        }
    }


    // val 만큼 체력 변경
    public void ModifyHealth(float val)
    {
        health += val;
        if (health > maxhealth)
            health = maxhealth;
        if (health <= 0)
            KillPlayer();
    }

    // duration동안 val 만큼 이동속도 조정
    public void ModifyMoveSpeed(float val, float duration)
    {
        if (setMoveSpeed != null)
        {
            moveSpeed = basicMoveSpeed;
            StopCoroutine(setMoveSpeed);
        }
        setMoveSpeed = StartCoroutine(SetMoveSpeed(val, duration));
    }

    //피격시 이펙트 처리를 위한 함수
    public void HitAnimation()
    {
        if(hitAnamation != null)
        {
            StopCoroutine(hitAnamation);
        }
        hitAnamation = StartCoroutine(HitAnimationCorutine());
    }

    IEnumerator SetMoveSpeed(float val, float duration)
    {
        if (moveSpeed <= 0)
            yield break;
        moveSpeed += val;
        yield return new WaitForSeconds(duration);
        moveSpeed = basicMoveSpeed;
    }

    IEnumerator HitAnimationCorutine()
    {
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(1f);
        spriteRenderer.color = Color.white; 
    }

    //플레이어 사망 시 호출
    private void KillPlayer()
    {
        GameOverManager.Instance.TriggerGameOver();
        //Destroy(gameObject);
    }
}
