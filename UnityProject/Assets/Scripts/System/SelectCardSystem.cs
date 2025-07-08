using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class SelectCardSystem : Singleton<SelectCardSystem>
{
    [SerializeField] private RectTransform selectCardPanel;

    [SerializeField] private List<CardData> allCardData; // Inspector에 직접 할당
    [SerializeField] private DimOverlayController dimOverlay;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip openSelectSFX;
    private List<CardData> cardPool;
    private readonly List<CardView> options = new();
    private System.Action<Card> onCardSelectedCallback;
    private bool cardSelected = false;

    protected override void Awake()
    {
        base.Awake(); // 이거 꼭 넣어줘야 Instance가 초기화됨
        cardPool = new List<CardData>(allCardData);
    }


    public void ShowCardChoices(System.Action<Card> callback)
    {
        Debug.Log("ShowCardChoices() 진입");
        Debug.Log("cardPool.Count: " + cardPool.Count);
        Debug.Log("현재 덱 카드 수: " + PlayerDeck.Instance.GetCards().Count);

        onCardSelectedCallback = callback;
        cardSelected = false;
        if (cardPool == null || cardPool.Count == 0)
        {
            Debug.LogError("카드 풀이 비어있습니다.");
            return;
        }

        if (cardPool.Any(cd => cd == null))
        {
            Debug.LogError("cardPool 안에 null CardData 있음!");
            return;
        }

        if (CardViewCreator.Instance == null)
        {
            Debug.LogError("CardViewCreator.Instance가 null입니다!");
            return;
        }

        if (selectCardPanel == null)
        {
            Debug.LogError("selectCardPanel이 null입니다!");
            return;
        }
        dimOverlay?.Show(); //어두운 배경 켜기  
        Vector2 center = Vector2.zero;

        var unused = cardPool
    .Where(cd => !PlayerDeck.Instance.GetCards().Any(c => c.Id == cd.Id))
    .OrderBy(_ => Random.value)
    .Take(3)
    .ToList(); //  지연 평가 방지

        Debug.Log("실제 선택 후보 카드 수: " + unused.Count());
        float spacing = 300f; // 카드 간 간격 ( 원하면 조절 가능)
        int index = 0;

        foreach (var data in unused)
        {
            // 중앙 기준 좌우로 퍼지게 배치: -1, 0, +1
            float xOffset = (index - 1) * spacing;
            Vector2 anchoredPos = new Vector2(xOffset, 0f);

            Card card = new(data);
            CardView view = CardViewCreator.Instance.CreateCardView(card, Vector2.zero);

            //  카드 부모 지정 후 위치 설정
            view.transform.SetParent(selectCardPanel, false);
            view.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

            view.gameObject.AddComponent<CardSelectOption>();
            view.SetInteractive(false);

            options.Add(view);
            index++;
        }

        selectCardPanel.gameObject.SetActive(true);
        if (sfxSource != null && openSelectSFX != null)
        {
            sfxSource.PlayOneShot(openSelectSFX);
        }
        if (messageText != null)
        {
            messageText.text = "카드를 한 장 선택하세요!";
            messageText.gameObject.SetActive(true);
        }
    }

    public void OnCardSelected(Card selected)
    {
        if (cardSelected) return;
        cardSelected = true;

        // 1. 덱에 직접 추가
        PlayerDeck.Instance.AddCard(selected);

        // 2. 카드 뷰는 큐에 보관 (직접 생성 X)
        StartCoroutine(DeferCardCreation(selected));

        // 3. 외부 콜백 실행 (로그 기록 등)
        onCardSelectedCallback?.Invoke(selected);

        // 4. UI 정리
        foreach (var v in options)
            if (v != null) Destroy(v.gameObject);
        options.Clear();

        selectCardPanel.gameObject.SetActive(false);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
        HandView.Instance.SetWaitUntilCardSelectFinished(false);
        HandView.Instance.ResumeQueuedCards();
        dimOverlay?.Hide();
    }

    private IEnumerator DeferCardCreation(Card selected)
    {
        // 한 프레임 기다리면 UI 사라지고 화면 정리된 다음 카드 생성됨
        yield return null;

        var view = CardViewCreator.Instance.CreateCardView(selected, Vector2.zero);
        if (view != null)
            StartCoroutine(HandView.Instance.AddCard(view));
    }
}

public class CardSelectOption : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectCardSystem.Instance.OnCardSelected(GetComponent<CardView>().Card);
    }
}



