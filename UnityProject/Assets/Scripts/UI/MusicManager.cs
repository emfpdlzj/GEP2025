using UnityEngine;
using UnityEngine.UI;
//배경음악 재생용
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioSource bgm;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }

    void Start()
    {
        bgm.Play();
    }

    public void StopMusic()
    {
        if (bgm.isPlaying)
            bgm.Stop();
    }
}
