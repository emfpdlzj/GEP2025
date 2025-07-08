using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
//프리팹 CardView를 관리하는 함수 
public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rt;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;
    private Vector2 originalHoverPos;
    [SerializeField] private Image cardImage;
    public Card Card { get; private set; }
    private bool isInteractive = true;
    public void SetInteractive(bool enabled)
    {
        isInteractive = enabled;
    }
    public bool IsInteractive => isInteractive;
    public void SetAlpha(float value)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = value;
    }
    public void Setup(Card card)
    {
        Card = card;
        cardImage.sprite = card.Image;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractive) return;
        //  원래 위치를 최신 위치로 갱신
        originalHoverPos = rt.anchoredPosition;
        rt.DOAnchorPos(originalHoverPos + new Vector2(0, 80), 0.15f).SetEase(Ease.OutQuad);
        CardViewHoverSystem.Instance?.Show(Card, GetAnchoredPosition());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractive) return;
        rt.DOAnchorPos(originalHoverPos, 0.15f).SetEase(Ease.OutQuad);
        CardViewHoverSystem.Instance?.Hide();
    }

    private Vector2 GetAnchoredPosition()
    {
        RectTransform rt = GetComponent<RectTransform>();
        return rt.anchoredPosition;
    }
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        // 자식 중 첫 번째 Image 자동 연결
        if (cardImage == null)
            cardImage = GetComponentInChildren<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractive) return;
        //Debug.Log("Drag 시작");
        originalPos = rt.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        CardViewHoverSystem.Instance.Hide();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractive) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        rt.anchoredPosition = localPoint;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractive) return;
        canvasGroup.blocksRaycasts = true;
        HandView.Instance.TryUseCard(this, rt.anchoredPosition);
    }
}