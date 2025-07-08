using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HandView : Singleton<HandView>
{   //덱 Handview를 관리하는 함수 
    [SerializeField] private RectTransform cardPanel;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float globalCooldownDuration = 1f;
    private readonly List<CardView> cards = new();
    private bool isGlobalCooldown = false;
    private bool cardInputBlocked = false;
    private bool waitUntilCardSelectFinished = false;
    public void SetCardInputBlocked(bool blocked) => cardInputBlocked = blocked;
    public void SetWaitUntilCardSelectFinished(bool wait) => waitUntilCardSelectFinished = wait;
    private readonly Queue<Card> pendingCards = new();
    public IEnumerator AddCard(CardView cardView)
    {
        // 게임 클리어 상태라면 추가 안 함
        if (cardInputBlocked) yield break;
        // 선택 카드 UI가 떠 있는 동안은 대기
        while (waitUntilCardSelectFinished)
            yield return null;

        if (cardView == null)
        {
            Debug.LogError("AddCard(): cardView가 null입니다!");
            yield break;
        }
        Debug.Log("AddCard: 카드 1 추가 시도");
        cards.Add(cardView);
        cardView.transform.SetParent(cardPanel, false);

        // 덱에도 등록
        PlayerDeck.Instance.AddCard(cardView.Card);
        //Debug.Log($"setParent 완료 - 카드 개수: {cards.Count}");
        if (isGlobalCooldown)
        {
            cardView.SetAlpha(0.5f);
            cardView.SetInteractive(false);
        }
        yield return null;
        yield return UpdateCardPositions(0.15f); // 정렬. 여기서 반드시 호출돼야 함
    }
    public void RemoveCard(CardView cardView)
    {
        if (cards.Contains(cardView))
        {
            cards.Remove(cardView);
            Destroy(cardView.gameObject);
        }
    }
    public void TryUseCard(CardView cardView, Vector2 dropPosition)
    {
        if (!cardView.IsInteractive || isGlobalCooldown) return;

        int cardIndex = cards.IndexOf(cardView);
        int side = 0;
        if (cardIndex == 0) side = 1; // 왼쪽 끝
        else if (cardIndex == cards.Count - 1) side = 2; // 오른쪽 끝

        var cardCast = FindObjectOfType<CardCast>();
        if (cardCast != null)
        {
            // 카드 ID, 좌·우 정보, 카드 전용 AudioClip 함께 전달
            cardCast.CastCardWithID(cardView.Card.Id, side);
        }
        else
        {
            Debug.LogError("CardCast 인스턴스를 찾을 수 없습니다.");
        }
        PlayerDeck.Instance.UseCard(cardView.Card);

        RemoveCard(cardView);
        StartCoroutine(UpdateCardPositions(0.15f));

        StartCoroutine(HandleGlobalCooldown(cardView.Card));
    }
    private IEnumerator HandleGlobalCooldown(Card usedCard)
    {
        isGlobalCooldown = true;

        foreach (var c in cards)
        {
            c.SetAlpha(0.5f);
            c.SetInteractive(false);
        }

        StartCoroutine(ReAddCardAfterCooldown(usedCard));
        yield return new WaitForSecondsRealtime(globalCooldownDuration);

        foreach (var c in cards)
        {
            c.SetAlpha(1f);
            c.SetInteractive(true);
        }
        isGlobalCooldown = false;
    }
    private IEnumerator ReAddCardAfterCooldown(Card card)
    {
        yield return new WaitForSecondsRealtime(card.Cooltime);

        if (waitUntilCardSelectFinished)
        {
            pendingCards.Enqueue(card); // Card만 큐에 저장
            //Debug.Log("선택 UI 열림 카드 생성 지연");
            yield break;
        }

        CreateAndAddCard(card);
    }
    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null)
            {
                Debug.LogWarning($"⚠ UpdateCardPositions(): cards[{i}]가 null입니다. 건너뜁니다.");
                continue;
            }
            Vector2 pos = GetCardArcPosition(i, cards.Count);
            float rotZ = GetCardArcRotation(i, cards.Count);

            RectTransform rt = cards[i].GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError($"cards[{i}]에 RectTransform이 없습니다.");
                continue;
            }
            rt.DOAnchorPos(pos, duration).SetEase(Ease.OutQuad);
            rt.DOLocalRotate(Vector3.forward * -rotZ, duration).SetEase(Ease.OutQuad);
        }
        yield return new WaitForSecondsRealtime(duration);
    }

    private Vector2 GetCardArcPosition(int index, int totalCards)
    {
        float radiusX = 700f; // 좌우 간격을 좁게
        float radiusY = 1000f; //  위쪽으로 더 많이 펼치기
        float angleStep = 6f; //angleStep을 5~7 사이에서 조정해 부채폭을 다듬음
        int mid = (totalCards - 1) / 2;
        float angle = (index - mid) * angleStep;

        float rad = angle * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad) * radiusX;
        float y = Mathf.Cos(rad) * radiusY;

        return new Vector2(x, y - radiusY + 60f); // y보정은 유지
    }
    private float GetCardArcRotation(int index, int totalCards)
    {
        if (totalCards == 1) return 0f;

        float angleStep = 15f;
        int mid = (totalCards - 1) / 2;

        float angle = (index - mid) * angleStep;
        return angle;
    }
    public void HideAllCards()
    {
        foreach (var cardView in cards)
        {
            cardView.gameObject.SetActive(false);
        }
    }
    public void ResumeQueuedCards()
    {
        StartCoroutine(ProcessPendingCards());
    }
    private IEnumerator ProcessPendingCards()
    {
        while (pendingCards.Count > 0)
        {
            var card = pendingCards.Dequeue();
            CreateAndAddCard(card);
            yield return null;
        }
    }

    private void CreateAndAddCard(Card card)
    {
        var view = CardViewCreator.Instance.CreateCardView(card, Vector2.zero);
        if (view != null)
            StartCoroutine(AddCard(view));
    }

}
