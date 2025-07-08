using System.Collections;
using UnityEngine;

public class CardCast : MonoBehaviour
{
    //카드 ID (공격 (0~19) 서포트 (20~40))
    // 0 : 하늘가르기
    // 1 : 빅뱅
    // 2 : 철선삼
    // 3 : 파이어브레스
    // 4 : 블러드로드
    // 5 : 은탄
    // 6 : 엑스칼리버
    // 7 : 점착폭탄
    // 8 : 세로베기
    // 9 : 불의폭풍
    // 10 : 위성
    //-------------------------------------------------- 
    // 20 : 고속질주
    // 21 : 비상 물약
    // 22 : 암흑의 결계 DarkBarrier
    // 23 : 초회복약
    // 24 : 명상
    // 25 : 시간왜곡
    // 26 : 놀라운회복력
    // 27 : 지진
    public GameObject playerObject;
    private PlayerControll playerScript;
    private Animator playerAnim;
    private Vector3 playerPos;


    //공격오브젝트 ID
    //0 : 하늘가르기
    //1 : 빅뱅
    //2 : 철선삼
    //3~5 : 암흑의 결계
    //6 : 파이어 브레스
    //7 : 블러드로드
    //8 : 시간왜곡
    //9 : 은탄
    //10 : 엑스칼리버
    //11 : 점착폭탄
    //12 : 지진
    //13 : 세로베기 (이펙트)
    //14 : 세로베기 (판정)
    //15 : 불의폭풍
    //16 : 위성
    public GameObject[] attackObjects = new GameObject[30];

    //공격사운드
    //0 : 하늘가르기
    //1 : 빅뱅
    //2 : 철선삼
    //3 : 암흑의결계
    //4 : 파이어 브레스
    //5 : 블러드 로드1 (준비)
    //6 : 블러드 로드2 (발사)
    //7 : 시간왜곡
    //8 : 은탄
    //9 : 점착폭탄1
    //10 : 점착폭탄2
    //11 : 지진
    //12 : 불의폭풍
    //13 : 위성
    //14 : 비상물약, 초회복약
    //15 : 명상, 놀라운회복력
    //16 : 고속질주
    //17 : 세로베기
    //18 : 엑스칼리버
    public AudioClip[] attackSounds = new AudioClip[30];
    public AudioSource audioSource; //  추가: 효과음 재생용 오디오 소스
    
    //비상물약 변수
    private float hpPotionDuration = 0;
    private float hpPotionTime = 0;
    private bool isHealing = false;
    private float amountPerFrame;

    //은탄 변수
    private int bulletCount = 6;
    private int count = 0;
    private float bulletTime = 0;
    private float bulletTimeSet = 1;
    private bool isBulletFire = false;

    //코루틴 (중복 사용 방지용)
    private Coroutine agility;
    private Coroutine meditation;

