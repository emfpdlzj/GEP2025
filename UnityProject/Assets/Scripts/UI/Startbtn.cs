using UnityEngine;
using UnityEngine.SceneManagement;
//스타트버튼용 
public class Startbtn : MonoBehaviour
{

    public void LoadNextScene()
    {
        // 효과음 다 들릴 수 있도록 딜레이 후 씬 전환
        Invoke("GoToScene", 0.3f);
    }

    private void GoToScene()
    {
        SceneManager.LoadScene("GameScene");
    }

}
