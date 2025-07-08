using UnityEngine;

// 실제 게임 중 사용되는 카드 클래스 (런타임 카드)
// CardData를 참조해서 이름, 설명, 이미지 등 가져옴
public class Card
{
    public int Id => data.Id;
    public Sprite Image => data.Image;
    public float Cooltime { get; private set; } 

    private readonly CardData data;
    public Card(CardData cardData)
    {
        data = cardData;
        Cooltime = data.CoolTime; // 생성자 안에서 안전하게 접근
    }
}
