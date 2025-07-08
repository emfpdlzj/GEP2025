//게임 시작시 카드 덱 세팅하는 함수. 
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private List<CardData> allCardData;
    [SerializeField] private int startcardnumber;
    private bool isInitialized = false;

    private void Start()
    {

        if (isInitialized) return;

        var firstThree = allCardData.OrderBy(_ => Random.value).Take(startcardnumber);
        foreach (var data in firstThree)
        {
            Card card = new(data);
            PlayerDeck.Instance.AddCard(card);

            Vector2 anchoredPos = Vector2.zero;
            CardView view = CardViewCreator.Instance.CreateCardView(card, anchoredPos);
            StartCoroutine(handView.AddCard(view));
        }

        isInitialized = true;
    }

}
