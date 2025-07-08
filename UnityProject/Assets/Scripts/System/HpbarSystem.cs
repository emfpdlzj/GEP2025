using UnityEngine;
using UnityEngine.UI;
//플레이어 체력 hpbar 함수
public class HpbarSystem : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private PlayerControll player;

    private void Start()
    {
        player = FindObjectOfType<PlayerControll>();

        if (player == null)
        {
            Debug.LogError(" PlayerControll 찾을 수 없음");
            enabled = false;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float current = player.Health;
        float max = player.MaxHealth;
        hpSlider.value = current / max;
    }
}
