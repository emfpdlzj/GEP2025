using UnityEngine;

// 싱글톤 패턴: 게임 내에서 하나만 존재하는 오브젝트 만들 때 사용
// T는 MonoBehaviour를 상속받는 클래스여야 함
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; } // 인스턴스 접근용 (전역에서 사용 가능)

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // 이미 존재하면 중복 제거
            return;
        }
        Instance = this as T; // 인스턴스 설정
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null; // 종료 시 정리
        //Destroy(gameObject); // 씬 안에 있던 오브젝트 제거
    }
}

// 씬 전환해도 안 사라지는 싱글톤
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // 씬 바뀌어도 살아있게 함
    }
}
