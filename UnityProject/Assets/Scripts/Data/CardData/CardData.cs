using UnityEngine;
[CreateAssetMenu(menuName = "Data/Card")]
//카드 데이터 관련 cs
public class CardData : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public float CoolTime { get; private set; } //  쿨타임 추가
}
