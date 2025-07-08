    using UnityEngine;
    using System.Collections.Generic;

    // 플레이어가 소유한 덱 (카드 데이터만 저장)
    // 화면에 보이는 건 HandView에서 따로 관리함
    public class PlayerDeck : Singleton<PlayerDeck>
    {
        private List<Card> cards = new(); // 덱 내부 카드 리스트

        // 카드 추가 (덱에만 추가됨, 화면에는 안 뜸)
        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        // 카드 사용 (덱에서 제거)
        public void UseCard(Card card)
        {
            cards.Remove(card);
            // 이후 CardEffectSystem 같은 시스템과 연동 가능
        }

        // 현재 덱에 있는 카드 리스트 복사본 반환
        public List<Card> GetCards() => new(cards);
    }