    void Start()
    {
        playerScript = playerObject.GetComponent<PlayerControll>();
        playerAnim = playerObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!GameOverManager.Instance.isGameOver)
            playerPos = playerObject.transform.position;
    }

    private void FixedUpdate()
    {
        //비상물약
        if (isHealing)
        {
            hpPotionTime += Time.deltaTime;
            if (hpPotionTime >= hpPotionDuration)
            {
                isHealing = false;
                hpPotionTime = 0;
            }
            playerScript.ModifyHealth(amountPerFrame);
        }

        //은탄
        if (isBulletFire)
        {
            if (count >= bulletCount)
                isBulletFire = false;

            bulletTime += Time.deltaTime;
            if (bulletTime >= bulletTimeSet)
            {
                bulletTime = 0;
                count++;
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[8], 1.0f);
                if (playerScript.isFliped)
                    Instantiate(attackObjects[9], playerPos, Quaternion.Euler(0, 0, 180));
                else
                    Instantiate(attackObjects[9], playerPos, Quaternion.Euler(0, 0, 0));
            }
        }
    }



    //leftright
    //0 : 기본
    //1 : 카드가 손패 가장 왼쪽에 있음
    //2 : 카드가 손패 가장 오른쪽에 있음
    public void CastCardWithID(int id, int leftright = 0, AudioClip clip = null)
    {
        switch (id)
        {
            //하늘가르기     왼쪽 : 위방향 추가 발사      오른쪽 : 아래방향 추가 발사
            case 0:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[0], 1.0f);
                //공격모션 존재
                playerAnim.SetTrigger("Attacking");
                Instantiate(attackObjects[0], playerPos, Quaternion.Euler(0, 0, 0));
                Instantiate(attackObjects[0], playerPos, Quaternion.Euler(0, 0, 180));
                if (leftright == 1)
                    Instantiate(attackObjects[0], playerPos, Quaternion.Euler(0, 0, 90));
                else if (leftright == 2)
                    Instantiate(attackObjects[0], playerPos, Quaternion.Euler(0, 0, 270));
                break;

            //빅뱅
            case 1:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[1], 1.0f);
                Instantiate(attackObjects[1], playerPos, Quaternion.identity);
                break;

            //철선섬
            case 2:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[2], 1.0f);
                if (!playerScript.isFliped)
                {
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 60));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 30));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 0));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, -30));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, -60));
                }
                else
                {
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 240));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 210));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 180));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 150));
                    Instantiate(attackObjects[2], playerPos, Quaternion.Euler(0, 0, 120));
                }
                break;

            //파이어브레스
            case 3:
                Instantiate(attackObjects[6], playerPos, Quaternion.identity);
                break;

            //블러드로드
            case 4:
                Instantiate(attackObjects[7], playerPos, Quaternion.identity);
                break;

            //은탄
            case 5:
                count = 0;
                bulletTime = 0;
                isBulletFire = true;
                break;

            //엑스칼리버
            case 6:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[18], 1.0f);
                //공격모션존재
                playerAnim.SetTrigger("Attacking");
                if (playerScript.isFliped)
                {
                    GameObject calibur = Instantiate(attackObjects[10], playerPos + new Vector3(-2, 1, 0), Quaternion.identity);
                    calibur.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    Instantiate(attackObjects[10], playerPos + new Vector3(2, 1, 0), Quaternion.identity);
                }
                break;

            //점착폭탄
            case 7:
                Instantiate(attackObjects[11], playerPos, Quaternion.identity);
                break;

            //세로베기
            case 8:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[17], 1.0f);
                //공격모션존재
                playerAnim.SetTrigger("Attacking");
                Instantiate(attackObjects[13], playerPos, Quaternion.identity);
                Instantiate(attackObjects[14], playerPos + new Vector3(0, 2, 0), Quaternion.identity);
                Instantiate(attackObjects[14], playerPos + new Vector3(0, -2, 0), Quaternion.identity);
                break;

            //불의폭풍
            case 9:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[12], 1.0f);
                for (int i = 0; i < 6; i++)
                {
                    Vector3 attackPos = (Vector2)playerPos + (Random.insideUnitCircle * 6);
                    Instantiate(attackObjects[15], attackPos, Quaternion.identity);
                }
                break;

            //위성
            case 10:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[13], 1.0f);
                Instantiate(attackObjects[16], playerPos, Quaternion.identity);
                break;

            //고속질주      기본 : 3초간 이속 +2      왼쪽 : 6초간 이속 +2              오른쪽 : 3초간 이속 +4
            case 20:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[16], 1.0f);
                if (agility != null)
                    StopCoroutine(agility);
                if (leftright == 1)
                    agility = StartCoroutine(Agility(2, 6));
                else if (leftright == 2)
                    agility = StartCoroutine(Agility(4, 3));
                else
                    agility = StartCoroutine(Agility(2, 3));
                break;

            //비상물약      기본 : 2초간 10 회복      왼쪽 : 0.1초(즉시) 10 회복        오른쪽 : 2초간 15 회복
            case 21:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[14], 1.0f);
                if (leftright == 1)
                    HPPotion(10f, 0.1f);
                else if (leftright == 2)
                    HPPotion(15f, 2f);
                else
                    HPPotion(10f, 2f);
                break;

            //암흑의 결계   기본 : 3초 반지름3        왼쪽 : 3초 반지름4                오른쪽 : 4초 반지름 3 
            case 22:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[3], 1.0f);
                if (leftright == 1)
                    Instantiate(attackObjects[4], playerPos, Quaternion.identity);
                else if (leftright == 2)
                    Instantiate(attackObjects[5], playerPos, Quaternion.identity);
                else
                    Instantiate(attackObjects[3], playerPos, Quaternion.identity);
                break;

            //초회복약      기본 : 즉시 40 회복       오른쪽 : 즉시 60 회복  
            case 23:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[14], 1.0f);
                if (leftright == 2)
                    HPPotion(40f, 0.1f);
                else
                    HPPotion(50f, 0.1f);
                break;

            //명상          기본 : 5초간 받는데미지 절반                                오른쪽 : 무적으로 변환
            case 24:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[15], 1.0f);
                if (meditation != null)
                    StopCoroutine(meditation);
                meditation = StartCoroutine(Meditation(leftright));
                break;

            //시간왜곡
            case 25:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[7], 1.0f);
                Instantiate(attackObjects[8], playerPos, Quaternion.identity);
                break;

            //놀라운회복력  기본 : 20초간 20회복      왼쪽 : 10초간 20회복
            case 26:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[15], 1.0f);
                if (leftright == 1)
                    HPPotion(20f, 10f);
                else
                    HPPotion(20f, 20f);
                break;
            
            //지진
            case 27:
                //공격사운드 재생
                audioSource.PlayOneShot(attackSounds[11], 1.0f);
                Instantiate(attackObjects[12], playerPos, Quaternion.identity);
                break;
        }
    }

    IEnumerator Agility(float speed, float dur)
    {
        playerScript.isInvincible = true;
        playerScript.isStar = true;
        playerScript.trail.SetActive(true);
        playerScript.ModifyMoveSpeed(speed, dur);
        yield return new WaitForSeconds(dur);
        playerScript.isInvincible = false;
        playerScript.trail.SetActive(false);
        playerScript.isStar = false;
    }

    IEnumerator Meditation(int lr)
    {
        playerScript.isOnMed = true;
        if (lr == 2)
            playerScript.isMedInv = true;
        yield return new WaitForSeconds(5.0f);
        playerScript.isOnMed = false;
        playerScript.isMedInv = false;
    }

    private void HPPotion(float val, float dur)
    {
        //1프레임 당 회복량 (지속시간 * 초당프레임(50) / 회복량)
        amountPerFrame = val / (dur * 50f);
        hpPotionDuration = dur;
        isHealing = true;
    }
}
