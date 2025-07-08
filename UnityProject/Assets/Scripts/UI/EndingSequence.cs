using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingSequence : MonoBehaviour
{
    public Image imageObject;          // UI용 Image 컴포넌트 직접 할당
    public float fadeDuration = 2f;    // 페이드인 시간

    private void Start()
    {
        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        // 1. 6.5초 대기
        yield return new WaitForSeconds(6.5f);

        // 2. 이미지 페이드인
        if (imageObject != null)
        {
            imageObject.gameObject.SetActive(true);

            // 시작 알파값 0
            Color c = imageObject.color;
            c.a = 0f;
            imageObject.color = c;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Clamp01(t / fadeDuration);
                c.a = alpha;
                imageObject.color = c;
                yield return null;
            }
        }
    }
}
