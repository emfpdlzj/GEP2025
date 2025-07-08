using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [SerializeField] private DimOverlayController dimOverlay;
    [SerializeField] private TextMeshProUGUI clickText;
    [SerializeField] private string startSceneName = "StartScene";
    [SerializeField] private AudioSource sfxSource;     // 효과음용 AudioSource
    [SerializeField] private AudioClip gameOverSFX;     // 게임오버 효과음
    [SerializeField] private UnityEngine.UI.Image gameOverImage;//게임오버 이미지


    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // 시작 시 비활성화 또는 투명하게
        clickText.alpha = 0f;
        clickText.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // 게임 정지
        Time.timeScale = 0f;
        HandView.Instance?.HideAllCards();
        // 배경음악 정지
        MusicManager.Instance?.StopMusic();

        // 효과음 재생
        if (sfxSource != null && gameOverSFX != null)
            sfxSource.PlayOneShot(gameOverSFX);

        // 딤 오버레이 보여주기
        dimOverlay?.Show(1.0f, 1.0f);

        // 텍스트 나타내기
        clickText.gameObject.SetActive(true);
        clickText.DOFade(1f, 0.4f).SetUpdate(true);
        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(true);
            Color c = gameOverImage.color;
            c.a = 0f;
            gameOverImage.color = c;

            gameOverImage.DOFade(1f, 0.4f).SetUpdate(true); // 이미지 페이드인
        }
    }


    private void Update()
    {
        if (!isGameOver) return;

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            Time.timeScale = 1f; // 정지 해제
            SceneManager.LoadScene(startSceneName);
        }
    }
}
