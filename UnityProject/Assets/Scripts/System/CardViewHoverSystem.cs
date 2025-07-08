using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{   //호버기능 관리하는 메소드
    [SerializeField] private CardView hoverView;
    [SerializeField] private RectTransform canvasRect;

    public void Show(Card card, Vector2 anchoredPos)
    {
        hoverView.gameObject.SetActive(true);
        hoverView.Setup(card);
        var rt = hoverView.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos + new Vector2(0, 200); // 화면 중앙에서 살짝 위에
    }

    public void Hide()
    {
        hoverView.gameObject.SetActive(false);
    }
}
