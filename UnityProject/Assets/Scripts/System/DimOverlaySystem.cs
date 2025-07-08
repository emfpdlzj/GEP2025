using UnityEngine;
using DG.Tweening;

public class DimOverlayController : MonoBehaviour
{   //SelectCardSystem, GameOverManager에서 사용하는 화면 어두워지는 함수.
    [SerializeField] private CanvasGroup overlayGroup;

    public void Show(float alpha = 0.6f, float duration = 0.25f)
    {
        //Debug.Log("DimOverlay Show 실행됨");
        gameObject.SetActive(true);
        overlayGroup.alpha = 0f;

        overlayGroup.DOFade(alpha, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true); // timeScale 무시하고 애니메이션 수행
    }

    public void Hide(float duration = 0.25f)
    {
        overlayGroup.DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true) // timeScale 무시하고 애니메이션 수행
            .OnComplete(() => gameObject.SetActive(false));
    }
}
