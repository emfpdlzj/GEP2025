using UnityEngine;
using DG.Tweening;

public class EndingPlayerWalk : MonoBehaviour
{
    public float walkDuration = 5f; // 걷는 시간
    public Vector3 endPosition = new Vector3(8f, 0f, 0f); // 도착 지점 (엔딩씬에 맞게 조정)
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isWalking", true); // 걷기 애니메이션 시작
        }

        // 오른쪽으로 걷기
        transform.DOMove(endPosition, walkDuration)
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     if (animator != null)
                         animator.SetBool("isWalking", false); // 도착하면 멈춤

                     // 추가 연출 넣을 수 있음 (예: 엔딩 문구 뜨기, 페이드 아웃 등)
                     Debug.Log("도착 완료");
                 });
    }
}
