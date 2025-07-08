using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //다른 스크립트에서의 접근을 위한 게임매니저 스태틱 instance 선언
    public static GameManager instance;

    //다른 스크립트에서의 PlayerControll 스크립트 접근을 위한 선언
    public GameObject player;
    PlayerControll playerScript;
    public PlayerControll PlayerScript
    {
        get { return playerScript; }
    }


    /*
     * 프로젝트 설정상 FixedUpdate()는 1초에 50프레임 호출
     * FixedUpdate()에서 runTime을 Time.deltaTime만큼 증가시키면 매 프레임 당 0.02씩 증가
     * runTime값에 50을 곱하면 프레임 당 1,2,3,4.... 값
     * Batch : 적, 발사체 등을 집단으로 나누어 한 프레임에 다 처리하지 않고 순차적으로 처리
     * 예) enemyBatches[runTime*50].runLogic()
     */
    //적 Batch 관리
    Dictionary<int, List<EnemyController>> enemyBatches = new Dictionary<int, List<EnemyController>>();
    float runTimer = 0f;
    float runTimerSet = 1f;


    //Partiton Group
    public int MapWidth { get { return MapWidth; } }

    private int mapHeight = 100;                                //맵 높이(100x100)
    public int MapHeight {  get { return mapHeight; } }

    private int partitions = 10000;                             //총 파티션 개수
    public int Partitions {get { return partitions; } }

    //파티션 별 적 그룹
    //인스펙터에서 보이지 않게함
    [HideInInspector]
    public Dictionary<int, HashSet<EnemyController>> enemyGroups = new Dictionary<int, HashSet<EnemyController>>();

    //파티션 별 공격오브젝트 그룹
    [HideInInspector]
    public Dictionary<int, HashSet<AttackObject>> attackObjectGroups = new Dictionary<int, HashSet<AttackObject>>();

    public class BatchScore : System.IComparable<BatchScore>
    {
        public int Id { get; }
        public int Score { get; private set; }

        public BatchScore(int id, int score)
        {
            Id = id;
            Score = score;
        }

        public void UpdateScore(int val)
        {
            Score += val;
        }


        //Batch간 비교 (스코어 우선)
        public int CompareTo(BatchScore other)
        {
            int scoreComparison = Score.CompareTo(other.Score);
            if(scoreComparison == 0)
            {
                //Score가 같으면 ID로 비교
                return Id.CompareTo(other.Id);
            }

            return scoreComparison;
        }
    }

    SortedSet<BatchScore> batchEnemy = new SortedSet<BatchScore>();
    Dictionary<int, BatchScore> batchScoreList_Enemy = new Dictionary<int, BatchScore>();

    //Best Batch에 적 추가
    public void AddEnemyBatch(EnemyController enemy)
    {
        int addedBatch = GetBestBatch("enemy");
        enemy.BatchID = addedBatch;
        enemyBatches[addedBatch].Add(enemy);
    }

    //Enemey가 죽으면 Batch 업데이트
    public void UpdateBatchOnDeath(string tag, int id)
    {
        if (tag == "enemy")
            UpdateBatchOnDeathRaw(batchEnemy, batchScoreList_Enemy, id);
    }

    private void UpdateBatchOnDeathRaw(SortedSet<BatchScore> batchQueue, Dictionary<int, BatchScore> batchScoreList, int id)
    {
        if (batchScoreList.ContainsKey(id))
        {
            //batchEnemy에서 죽은 적 삭제, 해당 id의 score 감소
            BatchScore score = batchScoreList[id];
            batchQueue.Remove(score);
            score.UpdateScore(-1);
            batchQueue.Add(score);
        }
    }

    //스코어가 가장 작은 Batch의 ID 반환
    public int GetBestBatch(string tag)
    {
        if (tag == "enemy")
            return GetBestBatchRaw(batchEnemy);
        else
            return -1;
    }
    private int GetBestBatchRaw(SortedSet<BatchScore> batchQueue)
    {
        BatchScore leastLoadedBatch = batchQueue.Min;

        if (leastLoadedBatch == null)
        {
            return 0;
        }

        batchQueue.Remove(leastLoadedBatch);
        leastLoadedBatch.UpdateScore(1);
        batchQueue.Add(leastLoadedBatch);

        return leastLoadedBatch.Id;
    }

    //Batch 초기화
    private void InitializeBatch()
    {
        for (int i = 0; i < 50; i++)
        {
            BatchScore score = new BatchScore(i, 0);

            enemyBatches.Add(i, new List<EnemyController>());
            batchScoreList_Enemy.Add(i, score);
            batchEnemy.Add(score);
        }
    }

    private void Awake()
    {
        //싱글톤 패턴 사용
        instance = this;
    }
    private void Start()
    {
        playerScript = player.GetComponent<PlayerControll>();

        InitializeBatch();

        //파티션 개수만큼 적 그룹과 공격오브젝트 그룹 추가
        for (int i = 0; i < partitions; i++)
        {
            enemyGroups.Add(i, new HashSet<EnemyController>());
            attackObjectGroups.Add(i, new HashSet<AttackObject>());
        }
    }

    private void FixedUpdate()
    {
        if (playerScript == null) return;

        runTimer += Time.deltaTime;
        if (runTimer >= runTimerSet)
        {
            runTimer = 0f;
        }

        RunBatch((int)(runTimer * 50));
    }

    void RunBatch(int id)
    {
        foreach(EnemyController enemy in enemyBatches[id])
        {
            if (enemy) enemy.RunLogic();
        }
    }

    //x, y좌표에 따라 파티션을 반환  (map 크기 100 x 100 기준)
    public int GetPartitionGroup(float x, float y)
    {
        float calX = x + 50;
        float calY = y + 50;

        int indexX = (int)(calX);
        int indexY = (int)(calY);

        return indexX + indexY * 100;
        //(-50, -50)    : 0
        //(50, -50)     : 99
        //(-50, 50)     : 9900
        //(50, 50)      : 9999
    }

    //적 파티션 그룹에 추가
    public void AddToEnemyGroup(int groupID, EnemyController enemy)
    {
        enemyGroups[groupID].Add(enemy);
    }

    //적 파티션 그룹에서 제거
    public void RemoveFromEnemyGroup(int groupID, EnemyController enemy)
    {
        enemyGroups[groupID].Remove(enemy);
    }

}
