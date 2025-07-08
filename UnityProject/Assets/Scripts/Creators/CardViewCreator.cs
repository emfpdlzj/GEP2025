using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;  // UI 카드 프리팹

    public CardView CreateCardView(Card card, Vector2 localPos)
{
    if (card == null)
    {
        Debug.LogError(" CreateCardView: card가 null입니다!");
        return null;
    }

    if (cardViewPrefab == null)
    {
        Debug.LogError(" cardViewPrefab이 null입니다! 프리팹 연결 안 됨");
        return null;
    }

    if (HandView.Instance == null)
    {
        Debug.LogError(" HandView.Instance가 null입니다!");
        return null;
    }

    CardView cardView = Instantiate(cardViewPrefab, HandView.Instance.transform);
    if (cardView == null)
    {
        Debug.LogError(" Instantiate 결과가 null입니다!");
        return null;
    }

    var rt = cardView.GetComponent<RectTransform>();
    if (rt == null)
    {
        Debug.LogError(" RectTransform이 없음");
        return null;
    }

    cardView.Setup(card);
    return cardView;
}

}
