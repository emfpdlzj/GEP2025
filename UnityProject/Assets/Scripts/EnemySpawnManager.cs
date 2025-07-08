using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour
{
    private int wave = 0;                   // 현재 웨이브
    public int waveSize = 2;                // 총 웨이브 수

    public Wave[] waves = new Wave[10];

    private float currentSpawnTime;         // 현재 적 생성 주기
    private float spawnTime = 0;

    private int currentSpawnCount;          // 현재 웨이브에서 생성할 적 그룹 수
    private int spawnCount = 0;

    private bool isBossSpawned;             // 보스 등장 여부

    public Transform enemyHolder;           // 일반 적 부모 오브젝트
    public Transform bossHolder;            // 보스 부모 오브젝트

    [Header("Clear 연출 ")]
    public CanvasGroup clearImageCanvasGroup;
    public string clearSceneName = "ClearScene";

    [Header("사운드")]
    public AudioSource sfxSource;               // 효과음 소스
    public AudioClip clearSFX;                  // 클리어 효과음

    public bool isGameCleared = false; //게임 클리어 알려주는 변수

    private void Start()
    {
        InitializeWave();
    }

    private void Update()
    {
        if (wave >= waveSize)
        {
            StartCoroutine(HandleGameClear());
            enabled = false;
            return;
        }
        if (spawnCount >= currentSpawnCount)
        {
            spawnCount = 0;
            SpawnBoss();
            isBossSpawned = true;
        }

        if (!isBossSpawned)
        {
            spawnTime += Time.deltaTime;

            if (spawnTime >= currentSpawnTime)
            {
                spawnTime = 0;
                spawnCount++;
                SpawnEnemies();
            }
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < waves[wave].enemyCount.Length; i++)
        {
            for (int j = 0; j < waves[wave].enemyCount[i]; j++)
            {
                float deg = Random.Range(0, 360);
                float cos = Mathf.Cos(deg * Mathf.Deg2Rad) * 20f;
                float sin = Mathf.Sin(deg * Mathf.Deg2Rad) * 20f;
                Vector3 spawnCircle = GameManager.instance.player.transform.position + new Vector3(cos, sin, 0);
                Vector3 spawnPos = new Vector3(Mathf.Clamp(spawnCircle.x, -49f, 49f), Mathf.Clamp(spawnCircle.y, -49f, 49f), 0);
                GameObject enemyP = Instantiate(waves[wave].enemyPrefabs[i], enemyHolder);
                enemyP.transform.position = spawnPos;
                enemyP.transform.parent = enemyHolder;

                EnemyController enemyScript = enemyP.GetComponent<EnemyController>();
                int partitionGroup = GameManager.instance.GetPartitionGroup(enemyP.transform.position.x, enemyP.transform.position.y);
                enemyScript.partitionGroup = partitionGroup;

                GameManager.instance.AddToEnemyGroup(partitionGroup, enemyScript);
                GameManager.instance.AddEnemyBatch(enemyScript);
            }
        }
    }

    void SpawnBoss()
    {
        float deg = Random.Range(0, 360);
        float cos = Mathf.Cos(deg * Mathf.Deg2Rad) * 20f;
        float sin = Mathf.Sin(deg * Mathf.Deg2Rad) * 20f;
        Vector3 spawnCircle = GameManager.instance.player.transform.position + new Vector3(cos, sin, 0);
        Vector3 spawnPos = new Vector3(Mathf.Clamp(spawnCircle.x, -49f, 49f), Mathf.Clamp(spawnCircle.y, -49f, 49f), 0);
        GameObject boss = Instantiate(waves[wave].bossPrefab, enemyHolder);
        boss.transform.position = spawnPos;
        boss.transform.parent = bossHolder;

        EnemyController enemyScript = boss.GetComponent<EnemyController>();
        int partitionGroup = GameManager.instance.GetPartitionGroup(boss.transform.position.x, boss.transform.position.y);
        enemyScript.partitionGroup = partitionGroup;

        GameManager.instance.AddToEnemyGroup(partitionGroup, enemyScript);
        GameManager.instance.AddEnemyBatch(enemyScript);
    }

    public void NextWave()
    {

        isBossSpawned = false;
        Time.timeScale = 0f;
        if (wave + 1 >= waveSize)
        {
            Debug.Log("마지막 웨이브 클리어 → 카드 선택 없이 바로 종료 진행");
            Time.timeScale = 1f;
            wave++;
            InitializeWave();
            return;
        }

        if (SelectCardSystem.Instance == null)
        {
            Debug.LogError("❌ SelectCardSystem.Instance가 null입니다!");
            return;
        }

        SelectCardSystem.Instance.ShowCardChoices(OnCardSelected);
    }

    private void OnCardSelected(Card selectedCard)
    {
        PlayerDeck.Instance.AddCard(selectedCard);
        Time.timeScale = 1f;

        wave++;
        InitializeWave();
    }

    void InitializeWave()
    {
        currentSpawnTime = waves[wave].spawnTime;
        currentSpawnCount = waves[wave].spawnCount;
        spawnTime = 0;
        spawnCount = 0;
        isBossSpawned = false;
    }

    private IEnumerator HandleGameClear()
    {
        // 게임 클리어 직전 설정
        isGameCleared = true;
        HandView.Instance?.SetCardInputBlocked(true); // 카드 추가, 입력 막기
        // 1. 모든 적 제거
        foreach (Transform enemy in enemyHolder)
            Destroy(enemy.gameObject);
        foreach (Transform boss in bossHolder)
            Destroy(boss.gameObject);
        // 모든 카드 비활성화
        HandView.Instance?.HideAllCards();
        // 2. 배경음악 정지
        MusicManager.Instance?.StopMusic();
        // 3. 클리어 효과음
        if (sfxSource != null && clearSFX != null)
            sfxSource.PlayOneShot(clearSFX);

        // 4. Fade In
        float duration = 3f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            clearImageCanvasGroup.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        // 5. 씬 전환
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(clearSceneName);
    }

    //웨이브 UI 참조용
    public int GetWave()
    {
        return wave+1;
    }
}


//웨이브 구조체
[System.Serializable]
public struct Wave
{
    //enemyPrefabs와 enemyCount의 Size는 반드시 같아야 한다.
    [Tooltip("소환 간격 (N초마다 적 무리 소환)")]
    public float spawnTime;                 //적 생성 시간(주기)
    [Tooltip("소환 횟수 (적 무리를 몇 번 소환할지)")]
    public int spawnCount;                  //웨이브 중 적이 소환되는 횟수
    public GameObject[] enemyPrefabs;       //적 프리팹 목록
    public int[] enemyCount;                //적 프리팹 별로 한 주기마다 생성될 수
    public GameObject bossPrefab;                 //마지막에 생성될 보스 프리팹
}